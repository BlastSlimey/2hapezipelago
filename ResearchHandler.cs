using System;
using Game.Core.Research;
using MonoMod.RuntimeDetour;
using ShapezShifter.Kit;

namespace _2hapezipelago
{
    public class ResearchHandler : IDisposable
    {
        public APMod? Mod;
        public int OperatorLevel = 0;
        public Hook RegisterHook, UnregisterHook;

        public ResearchHandler(APMod mod)
        {
            Mod = mod;
            RegisterHook = ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<GameSessionOrchestrator, ResearchManager.SerializedData, bool>(
                (orch, researchData, isNewGame) => orch.Init_3_2_EssentialManagers(researchData, isNewGame),
                RegisterEvents);
            UnregisterHook = ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<ResearchManager>(
                resManager => resManager.Dispose(),
                UnregisterEvents);
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

        [Obsolete]
        public void ReceiveReward(ISerializedResearchReward[] serializedRewards)
        {
            var rewardManager = GameHelper.Core.Research.RewardManager;
            foreach (var serializedRweard in serializedRewards)
            {
                var reward = ResearchRewardFactory.Create(serializedRweard);
                rewardManager.GrantReward(reward);
            }
        }

        public void ReceiveReward(string remoteUpgradeId)
        {
            var resManager = GameHelper.Core.Research;
            if (remoteUpgradeId.StartsWith("rp"))
            {
                var serializedReward = new SerializedResearchRewardResearchPoints()
                {
                    Amount = int.Parse(remoteUpgradeId[2..])
                };
                resManager.RewardManager.GrantReward(ResearchRewardFactory.Create(serializedReward));
            }
            else if (remoteUpgradeId.StartsWith("pl"))
            {
                var serializedReward = new SerializedResearchRewardChunkLimit()
                {
                    Amount = int.Parse(remoteUpgradeId[2..])
                };
                resManager.RewardManager.GrantReward(ResearchRewardFactory.Create(serializedReward));
            }
            else
            {
                var remoteUpgrade = resManager.Layout.GetUpgrade(new ResearchUpgradeId(remoteUpgradeId));
                resManager.UnlockManager.TryUnlock(remoteUpgrade, true);
            }
        }

        public void Dispose()
        {
            Mod = null;
            RegisterHook.Dispose();
            UnregisterHook.Dispose();
        }
    }
}
