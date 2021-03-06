﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TCEQApp.LogicSets
{
    public class Gulf_coast_oakville_2013: Gulf_coast_base_fresh_2012, ILogicSet
    {
        protected double _top_oakville = 0;
        protected double _top_oakvilleGSFC = 0;

        public override object GetFactoryKey()
        {
            return "Gulf_coast_oakville_2013";
        }

        protected override void getData()
        {
            //
            base.getData();

            ArcGISRESTClient.Layer oakvilleLayer = RestClient.GetLayerByName(GetSettingValueFromConfig("OAKVILLE_LAYER_NAME"));

            //We need to query the oakville dataset.
            _top_oakville = oakvilleLayer.QueryRasterLayer(_coords.X, _coords.Y);
            _top_oakvilleGSFC = _elevation - _top_oakville;

        }

        /// <summary>
        /// Override of the base logic inherited from the gulf coast base fresh logic type.
        /// </summary>
        /// <returns>HTML generic control containing the result data.</returns>
        public override HtmlGenericControl GenerateOutputControl()
        {
            HtmlGenericControl hgc = base.GenerateOutputControl();

            TableRow trOakville = new TableRow();
            TableCell tcOakvilleLabelCell = new TableCell();
            TableCell tcOakvilleValueCell = new TableCell();

            string tOakvilleMSLAbove = AboveBelow(_top_oakville);
            
            tcOakvilleLabelCell.Text = "Top of Oakville Isolation Interval";
            tcOakvilleValueCell.Text = Math.Round(Math.Abs(_top_oakvilleGSFC), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(_top_oakville), 0).ToString() + "' " + tOakvilleMSLAbove + " MSL)";

            trOakville.Cells.Add(tcOakvilleLabelCell);
            trOakville.Cells.Add(tcOakvilleValueCell);

            _resultsTable.Rows.Add(trOakville);

            return hgc;
        }
    }
}