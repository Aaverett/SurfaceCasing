using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class ProtectTo350ft_2014 : BaseLogicSet
    {
        public override object GetFactoryKey()
        {
            return "ProtectTo350Ft_2014";
        }

        protected override void getData()
        {
            base.getData();

            //Now, we'll get the aquifer data.
            ArcGISRESTClient.Layer l = RestClient.GetLayerByName(GetSettingValueFromConfig("AQUIFER_LAYER_NAME"));

            System.Data.DataTable aquifers = l.Query(null, _coords.GetJValue());

            _showAquiferRow = true;
            _aquiferValueString = ComposeAquiferNamesList(aquifers);

            _showBaseUSDWRow = false;
            _showTopFWIZRow = false;
            _showPBUQWRow = false;
            _showBotFWIZRow = false;

            _alternateTopSentence = "Protect to a depth of 350ft.";
            _alternatePBUQWSentence = "";
            _alternateUSDBSentence = "";

        }

        protected string ComposeAquiferNamesList(System.Data.DataTable aquifers)
        {
            string ret = string.Empty;

            List<string> listedAquifers = new List<string>();

            for (int i = 0; i < aquifers.Rows.Count; i++)
            {
                if (aquifers.Rows[i]["excludeFromList"].ToString() != "1")
                {
                    listedAquifers.Add(aquifers.Rows[i]["name"].ToString());
                }
            }

            for (int i = 0; i < listedAquifers.Count; i++)
            {
                ret += listedAquifers[i];

                if (i < listedAquifers.Count - 1)
                {
                    ret += ", ";
                }
            }

            return ret;
        }
    }
}