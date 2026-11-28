using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
            JObject options = (JObject)slotData["options"];
            JObject? locAdjust = (JObject?)options["location_adjustments"];
            
            if ((string?)options["goal"] == "milestones")
            {
                Goal = Goaltype.Milestone;
            }
            else if ((string?)options["goal"] == "operator_levels")
            {
                Goal = Goaltype.Operator;
            }
            else
            {
                throw new Exception("Bad goal in slot data: " + (string?)options["goal"]);
            }

            MilestoneGoalNumber = (int?)locAdjust?["Milestones"] ?? 3;
            OperatorGoalLevel = (int?)locAdjust?["Operator lines"] ?? 0;
        }
    }
}
