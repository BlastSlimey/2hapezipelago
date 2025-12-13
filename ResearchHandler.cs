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
            UnregisterHook = ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<ResearchManager>(
                (ResearchManager resManager) => resManager.Dispose(),
                UnregisterEvents);
            RegisterHook = ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<GameSessionOrchestrator, ResearchManager.SerializedData, bool>(
                (orch, researchData, isNewGame) => orch.Init_3_2_EssentialManagers(researchData, isNewGame),
                RegisterEvents);
        }

        public void RegisterEvents(GameSessionOrchestrator orch, ResearchManager.SerializedData researchData, bool isNewGame)
        {
            orch.Research.UnlockManager.OnResearchUnlockedByPlayer.Register(ResearchUnlocked);
            orch.Research.PlayerLevel.OnLevelChanged.Register(OperatorLevelChanged);
        }

        public void UnregisterEvents(ResearchManager resManager)
        {
            resManager.UnlockManager.OnResearchUnlockedByPlayer.TryUnregister(ResearchUnlocked);
            resManager.PlayerLevel.OnLevelChanged.TryUnregister(OperatorLevelChanged);
        }

        public void ResearchUnlocked(IResearchUpgrade upgrade)
        {
            var idParts = upgrade.Id.Id.Split('_');
            if (idParts[0].Equals("LocMilestone"))
            {
                int milestoneNum = Int32.Parse(idParts[1]);
                for (int i = 0; i < upgrade.Rewards.Count; i++)
                {
                    Mod?.ConHandler?.CheckLocation(NameConverter.MilestoneLocation(milestoneNum, i));
                }
            }
            else if (idParts[0].Equals("LocTask"))
            {
                Mod?.ConHandler?.CheckLocation(NameConverter.TaskLocation(Int32.Parse(idParts[1]), Int32.Parse(idParts[2])));
            }
        }

        public void OperatorLevelChanged()
        {
            var playerLevel = GameHelper.Core.Research.PlayerLevel;
            for (var i = OperatorLevel + 1; i <= playerLevel.Level; i++)
            {
                Mod?.ConHandler?.CheckLocation(NameConverter.OperatorLevelLocation(i));
            }
            OperatorLevel = playerLevel.Level;
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
                    Amount = Int32.Parse(remoteUpgradeId.Substring(2))
                };
                resManager.RewardManager.GrantReward(ResearchRewardFactory.Create(serializedReward));
            }
            else if (remoteUpgradeId.StartsWith("pl"))
            {
                var serializedReward = new SerializedResearchRewardChunkLimit()
                {
                    Amount = Int32.Parse(remoteUpgradeId.Substring(2))
                };
                resManager.RewardManager.GrantReward(ResearchRewardFactory.Create(serializedReward));
            }
            var remoteUpgrade = resManager.Layout.GetUpgrade(new ResearchUpgradeId(remoteUpgradeId));
            resManager.UnlockManager.TryUnlock(remoteUpgrade, true);
        }

        public void Dispose()
        {
            Mod = null;
            RegisterHook.Dispose();
            UnregisterHook.Dispose();
        }
    }
}
