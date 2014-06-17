using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// The salt dome computation logic from the 2011 round.  This replaces the actual top 1k value with the ground surface.  Otherwise, it's identical to the base logic.
/// It is actually not exclusive to salt domes.
/// </summary>
public class SaltDome2011 : BaseLogicSet, ILogicSet
{
    #region ILogicSet Members

    object ILogicSet.GetFactoryKey()
    {
        return "SaltDome2011";
    }

    protected override void getData()
    {
        base.getData();

        _topgsfc = 0;
        _topmsl = elevation;
    }

    #endregion
}