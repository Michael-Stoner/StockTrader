using StockTrader.Approaches;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;

namespace StockTrader.Approaches.Run
{

    /// <summary>
    /// s
    /// </summary>
    public class RunDefinition
    {
        public ApproachDefinition ApproachDefinition { get; set; }

        public List<RunAlgorithmDefinition> BuyAlgorithms { get; set; } = new List<RunAlgorithmDefinition>();
        public List<RunAlgorithmDefinition> SellAlgorithms { get; set; } = new List<RunAlgorithmDefinition>();

        public RunDefinition()
        {

        }

        public RunDefinition(ApproachDefinition definitionToClone)
        {
            ApproachDefinition = Utility.DeepClone<ApproachDefinition>(definitionToClone);
        }

        public string Name
        {
            get
            {
                string buyDescriptions = Common.GetAlgorithmsDescriptions(ApproachDefinition.BuyAlgorithms.OrderBy(a => a.Priority), true);
                string sellDescriptions = Common.GetAlgorithmsDescriptions(ApproachDefinition.SellAlgorithms.OrderBy(a => a.Priority), true);

                return $"B{buyDescriptions} S{sellDescriptions}";
            }
        }
    }
}