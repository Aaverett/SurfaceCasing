using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class SaltDome700_800 : SimpleStringLogicSet, ILogicSet
    {
        object ILogicSet.GetFactoryKey()
        {
            return "SaltDome700_800";
        }

        protected override string OutputString
        {
            get
            {
                return "Depth to cap rock and salt is 700 to 800 feet.";
            }
        }
    }
}