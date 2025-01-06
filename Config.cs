
using BepInEx.Configuration;

public struct ConnectionInfo {

    public string slot;
    public string address;
    public int port;
    public string password;

    public ConnectionInfo(string sl, string ad, int po, string pa) {
        slot = sl;
        address = ad;
        port = po;
        password = pa;
    }

}

public class ConfigHandler {

    public static ConfigEntry<string> ConnectionsList;
    public static ConfigEntry<int> ActiveSlot;

    public static void InitConfig(ConfigFile Config) {
        ConnectionInfo[] Def = [
            new ConnectionInfo("Player1", "archipelago.gg", 38281, "password"),
            new ConnectionInfo("AnotherSlot", "localhost", 38281, "")
        ];
        ConnectionsList = Config.Bind(
            "General", "ConnectionList", 
            "SlotName,,,archipelago.gg:38281,,,password;;;Player1,,,localhost:38281,,,;;;AnotherPlayer,,,your.APServer.com,,,password1234", 
            "A list of connection details, so you don't have to re-enter them every time you play another slot. " + 
            "Every entry consists of the slot name, server address:port, and password. " + 
            "See examples for how to correctly format your entries."
        );
        ActiveSlot = Config.Bind(
            "General", "ActiveSlot", -1, 
            "The ConnectionList entry to use for connecting to a sever. Begins with 0 as the first entry. Use -1 to disable connecting."
        );
    }

}
