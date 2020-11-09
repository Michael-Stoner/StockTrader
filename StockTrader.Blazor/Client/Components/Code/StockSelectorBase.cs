using Microsoft.AspNetCore.Components;
using StockTrader.Blazor.Client.Components.ViewModels;
using StockTrader.Dtos;
using Syncfusion.Blazor;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StockTrader.Blazor.Client.Components.Code
{
    public class StockSelectorBase : ComponentBase
    {

        protected bool showPredefinedGroupDialog = false;
        protected int PredefinedGroupId { get; set; }
        protected string PredefinedGroupName { get; set; }
        protected PredefinedGroup PredefinedGroup { get; set; }
        protected List<PredefinedGroup> PredefinedGroups { get; set; }

        protected bool showStockSelector { get; set; } = false;
        private List<string> originalSelections { get; set; }

        protected Type model = typeof(StockReadDto);
        protected List<StockReadDto> Stocks = new List<StockReadDto>();

        [Parameter]
        public List<string> Selections { get; set; } = new List<string>();

        [Parameter]
        public EventCallback<List<string>> SelectionsChanged { get; set; }

        [Inject]
        protected HttpClient _http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {

                Stocks = await _http.GetFromJsonAsync<List<StockReadDto>>("https://localhost:5051/api/stocks");

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            //setup some temporary predefined groups
            PredefinedGroups = new List<PredefinedGroup> {
                new PredefinedGroup { Id = 1, Name="FAANG", StockTickers = new List<string> {"F", "AMZN", "AAPL", "NFLX", "GOOG"}},
                new PredefinedGroup { Id = 1, Name="FAAMG", StockTickers = new List<string> {"F", "AMZN", "AAPL", "MSFT",  "GOOG"}},

            };

            PredefinedGroupId = PredefinedGroups.First().Id;
        }

        public void ShowStockSelector()
        {
            originalSelections = Selections?.ToList();
            showStockSelector = true;
        }

        protected async Task OnChange(MultiSelectChangeEventArgs<List<string>> args)
        {
            await SelectionsChanged.InvokeAsync(Selections);

        }

        protected void BeforeDialogOpen(Syncfusion.Blazor.Popups.BeforeOpenEventArgs args)
        {

        }

        protected async Task StockSelectorOkClicked()
        {
            showStockSelector = false;
            await SelectionsChanged.InvokeAsync(Selections);
        }

        protected async Task StockSelectorCancelClicked()
        {
            Selections = originalSelections?.ToList();
            showStockSelector = false;
            await SelectionsChanged.InvokeAsync(Selections);
        }

        protected async Task PredefinedDialogOkClicked()
        {
            PredefinedGroup = PredefinedGroups.Find(g => g.Name.Equals(PredefinedGroupName));

            if (PredefinedGroup != null)
            {

                Selections ??= new List<string>();

                Selections.AddRange(PredefinedGroup.StockTickers.Except(Selections));

                await SelectionsChanged.InvokeAsync(Selections);

                showPredefinedGroupDialog = false;
            }
        }

        protected void PredefinedDialogCancelClicked()
        {
            showPredefinedGroupDialog = false;
        }

        protected void DialogClose(CloseEventArgs args)
        {

        }

        protected void HideDialog(Object e)
        {

        }
    }
}