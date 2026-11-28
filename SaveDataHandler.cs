using System;
using Core.Factory;
using ShapezShifter.Flow;
using ShapezShifter.Hijack;

namespace _2hapezipelago
{
    [Serializable]
    public class APModData
    {
        public int ReceivedItemsCount { get; set; }
        public string PlayerName { get; set; }
        public string AddressPort { get; set; }
        public string Password { get; set; }
        public int RemotePlatforms { get; set; }
    }

    public class SaveDataHandler : IDisposable
    {
        public APMod? Mod;
        public APModData SaveData;

        public SaveDataHandler(APMod mod)
        {
            Mod = mod;
            SaveData = new APModData()
            {
                ReceivedItemsCount = 0,
                PlayerName = "",
                AddressPort = "",
                Password = "",
                RemotePlatforms = 0
            };
            Mod.AttachSaveData(delegate ()
            {
                return new APModData()
                {
                    ReceivedItemsCount = 0,
                    PlayerName = "",
                    AddressPort = "",
                    Password = "",
                    RemotePlatforms = 0
                };
            });
            Mod.RegisterToAfterSaveDataDeserialized<APModData>(SetSaveData);
        }

        public void SetSaveData(APModData data)
        {
            SaveData = data;
        }

        public void Dispose()
        {
            Mod?.UnregisterToAfterSaveDataDeserialized<APModData>(SetSaveData);
            Mod = null;
        }
    }
}
