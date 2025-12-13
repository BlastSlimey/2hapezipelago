using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
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
        public string? PlayerName;
        public string? AddressPort;
        public string? Password;
        public int ReceivedItemsCount = 0;
        public int LoadedReceivedItemsCount = 0;

        public ConnectionHandler(APMod mod)
        {
            Mod = mod;
            Logger = Mod.Logger;
            Mod.RegisterConsoleCommand("ap.connect", CommandConnect, isCheat: true, arg1: new DebugConsole.StringOption("slotnameAddressPortPassword"));
            Mod.RegisterConsoleCommand("ap.reconnect", Connect, isCheat: true);
            Mod.RegisterConsoleCommand("ap.help", CommandHelp);
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
            ctx.Output("If you connect this savegame for the first time, type 'ap.connect player@address:port' " +
                "or 'ap.connect player:password@address:port' if your multiworld has a password. " +
                "Your connection details will then be saved to the savegame, so that you can use 'ap.reconnect' every time after the first time. " +
                "To disconnect, just return to the main menu.");
        }

        public void CheckLocation(string locationName)
        {
            if (Success == null) return;
            Session?.Locations.CompleteLocationChecks(Session.Locations.GetLocationIdFromName("shapez 2", locationName));
        }

        public void Connect(DebugConsole.CommandContext ctx)
        {
            if (PlayerName == null || AddressPort == null || Password == null)
            {
                Logger.Warning?.Log("Trying to reconnect but no connection details found");
                ctx.Output("Trying to reconnect but no connection details found");
                return;
            }
            Connect(PlayerName, AddressPort, Password, ctx);
        }

        public void Connect(string playername, string addressPort, string password, DebugConsole.CommandContext ctx)
        {
            Session = ArchipelagoSessionFactory.CreateSession(addressPort);
            LoginResult result;
            PlayerName = playername;
            AddressPort = addressPort;
            Password = password;

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
                Session.Items.ItemReceived += (ReceivedItemsHelper receivedItemsHelper) => 
                {
                    try
                    {
                        var itemInfo = receivedItemsHelper.DequeueItem();
                        if (ReceivedItemsCount >= LoadedReceivedItemsCount)
                        {
                            Mod?.ResHandler?.ReceiveReward(NameConverter.RemoteUpgrade(itemInfo.ItemName));
                        }
                        ReceivedItemsCount++;
                        LoadedReceivedItemsCount = ReceivedItemsCount;
                    }
                    catch (Exception e)
                    {
                        Logger.Warning?.Log("Receiving item failed: " + e.Message);
                        ctx.Output("Receiving item failed: " + e.Message);
                    }
                };
                Session.MessageLog.OnMessageReceived += (LogMessage message) => 
                {
                    Logger.Info?.Log(message.ToString());
                    ctx.Output(message.ToString());
                };
                Session.Socket.SocketClosed += (string reason) =>
                {
                    Success = null;
                    Logger.Info?.Log(reason);
                    ctx.Output(reason);
                };
            }
        }

        public void Dispose()
        {
            Mod = null;
            Session?.Socket.DisconnectAsync().Start();
        }
    }
}
