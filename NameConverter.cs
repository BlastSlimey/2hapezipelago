
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            ["Space Pipe"] = "RemoteSpacePipe",
            ["Fluid Miner"] = "RemoteFluidMiner",
            ["Fluid Miner Extension"] = "RemoteFluidMinerExtension",
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

        [Obsolete]
        public static Dictionary<string, string[]> BuildingIdByItem = new Dictionary<string, string[]>()
        {
            ["Conveyor Belt"] = new[] { 
                "BeltDefaultVariant", "BeltPortSenderVariant", "BeltPortReceiverVariant",
                "Merger2To1Variant", "Merger3To1Variant", "MergerTShapeVariant",
                "Splitter1To2Variant" , "Splitter1To3Variant", "SplitterTShapeVariant",
                "Lift1LayerVariant", "Lift2LayerVariant"
            },
            ["Extractor"] = new[] { "ExtractorDefaultVariant" },
            ["Half Destroyer"] = new[] { "CutterHalfVariant" },
            ["Rotator (CW)"] = new[] { "RotatorOneQuadVariant" },
            ["Stacker"] = new[] { "StackerStraightVariant" },
            ["Painter"] = new[] { "PainterDefaultVariant" },
            ["Pump"] = new[] { "PumpDefaultVariant" },
            ["Pipe"] = new[] { "PipeDefaultVariant", "Pipe1LayerVariant", "Pipe2LayerVariant", "FluidPortSenderVariant", "FluidPortReceiverVariant" },
            ["Pin Pusher"] = new[] { "PinPusherDefaultVariant" },
            ["Color Mixer"] = new[] { "MixerDefaultVariant" },
            ["Crystal Generator"] = new[] { "CrystalGeneratorDefaultVariant" },
            ["Cutter"] = new[] { "CutterDefaultVariant" },
            ["Swapper"] = new[] { "HalvesSwapperDefaultVariant" },
            ["Trash"] = new[] { "TrashDefaultVariant" },
            ["Rotator (CCW)"] = new[] { "RotatorOneQuadCCWVariant" },
            ["Rotator (180)"] = new[] { "RotatorHalfVariant" },
            ["Stacker (Bent)"] = new[] { "StackerDefaultVariant" },
            ["Label"] = new[] { "LabelDefaultVariant" },
            ["Overflow Splitter"] = new[] { "SplitterOverflowVariant" },
            ["Sandbox Item Producer"] = new[] { "SandboxItemProducerDefaultVariant" },
            ["Sandbox Fluid Producer"] = new[] { "SandboxFluidProducerDefaultVariant" },
            ["Wires"] = new[] { "WireDefaultVariant", "WireBridgeVariant", "WireTransmitterSenderVariant", "WireTransmitterReceiverVariant" },
            ["Signal Producer"] = new[] { "ConstantSignalDefaultVariant" },
            ["Belt Filter"] = new[] { "BeltFilterDefaultVariant" },
            ["Belt Reader"] = new[] { "BeltReaderDefaultVariant" },
            ["Pipe Gate"] = new[] { "PipeGateDefaultVariant" },
            ["Display"] = new[] { "DisplayDefaultVariant" },
            ["Button"] = new[] { "ButtonDefaultVariant" },
            ["Logic Gates"] = new[] { 
                "LogicGateAndVariant", "LogicGateOrVariant", "LogicGateIfVariant",
                "LogicGateXOrVariant", "LogicGateNotVariant", "LogicGateCompareVariant"
            },
            ["Simulated Buildings"] = new[] {
                "VirtualRotatorDefaultVariant", "VirtualAnalyzerDefaultVariant", "VirtualUnstackerDefaultVariant",
                "VirtualStackerDefaultVariant", "VirtualHalfCutterDefaultVariant", "VirtualPainterDefaultVariant",
                "VirtualPinPusherDefaultVariant", "VirtualCrystalGeneratorDefaultVariant", "VirtualHalvesSwapperDefaultVariant"
            },
            ["Global Signal Transmission"] = new[] { "ControlledSignalReceiverVariant", "ControlledSignalTransmitterVariant" },
            ["Operator Signal Receiver"] = new[] { "WireGlobalTransmitterReceiverVariant" },
            ["Fluid Tank"] = new[] { "FluidStorageDefaultVariant" },
        };

        [Obsolete]
        public static Dictionary<string, string[]> IslandBuildingIdByItem = new Dictionary<string, string[]>()
        {
            ["1x1 Foundation"] = new[] { "FoundationGroup_1x1" },
            ["1x2 Foundation"] = new[] { "FoundationGroup_1x2" },
            ["Space Belt"] = new[] { "SpaceBeltsGroup" },
            ["Shape Miner"] = new[] { "ShapeMinerExtractorsGroup" },
            ["Shape Miner Extension"] = new[] { "ShapeMinerChainsGroup" },
            ["Space Pipe"] = new[] { "SpacePipesGroup" },
            ["Fluid Miner"] = new[] { "FluidMinerExtractorsGroup" },
            ["Fluid Miner Extension"] = new[] { "FluidMinerChainsGroup" },
            ["Rails"] = new[] { 
                "TrainLaunchersGroup", "TrainCatchersGroup",
                "RailLiftUp1X1X2Group", "RailLiftDown1X1X2Group", "RailLiftUp1X1X3Group", "RailLiftDown1X1X3Group"
            },
            ["Shape (Un)loader"] = new[] { "TrainShapeLoadersGroup", "TrainShapeUnloadersGroup" },
            ["Shape Wagon"] = new[] { "ShapeCargoFactoriesGroup" },
            ["Red Trains"] = new[] { "RedTrainProducerGroup", "RedRailGroup" },
            ["1x3 Foundation"] = new[] { "FoundationGroup_1x3" },
            ["1x4 Foundation"] = new[] { "FoundationGroup_1x4" },
            ["3-Blocks L Foundation"] = new[] { "FoundationGroup_L3" },
            ["4-Blocks L Foundation"] = new[] { "FoundationGroup_L4" },
            ["T Foundation"] = new[] { "FoundationGroup_T4" },
            ["S Foundation"] = new[] { "FoundationGroup_S4" },
            ["Cross Foundation"] = new[] { "FoundationGroup_C5" },
            ["2x2 Foundation"] = new[] { "FoundationGroup_2x2" },
            ["2x3 Foundation"] = new[] { "FoundationGroup_2x3" },
            ["2x4 Foundation"] = new[] { "FoundationGroup_2x4" },
            ["3x3 Foundation"] = new[] { "FoundationGroup_3x3" },
            ["Green Trains"] = new[] { "GreenRailGroup", "GreenTrainProducerGroup" },
            ["Blue Trains"] = new[] { "BlueRailGroup", "BlueTrainProducerGroup" },
            ["Cyan Trains"] = new[] { "CyanRailGroup", "CyanTrainProducerGroup" },
            ["Magenta Trains"] = new[] { "MagentaTrainProducerGroup", "MagentaRailGroup" },
            ["Yellow Trains"] = new[] { "YellowTrainProducerGroup", "YellowRailGroup" },
            ["White Trains"] = new[] { "WhiteTrainProducerGroup", "WhiteRailGroup" },
            ["Fluid (Un)loader"] = new[] { "TrainFluidLoadersGroup", "TrainFluidUnloadersGroup" },
            ["Fluid Wagon"] = new[] { "FluidCargoFactoriesGroup" },
            ["Shape Wagon Transfer"] = new[] { "TrainShapeTransferGroup" },
            ["Fluid Wagon Transfer"] = new[] { "TrainFluidTransferGroup" },
            ["Train Quick Stop"] = new[] { "TrainQuickStationsGroup" },
            ["Train Wait Stop"] = new[] { "TrainWaitStationsGroup" },
            ["Filler Wagon"] = new[] { "FillerCargoFactoriesGroup" },
            ["Roller Coaster"] = new[] { "TrainTwistersGroup", "TrainLoopsGroup", "RailLiftUp2X1X2Group", "RailLiftDown2X1X2Group" },
        };

        [Obsolete]
        public static Dictionary<string, string[]> MechanicIdByItem = new Dictionary<string, string[]>()
        {
            ["2nd Floor"] = new[] { "RULayer2" },
            ["Upgrades"] = new[] { "RUSideUpgrades" },
            ["Blueprints"] = new[] { "RUBlueprints" },
            ["Space Platforms"] = new[] { "RUIslandPlacement" },
            ["2nd Platform Floor"] = new[] { "RUIslandLayer2" },
            ["Fluids"] = new[] { "RUFluids" },
            ["Operator Levels"] = new[] { "RUPlayerLevel" },
            ["Trains"] = new[] { "RUTrains" },
            ["3rd Platform Floor"] = new[] { "RUIslandLayer3" },
            ["Infinite Goals"] = new[] { "RUInfiniteGoals" },
            ["3rd Floor"] = new[] { "RULayer3" },
            ["Wires (Category)"] = new[] { "RUWires" },
            ["Train Delivery"] = new[] { "RUTrainHubDelivery" },
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

        [Obsolete]
        public static ISerializedResearchReward[] SerializeItem(string itemName)
        {
            if (itemName.StartsWith("Task line #"))
            {
                var serializedReward = new SerializedResearchRewardMechanic()
                {
                    MechanicId = $"TaskLine{itemName["Task line #".Length..]}"
                };
                return new[] { serializedReward };
            }
            else if (itemName.StartsWith("Operator line #"))
            {
                var serializedReward = new SerializedResearchRewardMechanic()
                {
                    MechanicId = $"OperatorLine{itemName["Operator line #".Length..]}"
                };
                return new[] { serializedReward };
            }
            else if (itemName.EndsWith("Research Points"))
            {
                var serializedReward = new SerializedResearchRewardResearchPoints()
                {
                    Amount = Int32.Parse(itemName.Split()[0])
                };
                return new[] { serializedReward };
            }
            else if (itemName.EndsWith("Platforms"))
            {
                var serializedReward = new SerializedResearchRewardChunkLimit()
                {
                    Amount = Int32.Parse(itemName.Split()[0])
                };
                return new[] { serializedReward };
            }
            else if (BuildingIdByItem.ContainsKey(itemName))
            {
                var serRewards = new SerializedResearchRewardBuildingDefinitionGroup[] {};
                foreach (var rewardId in BuildingIdByItem[itemName])
                {
                    serRewards.Append<SerializedResearchRewardBuildingDefinitionGroup>(
                        new SerializedResearchRewardBuildingDefinitionGroup()
                        {
                            BuildingDefinitionGroupId = rewardId
                        }
                    );
                }
                return serRewards;
            }
            else if (IslandBuildingIdByItem.ContainsKey(itemName))
            {
                var serRewards = new SerializedResearchRewardIslandGroup[] { };
                foreach (var rewardId in IslandBuildingIdByItem[itemName])
                {
                    serRewards.Append(
                        (SerializedResearchRewardIslandGroup) typeof(SerializedResearchRewardIslandGroup).GetConstructor(
                            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(new object[] { rewardId })
                    );
                }
                return serRewards;
            }
            else if (MechanicIdByItem.ContainsKey(itemName))
            {
                var serRewards = new SerializedResearchRewardMechanic[] { };
                foreach (var rewardId in MechanicIdByItem[itemName])
                {
                    serRewards.Append<SerializedResearchRewardMechanic>(
                        new SerializedResearchRewardMechanic()
                        {
                            MechanicId = rewardId
                        }
                    );
                }
                return serRewards;
            }
            else
            {
                throw new Exception($"Uknown item \"{itemName}\"");
            }
        }
    }
}
