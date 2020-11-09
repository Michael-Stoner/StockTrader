using AutoMapper;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockTrader.Approaches;
using StockTrader.Approaches.Run;
using StockTrader.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StockTrader.Blazor.Client.Components.Code
{
    public class DefineApproachBase : ComponentBase
    {
        [Parameter]
        public ApproachDefinition ApproachDefinition { get; set; } = new ApproachDefinition();

        [Parameter]
        public EventCallback<ApproachDefinition> ApproachDefinitionChanged { get; set; }

        [Parameter]
        public bool Visible { get; set; } = false;

        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        [Inject]
        public HttpClient _http { get; set; }

        [Inject]
        public ILocalStorageService localStorage { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected bool Step1Hidden { get; set; } = false;
        protected bool Step2Hidden { get; set; } = true;
        protected bool Step3Hidden { get; set; } = true;

        protected int TotalPermutationCount { get; set; }
        protected int CurrentPermutationCount { get; set; }

        protected void AlgorithmsChanged(List<AlgorithmDefinition> algorithms)
        {
            ApproachDefinition.BuyAlgorithms = algorithms?.Where(a => !a.SellDefinition).ToList() ?? new List<AlgorithmDefinition>();
            ApproachDefinition.SellAlgorithms = algorithms?.Where(a => a.SellDefinition).ToList() ?? new List<AlgorithmDefinition>();
        }

        protected void Step1Next()
        {
            Step1Hidden = true;
            Step2Hidden = false;
        }

        protected void Step2Back()
        {
            Step1Hidden = false;
            Step2Hidden = true;
        }

        protected async Task GenerateRuns()
        {
            Step2Hidden = true;
            Step3Hidden = false;

            //The callback is async, however Blazor is single threaded
            //(meaning all work is in a timeshare on a single thread)
            //So, we need to open the work in a new tab so that the UI does not lock.

            var success = await SaveApproachDefinition();

            if (success)
            {
                var url = $"https://localhost:5001/generateruns/{ApproachDefinition.Id}";
                await JSRuntime.InvokeAsync<object>("open", url, "_blank");
            }
            //Open a new tab with the ID of the approach definition

            StateHasChanged();
        }

        private async Task<bool> SaveApproachDefinition()
        {
            bool success;

            success = await SaveToServer(ApproachDefinition);

            if (success)
            {
                success = await SaveToLocalStorage(ApproachDefinition);
            }

            return success;

        }

        private async Task<bool> SaveToLocalStorage(ApproachDefinition approachDefinition)
        {
            try
            {
                Console.WriteLine("in SaveLocalStorage");
                await localStorage.SetItemAsync($"ApproachID:{approachDefinition.Id}", approachDefinition);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
            Console.WriteLine("out SaveLocalStorage");

            return true;

        }

        private async Task<bool> SaveToServer(ApproachDefinition approachDefinition)
        {
            ApproachCreateDto createApproach = new ApproachCreateDto
            {
                Name = approachDefinition.Name,
                ApproachDefinitionJson = System.Text.Json.JsonSerializer.Serialize(approachDefinition)
            };

            //Save to server
            using var response = await _http.PostAsJsonAsync("https://localhost:5051/api/approaches", createApproach);

            if (response.IsSuccessStatusCode)
            {
                var approachReadDto = await response.Content.ReadFromJsonAsync<ApproachReadDto>();
                approachDefinition.Id = approachReadDto.Id;

                //Now save to local storage
                return true;
            }

            return false;

        }
    }
}