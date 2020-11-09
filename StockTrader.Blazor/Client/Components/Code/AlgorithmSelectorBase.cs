using Microsoft.AspNetCore.Components;
using StockTrader.Approaches;
using StockTrader.Blazor.Client.Components.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Blazor.Grids;

namespace StockTrader.Blazor.Client.Components.Code
{
    public class AlgorithmSelectorBase : ComponentBase
    {
        protected bool ShowAlgorithmSelector { get; set; }

        //[Parameter]
        protected List<AlgorithmDefinition> AvailableAlgorithms { get; set; }

        //[Parameter]
        //public EventCallback<List<AlgorithmDefinition>> AvailableAlgorithmsChanged { get; set; }

        [Parameter]
        public List<AlgorithmDefinition> SelectedAlgorithms { get; set; } = new List<AlgorithmDefinition>();

        private List<AlgorithmDefinition> OriginalSelectedAlgorithms { get; set; } = new List<AlgorithmDefinition>();

        [Parameter]
        public EventCallback<List<AlgorithmDefinition>> SelectedAlgorithmsChanged { get; set; }

        public SfGrid<AlgorithmDefinition> AlgorithmGrid;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            AvailableAlgorithms = AlgorithmManager.GetAlgorithmDefaultDefinitions();
        }

        internal async Task AddAlgorithm(AlgorithmDefinition algorithm)
        {
            if (!SelectedAlgorithms?.Contains(algorithm) ?? false)
            {
                SelectedAlgorithms.Add(algorithm);
            }
            await SelectedAlgorithmsChanged.InvokeAsync(SelectedAlgorithms).ConfigureAwait(false);
        }

        internal async Task RemoveAlgorithm(AlgorithmDefinition algorithm)
        {
            if (SelectedAlgorithms?.Contains(algorithm) ?? false)
            {
                SelectedAlgorithms.Remove(algorithm);
            }

            await SelectedAlgorithmsChanged.InvokeAsync(SelectedAlgorithms).ConfigureAwait(false);
        }

        internal bool AlgorithmSelected(AlgorithmDefinition algorithm)
        {
            return SelectedAlgorithms?.Contains(algorithm) ?? false;
        }

        internal void ToggleAlgorithm(bool add, AlgorithmDefinition algorithmDefinition)
        {
            if (add)
            {
                _ = AddAlgorithm(algorithmDefinition);
            }
            else
            {
                _ = RemoveAlgorithm(algorithmDefinition);
            }
        }

        //internal async Task ParameterChanged(AlgorithmParameter param)
        //{
        //    //since the selected algorithm display is derived, I have to invoke a redraw
        //    this.StateHasChanged();

        //}

        public void OnChanged(ParameterChangedEventArgs args)
        {
            //since the selected algorithm display is derived, I have to invoke a redraw
            this.StateHasChanged();
        }

        //public async Task GetSelectedRecords(RowSelectEventArgs<AlgorithmDefinition> args)
        //{

        //    await AlgorithmGrid.DetailCollapseAll();

        //    await AlgorithmGrid.DetailExpandCollapseRow(args.Data);

        //}

        public void RowSelectHandler(RowSelectEventArgs<AlgorithmDefinition> args)
        {
            AlgorithmGrid.DetailExpandCollapseRow(args.Data);
        }

        public void RowDeselectHandler(RowDeselectEventArgs<AlgorithmDefinition> args)
        {
            AlgorithmGrid.DetailExpandCollapseRow(args.Data);
        }

        public async Task OkClicked()
        {
            ShowAlgorithmSelector = false;
            await SelectedAlgorithmsChanged.InvokeAsync(SelectedAlgorithms).ConfigureAwait(false);
        }

        public async Task CancelClicked()
        {
            ShowAlgorithmSelector = false;
            SelectedAlgorithms = OriginalSelectedAlgorithms;
            await SelectedAlgorithmsChanged.InvokeAsync(SelectedAlgorithms).ConfigureAwait(false);
        }

        public void Show()
        {
            OriginalSelectedAlgorithms = SelectedAlgorithms.ToList();  //Make a shallow copy
            ShowAlgorithmSelector = true;
        }
    }
}