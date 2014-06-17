using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class Oakville_2013 : BaseLogicSet, ILogicSet
    {
        protected double _oakvilleDepth = 0;

        public override object GetFactoryKey()
        {
            return "Oakville_2013";
        }

        protected override void getData()
        {
            base.getData();

            double _oakvilleDepth = g.queryRaster(_coords, "sde.SDE.TCEQ_OAKVILLE_TOP");

            _botFWIZValueString = "N/A";
        }

        public override System.Web.UI.HtmlControls.HtmlGenericControl GenerateOutputControl()
        {
            System.Web.UI.HtmlControls.HtmlGenericControl ret = base.GenerateOutputControl();

            //Fish the table out of hc.
            try
            {
                double _oakvilleElevMSL = _elevation - _oakvilleDepth;

                string oakvilleAboveBelowMSL = AboveBelow(_oakvilleElevMSL);

                //Make a new row for the table.
                System.Web.UI.WebControls.TableRow oakvilleRow = new System.Web.UI.WebControls.TableRow();
                System.Web.UI.WebControls.TableCell labelCell = new System.Web.UI.WebControls.TableCell();
                labelCell.ID = "oakvilleLabelCell";
                labelCell.Text = "Top of Oakville Isolation Interval: ";
                oakvilleRow.Cells.Add(labelCell);

                System.Web.UI.WebControls.TableCell valueCell = new System.Web.UI.WebControls.TableCell();
                valueCell.ID = "oakvilleValueCell";
                valueCell.Text = Math.Round(Math.Abs(_oakvilleDepth), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(_oakvilleElevMSL), 0).ToString() + "' " + oakvilleAboveBelowMSL + " MSL)"; ;
                oakvilleRow.Cells.Add(valueCell);

                _resultsTable.Rows.Add(oakvilleRow);
            }
            catch
            {

            }

            return ret;
        }
    }
}