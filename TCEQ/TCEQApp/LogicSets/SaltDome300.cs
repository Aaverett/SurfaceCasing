using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class SaltDome300 : SimpleStringLogicSet, ILogicSet
    {
        object ILogicSet.GetFactoryKey()
        {
            return "SaltDome300";
        }

        protected override string OutputString
        {
            get
            {
                return "Protect to 300 feet depth.";
            }
        }
    }
}