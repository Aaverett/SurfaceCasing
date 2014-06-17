using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class SaltDome250 : SimpleStringLogicSet, ILogicSet
    {
        object ILogicSet.GetFactoryKey()
        {
            return "SaltDome250";
        }

        protected override string OutputString
        {
            get
            {
                return "Protect to 250 feet depth.";
            }
        }
    }
}