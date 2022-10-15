using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        List<IMyCargoContainer> cargos = new List<IMyCargoContainer>();
        IMyTextSurface panel0;

        const string IGC_TAG = "STATION_MINAGE";
        string gridName = "Station Forage";
        string err_TXT = "";
        bool check = false;


        string status = "";

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (check)
            {
                status = gridName + "\nNb Cargo : " + cargos.Count + "\n";
                MyFixedPoint currentinventory = 0;
                MyFixedPoint maxinventory = 0;
                List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
                foreach (var cargo in cargos)
                {
                    currentinventory += cargo.GetInventory(0).CurrentVolume;
                    maxinventory += cargo.GetInventory(0).MaxVolume;
                    cargo.GetInventory(0).GetItems(inventoryItems);
                }
                double currentInv = AsDouble(currentinventory);
                double maxInv = AsDouble(maxinventory);

                double percentage = (currentInv * 100 / maxInv);

                foreach(var inventoryItem in inventoryItems)
                {
                    Echo(inventoryItem.Type.ToString());
                    status += inventoryItem.Type.SubtypeId + " - " + inventoryItem.Amount + " Kg\n";
                }


                status += "percentage : " + percentage.ToString("0.00") + " %\n";

                IGC.SendBroadcastMessage(IGC_TAG, status, TransmissionDistance.TransmissionDistanceMax);

                panel0.WriteText(status);
            }
            else
            {
                if (CheckList())
                {
                    check = true;
                }
                else
                {
                    Echo(err_TXT);
                }
            }
        }

        public double AsDouble(MyFixedPoint point)
        {
            return (double)point; // double.Parse(point.ToString());
        }

        public bool CheckList()
        {
            bool isReady = true;

            panel0 = Me.GetSurface(0);
            panel0.ContentType = ContentType.TEXT_AND_IMAGE;
            panel0.FontSize = 1.5f;
            panel0.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(blocks);

            foreach (var block in blocks)
            {
                /*
                LargePistonBase
LargeAdvancedStator
LargeBlockDrill
LargeBlockLargeIndustrialContainer
LargeBlockBatteryBlock
LargeBlockConveyorSorter
Connector
LargeBlockOffsetDoor
LargeProgrammableBlock
SmallLight
LargeBlockLight_2corner
LargeBlockSolarPanel
SmallBlockSmallModularThruster
SmallBlockLargeThrustSciFi
ConnectorMedium
SmallBlockConveyorSorter
SmallBlockRemoteControl
SmallBlockSensor
SmallBlockLargeContainer
SmallBlockBatteryBlock
SmallBlockGyro3
TimerBlockSmall
SmallBlockStandingCockpit
SmallProgrammableBlock

                 */
                
                if (block.CubeGrid.CustomName.Equals(Me.CubeGrid.CustomName))
                {
                    if (block.BlockDefinition.SubtypeName.Contains("Container"))
                    {
                        block.CustomName = gridName + " - Cargo";
                        cargos.Add((IMyCargoContainer)block);
                    }
                }

            }

            return isReady;
        }
    }
}
