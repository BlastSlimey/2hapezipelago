using System;
using System.Collections.Generic;
using System.Text;
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
                Password = ""
            };
            AttachSaveDataFix(delegate ()
            {
                return new APModData()
                {
                    ReceivedItemsCount = 0,
                    PlayerName = "",
                    AddressPort = "",
                    Password = ""
                };
            });
            Mod.RegisterToAfterSaveDataDeserialized<APModData>(SetSaveData);
        }

        //Fix for the current problem of ShapezShifter storing the rewirer with a bad key in the dictionary.
        //Remove this once fixed (and replace the "AttachSaveDataFix" with "Mod.AttachSaveData").
        public void AttachSaveDataFix<T>(Func<T> defaultDataFactory) where T : new()
        {
            string text = Mod.ResolveId<T>();
            string fileName = text + ".json";
            ModSaveDataRewirer<T> modSaveDataRewirer = new ModSaveDataRewirer<T>(fileName, new LambdaFactory<T>(defaultDataFactory), Mod?.Logger);
            RewirerHandle handle = GameRewirers.AddRewirer(modSaveDataRewirer);
            ModSaveDataExtensions.ActiveRewirer value = new ModSaveDataExtensions.ActiveRewirer(handle, modSaveDataRewirer);
            ModSaveDataExtensions.Rewirers.Add(text, value);
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
