
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace _2hapezipelago;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("shapez 2.exe")]
public class Plugin : BaseUnityPlugin {

    public static new ManualLogSource Logger;
    public static Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
    public static GameCore gamecore;
        
    private void Awake() {

        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Logger.LogInfo("Initializing config...");
        ConfigHandler.InitConfig(Config);

        Logger.LogInfo("Applying Harmony patches...");
        harmony.PatchAll();

        Logger.LogInfo("Attempting connection...");
        ConnectionHandler.Connect(Logger);

    }

    public static bool InValidSavegame() {
        if (gamecore != null) {
            SavegameOptionsManager SOM = (SavegameOptionsManager)Util.getPrivateProperty(
                typeof(GameCore), gamecore, "SavegameOptionsManager"
            );
            if (SOM != null) {
                return !SOM.Options.MenuMode;
            }
        }
        return false;
    }

}
