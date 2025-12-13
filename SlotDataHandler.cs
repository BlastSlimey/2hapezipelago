using System;
using System.Collections.Generic;
using System.Text;

namespace _2hapezipelago
{
    public class SlotDataHandler
    {
        public enum Goaltype
        {
            Milestone,
            Operator,
        }

        public Dictionary<string, object> SlotData;
        public Goaltype Goal;
        public int MilestoneGoalNumber;
        public int OperatorGoalLevel;

        public SlotDataHandler(Dictionary<string, object> slotData)
        {
            SlotData = slotData;
            
            if ((string)slotData["goal"] == "Milestone")
            {
                Goal = Goaltype.Milestone;
            }
            else if ((string)slotData["goal"] == "Operator level")
            {
                Goal = Goaltype.Operator;
            }
            else
            {
                throw new Exception("Bad goal in slot data: " + (string)slotData["goal"]);
            }

            MilestoneGoalNumber = (int)slotData["milestone_goal_number"];
            OperatorGoalLevel = (int)slotData["operator_goal_level"];
        }
    }
}
