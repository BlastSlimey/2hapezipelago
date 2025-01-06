
using System;
using System.Linq;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using BepInEx.Logging;

public class ConnectionHandler {

    public static ArchipelagoSession Session;
    public static LoginSuccessful Success;

    public static void Connect(ManualLogSource Logger) {

        string[] list = ConfigHandler.ConnectionsList.Value.Split(";;;");
        int active = ConfigHandler.ActiveSlot.Value;
        if (active < -1) {
            Logger.LogWarning($"Illegal ActiveSlot value: {active}");
            return;
        } else if (active >= list.Length) {
            Logger.LogWarning($"ActiveSlot value out of range: {active}");
            return;
        } else if (active == -1) {
            Logger.LogInfo("Connecting disabled as per config");
            return;
        }
        string[] activeList = list[active].Split(",,,");
        if (activeList.Length < 2) {
            Logger.LogWarning($"Chosen slot contains not enough information");
            return;
        } else if (activeList.Length == 2) {
            activeList.Append("");
        }

        Session = ArchipelagoSessionFactory.CreateSession(activeList[1]);
        LoginResult result;
        try {
            result = Session.TryConnectAndLogin(
                "shapez 2", activeList[0], ItemsHandlingFlags.AllItems, password: activeList[2]
            );
        } catch (Exception e) {
            result = new LoginFailure(e.GetBaseException().Message);
        }

        if (!result.Successful) {
            LoginFailure failure = (LoginFailure)result;
            string errorMessage = $"Failed to Connect to {activeList[1]} as {activeList[0]}:";
            foreach (string error in failure.Errors) {
                errorMessage += $"\n    {error}";
            }
            foreach (ConnectionRefusedError error in failure.ErrorCodes) {
                errorMessage += $"\n    {error}";
            }
        } else {
            Success = (LoginSuccessful)result;
            Logger.LogInfo("Connection successful");
            CheckLocation("Connect");
        }

    }

    public static bool HasItem(string name) {
        foreach (ItemInfo item in Session.Items.AllItemsReceived) {
            if (item.ItemName.Equals(name)) {
                return true;
            }
        }
        return false;
    }

    public static void CheckLocation(string name) {
        long id = ConnectionHandler.Session.Locations.GetLocationIdFromName("shapez 2", name);
        ConnectionHandler.Session.Locations.CompleteLocationChecks([id]);
    }

}
