using System;
using System.Collections.Generic;
using System.Text;
using ShapezShifter.Flow;

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
        public ModSaveData<APModData> SaveData;

        public SaveDataHandler(APMod mod)
        {
            Mod = mod;
            SaveData = Mod.CreateSaveData<APModData>();
            SaveData.OnDataLoaded += data =>
            {

            };
        }

        public void Dispose()
        {
            Mod = null;
        }
    }
}
