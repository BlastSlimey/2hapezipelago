using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using MonoMod.RuntimeDetour;
using ShapezShifter.Flow;
using ShapezShifter.Hijack;
using ShapezShifter.Kit;
using ILogger = Core.Logging.ILogger;

namespace _2hapezipelago
{
    public class ConnectionHandler : IDisposable
    {
        //Fix for the current problem of ShapezShifter appending a bad prefix to debug commands.
        //Remove this once fixed.
        private class ModConsoleRewirerFix : IConsoleRewirer, IRewirer, IEquatable<IRewirer>, IDisposable
        {
            public readonly APMod Mod;
            public readonly List<Action<IDebugConsole>> ConsoleCommandsAddActions = new List<Action<IDebugConsole>>();
            public readonly RewirerHandle Handle;

            public ModConsoleRewirerFix(APMod mod)
            {
                Mod = mod;
                Handle = GameRewirers.AddRewirer(this);
            }

            public void AddCommand(Action<IDebugConsole> command)
            {
                ConsoleCommandsAddActions.Add(command);
            }

            public void RegisterCommands(IDebugConsole console)
            {
                ModConsoleCommandWrapperFix obj = new ModConsoleCommandWrapperFix(Mod, console);
                foreach (Action<IDebugConsole> consoleCommandsAddAction in ConsoleCommandsAddActions)
                {
                    consoleCommandsAddAction(obj);
                }
            }

            public void Dispose()
            {
                GameRewirers.RemoveRewirer(Handle);
            }
        }

        //Fix for the current problem of ShapezShifter appending a bad prefix to debug commands.
        //Remove this once fixed.
        private class ModConsoleCommandWrapperFix : IDebugConsole
        {
            public readonly IDebugConsole Console;
            public readonly string ModPrefix;

            public ModConsoleCommandWrapperFix(APMod mod, IDebugConsole console)
            {
                Console = console;
                ModPrefix = mod.GetType().Assembly.GetName().Name;
            }

            public List<string> GetAutoCompletions(string start)
            {
                return Console.GetAutoCompletions(start);
            }

            public void ParseAndExecute(string command, Action<string> output)
            {
                Console.ParseAndExecute(command, output);
            }

            public void Register(string id, Action<DebugConsole.CommandContext> handler, bool isCheat = false)
            {
                Console.Register(ModPrefix + "." + id, handler, isCheat);
            }

            public void Register(string id, DebugConsole.ConsoleOption option0, Action<DebugConsole.CommandContext> handler, bool isCheat = false)
            {
                Console.Register(ModPrefix + "." + id, option0, handler, isCheat);
            }

            public void Register(string id, DebugConsole.ConsoleOption option0, DebugConsole.ConsoleOption option1, Action<DebugConsole.CommandContext> handler, bool isCheat = false)
            {
                Console.Register(ModPrefix + "." + id, option0, option1, handler, isCheat);
            }
        }

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
            new ModConsoleRewirerFix(mod).AddCommand(con =>
            {
                con.Register("connect", new DebugConsole.StringOption("slotnameAddressPortPassword"), ctx =>
                {
                    if (CommandConnect(ctx))
                    {
                        var con2 = con is ModConsoleCommandsCreator.ModConsoleCommandWrapper wrapper ? wrapper.Console : con;
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
            Session = ArchipelagoSessionFactory.CreateSession(addressPort);
            LoginResult result;
            var SaveHandler = Mod?.SaveHandler ?? null;
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
                    var itemInfo = receivedItemsHelper.DequeueItem();
                    if (ReceivedItemsCount >= Mod?.SaveHandler?.SaveData.ReceivedItemsCount)
                    {
                        Mod?.ResHandler?.ReceiveReward(NameConverter.RemoteUpgrade(itemInfo.ItemName));
                    }
                    ReceivedItemsCount++;
                    if (SaveHandler != null)
                    {
                        SaveHandler.SaveData.ReceivedItemsCount = ReceivedItemsCount;
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
                ReceivedItemsCount = 0;
                SlotDatHand = new SlotDataHandler(Success.SlotData, Mod, ctx);
                Mod?.ResHandler?.ResyncChecks();
            }
        }

        public void Disconnect()
        {
            Session?.Socket.DisconnectAsync().Start();
            Session = null;
            Success = null;
        }

        public void Dispose()
        {
            Mod = null;
            Disconnect();
            DisconnectOnDisposeHook.Dispose();
        }
    }
}
