using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    /// <summary>
    /// Wrapper on the Salt dome from 2011 class.  The behavior here is identical.
    /// </summary>
    public class WilcoxDEM2012 : SaltDome2011, ILogicSet
    {
        object ILogicSet.GetFactoryKey()
        {
            return "WilcoxDEM2012";
        }
    }
}