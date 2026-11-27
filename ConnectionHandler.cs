using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using MonoMod.RuntimeDetour;
using static ShapezShifter.Flow.ModConsoleCommandsCreator;
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
        public long LastReceive = 0;
        public Queue<string> ItemsToProcess = new Queue<string>();

        public ConnectionHandler(APMod mod)
        {
            Mod = mod;
            Logger = Mod.Logger;
            new ModConsoleRewirer(mod).AddCommand(con =>
            {
                con.Register("connect", new DebugConsole.StringOption("slotnameAddressPortPassword"), ctx =>
                {
                    if (CommandConnect(ctx))
                    {
                        var con2 = con is ModConsoleCommandWrapper wrapper ? wrapper.Console : con;
                        if (con2 is DebugConsole deb)
                        {
                            deb.Savegame.CheatsEnabled = true;
                        }
                    }
                });
                con.Register("reconnect", Connect);
                con.Register("help", CommandHelp);
                con.Register("disconnect", ctx => { Disconnect(); });
            });
            DisconnectOnDisposeHook = ShapezShifter.SharpDetour.DetourHelper.CreatePostfixHook<GameSessionOrchestrator>(
                orch => orch.Dispose(),
                orch => { Disconnect(); });
        }

        private bool CommandConnect(DebugConsole.CommandContext ctx)
        {
            if (ctx.Options.Length != 1)
            {
                CommandHelp(ctx);
                return false;
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
            return true;
        }

        private void CommandHelp(DebugConsole.CommandContext ctx)
        {
            ctx.Output(
                "If you connect this savegame for the first time, type '2hapezipelago.connect player@address:port'" +
                "\nor '2hapezipelago.connect player:password@address:port' if your multiworld has a password. " +
                "\nYour connection details will then be saved to the savegame, so that you can use " +
                "\n'2hapezipelago.reconnect' every time after the first time. To disconnect, just return to the main menu.");
        }

        public void CheckLocation(string locationName)
        {
            if (Success == null) return;
            Session?.Locations.CompleteLocationChecks(Session.Locations.GetLocationIdFromName("shapez 2", locationName));
        }

        public void Connect(DebugConsole.CommandContext ctx)
        {
            var PlayerName = Mod?.SaveHandler?.SaveData.PlayerName ?? "";
            var AddressPort = Mod?.SaveHandler?.SaveData.AddressPort ?? "";
            if (PlayerName == "" || AddressPort == "")
            {
                Logger.Warning?.Log("Trying to reconnect but no connection details found or incomplete");
                ctx.Output("Trying to reconnect but no connection details found or incomplete");
                return;
            }
            Connect(PlayerName, AddressPort, Mod?.SaveHandler?.SaveData.Password ?? "", ctx);
        }

        public void Connect(string playername, string addressPort, string password, DebugConsole.CommandContext ctx)
        {
            if (Session != null)
            {
                Disconnect();
            }
            Session = ArchipelagoSessionFactory.CreateSession(addressPort);
            LoginResult result;
            var SaveHandler = Mod?.SaveHandler;
            if (SaveHandler != null)
            {
                SaveHandler.SaveData.PlayerName = playername;
                SaveHandler.SaveData.AddressPort = addressPort;
                SaveHandler.SaveData.Password = password;
            }
            Session.Items.ItemReceived += receivedItemsHelper =>
            {
                try
                {
                    LastReceive = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    var itemInfo = receivedItemsHelper.DequeueItem();
                    ReceivedItemsCount++;
                    Logger.Info?.Log($"[Debug] Receiving item #{ReceivedItemsCount} ({Mod?.SaveHandler?.SaveData.ReceivedItemsCount} in save data)");
                    if (ReceivedItemsCount > Mod?.SaveHandler?.SaveData.ReceivedItemsCount)
                    {
                        ItemsToProcess.Enqueue(itemInfo.ItemName);
                        // Mod?.ResHandler?.ReceiveReward(NameConverter.RemoteUpgrade(itemInfo.ItemName));
                        // if (SaveHandler != null)
                        // {
                        //     SaveHandler.SaveData.ReceivedItemsCount = ReceivedItemsCount;
                        // }
                    }
                }
                catch (Exception e)
                {
                    Logger.Warning?.Log("Receiving item failed: " + e.Message);
                    // ctx.Output("Receiving item failed: \n" + e.Message);
                }
            };
            Session.MessageLog.OnMessageReceived += message =>
            {
                Logger.Info?.Log(message.ToString());
                // ctx.Output(message.ToString());
            };
            Session.Socket.SocketClosed += reason =>
            {
                Success = null;
                Logger.Info?.Log(reason);
                // ctx.Output(reason);
            };

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
                SlotDatHand = new SlotDataHandler(Success.SlotData, Mod, ctx);
                ItemsToProcess.Clear();
                Mod?.ResHandler?.ResyncChecks();
            }
        }

        public void Disconnect()
        {
            Session?.Socket.DisconnectAsync();
            Session = null;
            Success = null;
            ReceivedItemsCount = 0;
            ItemsToProcess.Clear();
        }

        public void Dispose()
        {
            Mod = null;
            Disconnect();
            DisconnectOnDisposeHook.Dispose();
        }
    }
}
