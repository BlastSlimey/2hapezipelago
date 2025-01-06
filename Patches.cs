
using System.Collections.Generic;
using System.Linq;
using _2hapezipelago;
using HarmonyLib;
using Newtonsoft.Json.Linq;

[HarmonyPatch(typeof(GameCore), "PreInit")]
public class GameCorePatch {

    private static void Postfix(GameCore __instance) {
        if (Plugin.gamecore == null) {
            Plugin.Logger.LogInfo("Obtained GameCore instance");
        }
        Plugin.gamecore = __instance;
    }

}

[HarmonyPatch(typeof(ShapesDeliveryTracker), "DeliverShape")]
public class ShapesDeliveryPatch {

    private static readonly string[] CheckedLocations = [];

    private static void Postfix(ShapesDeliveryTracker __instance, ShapeId itemId) {
        if (Plugin.InValidSavegame()) {
            string delivered = ShapeIdManager.IdToHash(itemId);
            Plugin.Logger.LogInfo($"Delivered {delivered}");
            if (!CheckedLocations.Contains(delivered)) {
                Plugin.Logger.LogInfo("Not in checked list");
                if (((JArray)ConnectionHandler.Success.SlotData["shapes"]).Any((JToken token) => {
                    return token.ToString().Equals(delivered);
                })) {
                    Plugin.Logger.LogInfo("But in slot data");
                    if (ConnectionHandler.HasItem(delivered)) {
                        Plugin.Logger.LogInfo("Has item, so it shall be checked");
                        ConnectionHandler.CheckLocation(delivered);
                        CheckedLocations.Append(delivered);
                    }
                }
            }
        }
    }

}
