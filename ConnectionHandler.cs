using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using MonoMod.RuntimeDetour;
using ShapezShifter.Flow;
using ILogger = Core.Logging.ILogger;

namespace _2hapezipelago
{
    public class ConnectionHandler : IDisposable
    {
        public APMod? Mod;
        public ILogger Logger;
        public ArchipelagoSession? Session;
        public LoginSuccessful? Success;
        public int ReceivedItemsCount = 0;
        public Hook DisconnectOnDisposeHook;
        public SlotDataHandler? SlotDatHand;

        public ConnectionHandler(APMod mod)
        {
            Mod = mod;
            Logger = Mod.Logger;
            Mod.RegisterConsoleCommand("ap.connect", CommandConnect, arg1: new DebugConsole.StringOption("slotnameAddressPortPassword"), useAssemblyPrefix: false);
            Mod.RegisterConsoleCommand("ap.reconnect", Connect, useAssemblyPrefix: false);
            Mod.RegisterConsoleCommand("ap.help", CommandHelp, useAssemblyPrefix: false);
            Mod.RegisterConsoleCommand("ap.disconnect", ctx => { Disconnect(); }, useAssemblyPrefix: false);
            DisconnectOnDisposeHook = ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<GameSessionOrchestrator>(
                orch => orch.Dispose(),
                orch => { Disconnect(); });
        }

        private void CommandConnect(DebugConsole.CommandContext ctx)
        {
            if (ctx.Options.Length != 1)
            {
                CommandHelp(ctx);
                return;
            }
            var input = ctx.GetString(0);
            var playerPassword = input[..input.IndexOf('@')];
            string password, player;
            if (playerPassword.Contains(':'))
            {
                player = playerPassword[..playerPassword.IndexOf(':')];
                password = playerPassword[(playerPassword.IndexOf(':') + 1)..];
            }
            else
            {
                player = playerPassword;
                password = "";
            }
            var addressPort = input[(input.IndexOf('@') + 1)..];
            this.Connect(player, addressPort, password, ctx);
        }

        private void CommandHelp(DebugConsole.CommandContext ctx)
        {
            ctx.Output(
                "If you connect this savegame for the first time, type 'ap.connect player@address:port'" +
                "\nor 'ap.connect player:password@address:port' if your multiworld has a password. " +
                "\nYour connection details will then be saved to the savegame, so that you can use " +
                "\n'ap.reconnect' every time after the first time. To disconnect, just return to the main menu.");
        }

        public void CheckLocation(string locationName)
        {
            if (Success == null) return;
            Session?.Locations.CompleteLocationChecks(Session.Locations.GetLocationIdFromName("shapez 2", locationName));
        }

        public void Connect(DebugConsole.CommandContext ctx)
        {
            var PlayerName = Mod?.SaveHandler?.SaveData.Data.PlayerName ?? "";
            var AddressPort = Mod?.SaveHandler?.SaveData.Data.AddressPort ?? "";
            if (PlayerName == "" || AddressPort == "")
            {
                Logger.Warning?.Log("Trying to reconnect but no connection details found or incomplete");
                ctx.Output("Trying to reconnect but no connection details found or incomplete");
                return;
            }
            Connect(PlayerName, AddressPort, Mod?.SaveHandler?.SaveData.Data.Password ?? "", ctx);
        }

        public void Connect(string playername, string addressPort, string password, DebugConsole.CommandContext ctx)
        {
            Session = ArchipelagoSessionFactory.CreateSession(addressPort);
            LoginResult result;
            var SaveHandler = Mod?.SaveHandler ?? null;
            if (SaveHandler != null)
            {
                SaveHandler.SaveData.Data.PlayerName = playername;
                SaveHandler.SaveData.Data.AddressPort = addressPort;
                SaveHandler.SaveData.Data.Password = password;
            }

            try
            {
                result = Session.TryConnectAndLogin(
                    "shapez 2", playername, ItemsHandlingFlags.RemoteItems, password: password
                );
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {addressPort} as {playername}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }
                Logger.Warning?.Log(errorMessage);
                ctx.Output(errorMessage);
            }
            else
            {
                Success = (LoginSuccessful)result;
                Logger.Info?.Log("Connection successful");
                ctx.Output("Connection successful");
                ReceivedItemsCount = 0;
                SlotDatHand = new SlotDataHandler(Success.SlotData);
                Session.Items.ItemReceived += receivedItemsHelper => 
                {
                    try
                    {
                        var itemInfo = receivedItemsHelper.DequeueItem();
                        if (ReceivedItemsCount >= Mod?.SaveHandler?.SaveData.Data.ReceivedItemsCount)
                        {
                            Mod?.ResHandler?.ReceiveReward(NameConverter.RemoteUpgrade(itemInfo.ItemName));
                        }
                        ReceivedItemsCount++;
                        if (SaveHandler != null)
                        {
                            SaveHandler.SaveData.Data.ReceivedItemsCount = ReceivedItemsCount;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Warning?.Log("Receiving item failed: " + e.Message);
                        ctx.Output("Receiving item failed: \n" + e.Message);
                    }
                };
                Session.MessageLog.OnMessageReceived += message => 
                {
                    Logger.Info?.Log(message.ToString());
                    ctx.Output(message.ToString());
                };
                Session.Socket.SocketClosed += reason =>
                {
                    Success = null;
                    Logger.Info?.Log(reason);
                    ctx.Output(reason);
                };
                Mod?.ResHandler?.ResyncChecks();
            }
        }

        public void Disconnect()
        {
            Session?.Socket.DisconnectAsync().Start();
        }

        public void Dispose()
        {
            Mod = null;
            Disconnect();
            DisconnectOnDisposeHook.Dispose();
        }
    }
}
