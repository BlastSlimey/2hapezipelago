using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Game.Core.Blueprint;
using Game.Core.Research;
using MonoMod.RuntimeDetour;
using ShapezShifter.Kit;

namespace _2hapezipelago
{
    public class ResearchHandler : IDisposable
    {
        public APMod? Mod;
        public int OperatorLevel = 0;
        public List<Hook> DetourHooks;

        public ResearchHandler(APMod mod)
        {
            Mod = mod;
            DetourHooks = new List<Hook>
            {
                ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<GameSessionOrchestrator, ResearchManager.SerializedData, bool>(
                    (orch, researchData, isNewGame) => orch.Init_3_2_EssentialManagers(researchData, isNewGame),
                    RegisterEvents),
                ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<ResearchManager>(
                    resManager => resManager.Dispose(),
                    UnregisterEvents),
                ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<ResearchChunkLimitManager, ChunkLimitCurrency>(
                    manager => manager.ComputeCurrentChunkLimit(),
                    ExtendedComputeCurrentChunkLimit),
                ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<HUDCompass, InputDownstreamContext>(
                    (compass, ctx) => compass.OnUpdate(ctx),
                    ItemsProcessorHook)
            };
        }

        public void ItemsProcessorHook(HUDCompass compass, InputDownstreamContext ctx)
        {
            var toProcess = Mod?.ConHandler?.ItemsToProcess;
            if (toProcess == null) return;
            while (toProcess.Count > 0)
            {
                if (Mod?.ConHandler?.ReceivedItemsCount == null) continue;
                if (Mod?.ResHandler == null) continue;
                var item = toProcess.Dequeue();
                Mod?.ResHandler?.ReceiveReward(NameConverter.RemoteUpgrade(item));
                if (Mod?.SaveHandler == null) continue;
                Mod.SaveHandler.SaveData.ReceivedItemsCount = Mod.ConHandler.ReceivedItemsCount;
            }
        }

        public void RegisterEvents(GameSessionOrchestrator orch, ResearchManager.SerializedData researchData, bool isNewGame)
        {
            orch.Research.UnlockManager.OnResearchManuallyUnlockedByPlayer.Register(ResearchUnlocked);
            orch.Research.PlayerLevel.OnLevelChanged.Register(OperatorLevelChanged);
        }

        public void UnregisterEvents(ResearchManager resManager)
        {
            resManager.UnlockManager.OnResearchManuallyUnlockedByPlayer.TryUnregister(ResearchUnlocked);
            resManager.PlayerLevel.OnLevelChanged.TryUnregister(OperatorLevelChanged);
        }

        public void HudReceiveTimeoutFix<TObject>(Action<TObject> orig, TObject self)
        {
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - Mod?.ConHandler?.LastReceive >= 250)
            {
                orig(self);
            }
        }

        public ChunkLimitCurrency ExtendedComputeCurrentChunkLimit(ResearchChunkLimitManager self, ChunkLimitCurrency result)
        {
            var newResult = result + new ChunkLimitCurrency((Mod?.SaveHandler?.SaveData.RemotePlatforms ?? 0) * 2);
            Mod?.Logger.Info?.Log($"[Debug] Computed chunk limit: {newResult.Amount} ({result.Amount} without remote)");
            return newResult;
        }

        public void ResearchUnlocked(IResearchUpgrade upgrade)
        {
            Mod?.Logger.Info?.Log($"[Debug] Research unlocked: {upgrade.Id.Id}");
            if (Mod?.ConHandler?.Success == null) 
            {
                Mod?.Logger.Info?.Log("[Debug] Not connected");
                return;
            }
            var idParts = upgrade.Id.Id.Split('_');
            if (idParts[0].Equals("LocMilestone"))
            {
                Mod?.Logger.Info?.Log("[Debug] Milestone detected");
                int milestoneNum = Int32.Parse(idParts[1]);
                for (int i = 1; i <= upgrade.Rewards.Count; i++)
                {
                    Mod?.Logger.Info?.Log($"[Debug] Checking milestone {milestoneNum}-{i}");
                    Mod?.ConHandler?.CheckLocation(NameConverter.MilestoneLocation(milestoneNum, i));
                }
                if (Mod?.ConHandler?.SlotDatHand?.Goal == SlotDataHandler.Goaltype.Milestone &&
                    milestoneNum >= Mod?.ConHandler?.SlotDatHand?.MilestoneGoalNumber)
                {
                    Mod?.Logger.Info?.Log("[Debug] Milestone goal reached");
                    Mod?.ConHandler?.Session?.SetGoalAchieved();
                }
            }
            else if (idParts[0].Equals("LocTask"))
            {
                Mod?.Logger.Info?.Log("[Debug] Task detected");
                Mod?.ConHandler?.CheckLocation(NameConverter.TaskLocation(Int32.Parse(idParts[1])+1, Int32.Parse(idParts[2])+1));
            }
            else
            {
                Mod?.Logger.Info?.Log("[Debug] Neither milestone nor task detected");
            }
        }

        public void OperatorLevelChanged()
        {
            Mod?.Logger.Info?.Log($"[Debug] Operator level changed: {GameHelper.Core.Research.PlayerLevel.Level}");
            if (Mod?.ConHandler?.Success == null)
            {
                Mod?.Logger.Info?.Log("[Debug] Not connected");
                return;
            }
            var playerLevel = GameHelper.Core.Research.PlayerLevel;
            for (var i = OperatorLevel + 1; i <= playerLevel.Level; i++)
            {
                Mod?.Logger.Info?.Log($"[Debug] Checking operator level {i}");
                Mod?.ConHandler?.CheckLocation(NameConverter.OperatorLevelLocation(i));
            }
            OperatorLevel = playerLevel.Level;
            if (Mod?.ConHandler?.SlotDatHand?.Goal == SlotDataHandler.Goaltype.Operator &&
                playerLevel.Level >= Mod?.ConHandler?.SlotDatHand?.OperatorGoalLevel)
            {
                Mod?.Logger.Info?.Log("[Debug] Operator level goal reached");
                Mod?.ConHandler?.Session?.SetGoalAchieved();
            }
        }

        public void ResyncChecks()
        {
            Mod?.Logger.Info?.Log("[Debug] Resyncing operator levels");
            OperatorLevelChanged();
            foreach (var upgradeId in GameHelper.Core.Research.Progress._ManuallyUnlockedUpgrades)
            {
                var upgrade = GameHelper.Core.Research.Layout.GetUpgrade(upgradeId);
                ResearchUnlocked(upgrade);
            }
        }

        public void ReceiveReward(string remoteUpgradeId)
        {
            Mod?.Logger.Info?.Log($"[Debug] Received {remoteUpgradeId} at {Mod?.ConHandler?.LastReceive}");
            var resManager = GameHelper.Core.Research;
            if (remoteUpgradeId.StartsWith("rp"))
            {
                var before = resManager.PointStorage.Points.Amount;
                resManager.PointStorage.Add(new ResearchPointCurrency(int.Parse(remoteUpgradeId[2..]) / 100));
                Mod?.Logger.Info?.Log($"[Debug] Added research points: {before} -> {resManager.PointStorage.Points.Amount}");
            }
            else if (remoteUpgradeId.StartsWith("bp"))
            {
                var before = resManager.BlueprintCurrencyManager.BlueprintCurrency.FractionAmount;
                resManager.BlueprintCurrencyManager.AddBlueprintCurrency(new BlueprintCurrency(int.Parse(remoteUpgradeId[2..]), 0));
                Mod?.Logger.Info?.Log($"[Debug] Added blueprint points: {before} -> {resManager.BlueprintCurrencyManager.BlueprintCurrency.FractionAmount}");
            }
            else if (remoteUpgradeId.StartsWith("pl"))
            {
                if (Mod != null && Mod.SaveHandler != null)
                {
                    var before = Mod.SaveHandler.SaveData.RemotePlatforms;
                    Mod.SaveHandler.SaveData.RemotePlatforms += int.Parse(remoteUpgradeId[2..]) * 2;
                    Mod?.Logger.Info?.Log($"[Debug] Added platforms: {before} -> {Mod.SaveHandler.SaveData.RemotePlatforms}");
                    resManager.ChunkLimitManager.UpdateChunkLimit();
                }
            }
            else
            {
                var remoteUpgrade = resManager.Layout.GetUpgrade(new ResearchUpgradeId(remoteUpgradeId));
                resManager.UnlockManager.TryUnlock(remoteUpgrade, true);
                Mod?.Logger.Info?.Log($"[Debug] Research successfully unlocked");
            }
        }

        public void Dispose()
        {
            Mod = null;
            foreach (Hook detHook in DetourHooks)
            {
                detHook.Dispose();
            }
        }
    }
}
