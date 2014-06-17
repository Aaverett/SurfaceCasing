using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class SaltDome300_1500 : SimpleStringLogicSet, ILogicSet
    {
        object ILogicSet.GetFactoryKey()
        {
            return "SaltDome300_1500";
        }

        protected override string OutputString
        {
            get
            {
                return "Depth to salt is 300 to 1500 feet.";
            }
        }
    }
}