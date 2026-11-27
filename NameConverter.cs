
using System;
using System.Collections.Generic;

namespace _2hapezipelago
{
    public static class NameConverter
    {
        public static Dictionary<string, string> RemoteUpgradeByItem = new Dictionary<string, string>()
        {
            ["Painter"] = "RemotePainter",
            ["Color Mixer"] = "RemoteColorMixer",
            ["Crystal Generator"] = "RemoteCrystalGenerator",
            ["Trash"] = "RemoteTrash",
            ["Label"] = "RemoteLabel",
            ["Overflow Splitter"] = "RemoteOverflowSplitter",
            ["Wires"] = "RemoteWires",
            ["Signal Producer"] = "RemoteSignalProducer",
            ["Belt Filter"] = "RemoteBeltFilter",
            ["Belt Reader"] = "RemoteBeltReader",
            ["Pipe Gate"] = "RemotePipeGate",
            ["Display"] = "RemoteDisplay",
            ["Button"] = "RemoteButton",
            ["Logic Gates"] = "RemoteLogicGates",
            ["Simulated Machines"] = "RemoteSimulatedMachines",
            ["Global Signal Transmitter"] = "RemoteGlobalSignalTransmission",
            ["Operator Signal Receiver"] = "RemoteOperatorSignalReceiver",
            ["Fluid Tank"] = "RemoteFluidTank",
            ["Stacker"] = "RemoteStacker",
            ["Stacker (Bent)"] = "RemoteStackerBent",
            ["Rotator (CW)"] = "RemoteRotatorCW",
            ["Rotator (CCW)"] = "RemoteRotatorCCW",
            ["Rotator (180)"] = "RemoteRotator180",
            ["Conveyor Belt"] = "RemoteConveyorBelt",
            ["Mini-Miner"] = "RemoteMiniMiner",
            ["Half Destroyer"] = "RemoteHalfDestroyer",
            ["Pin Pusher"] = "RemotePinPusher",
            ["Cutter"] = "RemoteCutter",
            ["Swapper"] = "RemoteSwapper",
            ["Sandbox Item Producer"] = "RemoteSandboxItemProducer",
            ["Sandbox Fluid Producer"] = "RemoteSandboxFluidProducer",
            ["Mini Pump"] = "RemoteMiniPump",
            ["Pipe"] = "RemotePipe",
            ["Mini Pump + Pipe"] = "RemoteMiniPumpPipe",
            ["Shape Trains"] = "RemoteShapeTrains",
            ["Fluid Trains"] = "RemoteFluidTrains",
            ["1x3 Platform"] = "Remote1x3Platform",
            ["1x4 Platform"] = "Remote1x4Platform",
            ["3-Blocks L Platform"] = "Remote3BlocksLPlatform",
            ["4-Blocks L Platform"] = "Remote4BlocksLPlatform",
            ["T Platform"] = "RemoteTPlatform",
            ["S Platform"] = "RemoteSPlatform",
            ["Cross Platform"] = "RemoteCrossPlatform",
            ["2x2 Platform"] = "Remote2x2Platform",
            ["2x3 Platform"] = "Remote2x3Platform",
            ["2x4 Platform"] = "Remote2x4Platform",
            ["3x3 Platform"] = "Remote3x3Platform",
            ["Train Color: Green"] = "RemoteGreenTrains",
            ["Train Color: Blue"] = "RemoteBlueTrains",
            ["Train Color: Cyan"] = "RemoteCyanTrains",
            ["Train Color: Magenta"] = "RemoteMagentaTrains",
            ["Train Color: Yellow"] = "RemoteYellowTrains",
            ["Train Color: White"] = "RemoteWhiteTrains",
            ["Wagon Transfer"] = "RemoteWagonTransfer",
            ["Spacer Wagon"] = "RemoteSpacerWagon",
            ["Space Belt"] = "RemoteSpaceBelt",
            ["1x1 Platform"] = "Remote1x1Platform",
            ["1x2 Platform"] = "Remote1x2Platform",
            ["Asteroid Miner"] = "RemoteAsteroidMiner",
            ["Asteroid Pump"] = "RemoteAsteroidPump",
            ["Asteroid Miner Booster"] = "RemoteAsteroidMinerBooster",
            ["Asteroid Pump Booster"] = "RemoteAsteroidPumpBooster",
            ["Asteroid Miner + Booster"] = "RemoteAsteroidMinerPlusBooster",
            ["Asteroid Pump + Booster"] = "RemoteAsteroidPumpPlusBooster",
            ["Space Pipe"] = "RemoteSpacePipe",
            ["Asteroid Pump + Pipe"] = "RemoteAsteroidPumpPlusPipe",
            ["Asteroid Pump + Booster + Pipe"] = "RemoteAsteroidPumpPlusBoosterPipe",
            ["2nd Machine Level"] = "Remote2ndMachineLevel",
            ["Blueprints"] = "RemoteBlueprints",
            ["3rd Space Level"] = "Remote3rdSpaceLevel",
            ["3rd Machine Level"] = "Remote3rdMachineLevel",
            ["Train Vortex Delivery"] = "RemoteTrainVortexDelivery",
            ["Space Platforms"] = "RemoteSpacePlatforms",
            ["2nd Space Level"] = "Remote2ndSpaceLevel",
            ["Fluids"] = "RemoteFluids",
            ["Shop"] = "RemoteShop",
            ["Operator Levels"] = "RemoteOperatorLevels",
        };

        public static string MilestoneLocation(int milestoneNum, int itemNum)
        {
            return $"Milestone {milestoneNum} reward #{itemNum}";
        }

        public static string TaskLocation(int lineNum, int taskNum)
        {
            return $"Job #{lineNum}-{taskNum}";
        }

        public static string OperatorLevelLocation(int level)
        {
            return $"Operator level {level}";
        }

        public static string RemoteUpgrade(string itemName)
        {
            if (itemName.StartsWith("Job line #"))
            {
                return $"RemoteJobLine{itemName["Task line #".Length..]}";
            }
            else if (itemName.StartsWith("Operator line #"))
            {
                return $"RemoteOperatorLine{itemName["Operator line #".Length..]}";
            }
            else if (itemName.EndsWith("Research Credits"))
            {
                return "rp" + itemName.Split()[0];
            }
            else if (itemName.EndsWith("Blueprint Credits"))
            {
                return "bp" + itemName.Split()[0];
            }
            else if (itemName.EndsWith("Platform Capacity"))
            {
                return "pl" + itemName.Split()[0];
            }
            if (!RemoteUpgradeByItem.TryGetValue(itemName, out var remoteUpgrade))
            {
                throw new Exception($"Uknown item \"{itemName}\"");
            }
            return remoteUpgrade;
        }
    }
}
