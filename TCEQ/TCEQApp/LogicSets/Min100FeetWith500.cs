using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class Min100FeetWith500 : SimpleStringLogicSet, ILogicSet
    {
        object ILogicSet.GetFactoryKey()
        {
            return "Min100FeetWith500";
        }

        protected override string OutputString
        {
            get
            {
                return "Protect to a minimum of 100 feet depth.  Local areas should be protected to 500 feet depth.";
            }
        }
    }
}