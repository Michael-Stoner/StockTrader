using Microsoft.AspNetCore.Components;
using StockTrader.Approaches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StockTrader.Blazor.Client.Pages.Code
{
    public class NewApproachBase : ComponentBase
    {
        [Parameter]
        public ApproachDefinition ApproachDefinition { get; set; } = new ApproachDefinition();

        [Parameter]
        public EventCallback<ApproachDefinition> ApproachDefinitionChanged { get; set; }

        //protected override async Task OnInitializedAsync()
        //{

        //}

        //public List<AlgorithmDefinition> AvailableAlgorithms
        //{
        //    get
        //    {
        //        var algos = ApproachManager.AllAlgorithms;

        //        //build a list of definitions
        //        foreach (var algo in algos)
        //        {
        //            algo.GetDefaultDefinition(true);
        //        }

        //    }
        //}

    }
}