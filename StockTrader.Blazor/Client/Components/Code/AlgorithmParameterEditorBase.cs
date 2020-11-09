using Microsoft.AspNetCore.Components;
using StockTrader.Approaches;
using StockTrader.Blazor.Client.Components.EventArgs;
using Syncfusion.Blazor.DocumentEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader.Blazor.Client.Components.Code
{
    public class AlgorithmParameterEditorBase : ComponentBase
    {
        [Parameter]
        public int ParmId { get; set; }

        public AlgorithmParameter Parameter { get; set; }

        [Parameter]
        public AlgorithmDefinition AlgorithmDefinition { get; set; }

        //[Parameter]
        //public EventCallback<AlgorithmParameter> ParameterChanged { get; set; }

        [Parameter]
        public EventCallback<ParameterChangedEventArgs> OnChanged { get; set; }

        //Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> e
        public async Task OnChangeAll(object variousChangeEventArgs)
        {

            await OnChanged.InvokeAsync(new ParameterChangedEventArgs { AlgorithmDefinition = AlgorithmDefinition, AlgorithmParameter = Parameter });
        }

        protected override void OnInitialized()
        {
            Parameter = AlgorithmDefinition.Parameters[ParmId];
            StateHasChanged();
        }
    }
}