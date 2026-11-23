
using System;
using System.Collections.Generic;

namespace _2hapezipelago
{
    public static class NameConverter
    {
        public static Dictionary<string, string> RemoteUpgradeByItem = new Dictionary<string, string>()
        {
            ["Conveyor Belt"] = "RemoteConveyorBelt",
            ["Extractor"] = "RemoteExtractor",
            ["Half Destroyer"] = "RemoteHalfDestroyer",
            ["Rotator (CW)"] = "RemoteRotatorCW",
            ["Stacker"] = "RemoteStacker",
            ["Painter"] = "RemotePainter",
            ["Pump"] = "RemotePump",
            ["Pipe"] = "RemotePipe",
            ["Pin Pusher"] = "RemotePinPusher",
            ["Color Mixer"] = "RemoteColorMixer",
            ["Crystal Generator"] = "RemoteCrystalGenerator",
            ["Cutter"] = "RemoteCutter",
            ["Swapper"] = "RemoteSwapper",
            ["Trash"] = "RemoteTrash",
            ["Rotator (CCW)"] = "RemoteRotatorCCW",
            ["Rotator (180)"] = "RemoteRotator180",
            ["Stacker (Bent)"] = "RemoteStackerBent",
            ["Label"] = "RemoteLabel",
            ["Overflow Splitter"] = "RemoteOverflowSplitter",
            ["Sandbox Item Producer"] = "RemoteSandboxItemProducer",
            ["Sandbox Fluid Producer"] = "RemoteSandboxFluidProducer",
            ["Wires"] = "RemoteWires",
            ["Signal Producer"] = "RemoteSignalProducer",
            ["Belt Filter"] = "RemoteBeltFilter",
            ["Belt Reader"] = "RemoteBeltReader",
            ["Pipe Gate"] = "RemotePipeGate",
            ["Display"] = "RemoteDisplay",
            ["Button"] = "RemoteButton",
            ["Logic Gates"] = "RemoteLogicGates",
            ["Simulated Buildings"] = "RemoteSimulatedBuildings",
            ["Global Signal Transmission"] = "RemoteGlobalSignalTransmission",
            ["Operator Signal Receiver"] = "RemoteOperatorSignalReceiver",
            ["Fluid Tank"] = "RemoteFluidTank",
            ["1x1 Foundation"] = "Remote1x1Foundation",
            ["1x2 Foundation"] = "Remote1x2Foundation",
            ["Space Belt"] = "RemoteSpaceBelt",
            ["Shape Miner"] = "RemoteShapeMiner",
            ["Shape Miner Extension"] = "RemoteShapeMinerExtension",
            ["Shape Miner + Extension"] = "RemoteShapeMinerPlusExtension",
            ["Space Pipe"] = "RemoteSpacePipe",
            ["Fluid Miner"] = "RemoteFluidMiner",
            ["Fluid Miner Extension"] = "RemoteFluidMinerExtension",
            ["Fluid Miner + Extension"] = "RemoteFluidMinerPlusExtension",
            ["Rails"] = "RemoteRails",
            ["Shape (Un)loader"] = "RemoteShapeUnLoader",
            ["Shape Wagon"] = "RemoteShapeWagon",
            ["Red Trains"] = "RemoteRedTrains",
            ["1x3 Foundation"] = "Remote1x3Foundation",
            ["1x4 Foundation"] = "Remote1x4Foundation",
            ["3-Blocks L Foundation"] = "Remote3BlocksLFoundation",
            ["4-Blocks L Foundation"] = "Remote4BlocksLFoundation",
            ["T Foundation"] = "RemoteTFoundation",
            ["S Foundation"] = "RemoteSFoundation",
            ["Cross Foundation"] = "RemoteCrossFoundation",
            ["2x2 Foundation"] = "Remote2x2Foundation",
            ["2x3 Foundation"] = "Remote2x3Foundation",
            ["2x4 Foundation"] = "Remote2x4Foundation",
            ["3x3 Foundation"] = "Remote3x3Foundation",
            ["Green Trains"] = "RemoteGreenTrains",
            ["Blue Trains"] = "RemoteBlueTrains",
            ["Cyan Trains"] = "RemoteCyanTrains",
            ["Magenta Trains"] = "RemoteMagentaTrains",
            ["Yellow Trains"] = "RemoteYellowTrains",
            ["White Trains"] = "RemoteWhiteTrains",
            ["Fluid (Un)loader"] = "RemoteFluidUnLoader",
            ["Fluid Wagon"] = "RemoteFluidWagon",
            ["Shape Wagon Transfer"] = "RemoteShapeWagonTransfer",
            ["Fluid Wagon Transfer"] = "RemoteFluidWagonTransfer",
            ["Train Quick Stop"] = "RemoteTrainQuickStop",
            ["Train Wait Stop"] = "RemoteTrainWaitStop",
            ["Filler Wagon"] = "RemoteFillerWagon",
            ["Roller Coaster"] = "RemoteRollerCoaster",
            ["2nd Floor"] = "Remote2ndFloor",
            ["Upgrades"] = "RemoteUpgrades",
            ["Blueprints"] = "RemoteBlueprints",
            ["Space Platforms"] = "RemoteSpacePlatforms",
            ["2nd Platform Floor"] = "Remote2ndPlatformFloor",
            ["Fluids"] = "RemoteFluids",
            ["Operator Levels"] = "RemoteOperatorLevels",
            ["Trains"] = "RemoteTrains",
            ["3rd Platform Floor"] = "Remote3rdPlatformFloor",
            ["Infinite Goals"] = "RemoteInfiniteGoals",
            ["3rd Floor"] = "Remote3rdFloor",
            ["Wires (Category)"] = "RemoteWiresCategory",
            ["Train Delivery"] = "RemoteTrainDelivery",
        };

        public static string MilestoneLocation(int milestoneNum, int itemNum)
        {
            return $"Milestone {milestoneNum} reward #{itemNum}";
        }

        public static string TaskLocation(int lineNum, int taskNum)
        {
            return $"Task #{lineNum}-{taskNum}";
        }

        public static string OperatorLevelLocation(int level)
        {
            return $"Operator level {level}";
        }

        public static string RemoteUpgrade(string itemName)
        {
            if (itemName.StartsWith("Task line #"))
            {
                return $"RemoteTaskLine{itemName["Task line #".Length..]}";
            }
            else if (itemName.StartsWith("Operator line #"))
            {
                return $"RemoteOperatorLine{itemName["Operator line #".Length..]}";
            }
            else if (itemName.EndsWith("Research Points"))
            {
                return "rp" + itemName.Split()[0];
            }
            else if (itemName.EndsWith("Blueprint Points"))
            {
                return "bp" + itemName.Split()[0];
            }
            else if (itemName.EndsWith("Platforms"))
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
