using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using StockTrader.Approaches;
using System.Net.Http;
using System.Net.Http.Json;
using StockTrader.Approaches.Run;
using StockTrader.Dtos;
using System.Text.Json;
using System.Dynamic;
using System.Diagnostics;

namespace StockTrader.Blazor.Client.Pages.Code
{
    public class GenerateRunDefintionsBase : ComponentBase
    {

        protected bool GenerateError { get; set; } = false;
        protected int CurrentPermutation { get; set; }
        protected int TotalPermutations { get; set; } = 1;  //prevents divide by zero on render
        protected string TimeRemaining { get; set; }
        protected Stopwatch startWatch { get; set; }

        protected double PercentComplete
        {
            get
            {
                return (double)CurrentPermutation / (double)TotalPermutations;
            }
        }

        private int SavedPermutations { get; set; }

        protected int counter = 0;

        [Parameter]
        public int ApproachId { get; set; }

        [Inject]
        public ILocalStorageService localStorage { get; set; }

        [Inject]
        protected HttpClient _http { get; set; }

        private System.Timers.Timer uiTimer;
        private System.Timers.Timer startWorkTimer;

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //try to update the UI if possible.
            //uiTimer = new System.Timers.Timer(1000);

            //uiTimer.Elapsed += (sender, e) =>
            //{
            //    //Console.WriteLine("UI Update Timer Elapsed");
            //    StateHasChanged();

            //    Task.Delay(1);
            //};
            //uiTimer.AutoReset = true;
            //uiTimer.Start();

            startWorkTimer = new System.Timers.Timer(2000);  //Wait for everythign to paint before kicking off
            startWorkTimer.Elapsed += async (sender, e) =>
            {
                startWatch = new Stopwatch();
                startWatch.Start();
                await RunGeneration();

            };
            startWorkTimer.AutoReset = false;
            startWorkTimer.Start();
        }

        private async Task RunGeneration()
        {
            var approach = await GetApproachDefinitionAsync(ApproachId);

            if (approach != null)
            {
                var totalDefinitions = await RunDefinitionGenerator.GenerateAsync(approach,
                             new RunDefinitionGenerator.SaveRunDefintionsCallback(SaveRunDefintions),
                             100,
                             new RunDefinitionGenerator.StatusCallback(UpdateStatus),
                             100);

            }
            else
            {
                Console.WriteLine("No approach found");
                GenerateError = true;
                StateHasChanged();
            }
        }

        private async Task<ApproachDefinition> GetApproachDefinitionAsync(int approachId)
        {
            //check the local storage for the approach first
            var approach = await localStorage.GetItemAsync<ApproachDefinition>($"ApproachID:{approachId}");

            if (approach == null)
            {
                //If it's not in storage, then hit the server
                try
                {
                    var approachContainer = await _http.GetFromJsonAsync<ApproachReadDto>($"https://localhost:5051/api/approaches/{approachId}");

                    approach = JsonSerializer.Deserialize<ApproachDefinition>(approachContainer.ApproachDefinitionJson);

                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to retrieve approach definition from server");
                }
            }

            return approach;
        }

        private async Task UpdateStatus(int currentPermutation, int totalPermutations)
        {
            //Random rnd = new Random();

            CurrentPermutation = currentPermutation;
            TotalPermutations = totalPermutations;

            var status = new GenerationStatus
            {
                ApproachId = ApproachId,
                CurrentPermutation = currentPermutation,
                TotalPermutations = totalPermutations,
                SavedPermutations = SavedPermutations
            };

            var result = localStorage.SetItemAsync(status.GetKey(), status);

            if (!result.IsCompletedSuccessfully)
            {
                // if for some reason the status cannot be updated, we take some action
                //currently ignore
            }

            var multiplier = 1 / PercentComplete;

            TimeSpan ts = startWatch.Elapsed;

            var finalTimeSpan = ts.Multiply(multiplier).Subtract(ts);

            TimeRemaining = $"{finalTimeSpan.Days} days {finalTimeSpan.Hours} hours {finalTimeSpan.Minutes} minutes {finalTimeSpan.Seconds} seconds";

            await InvokeAsync(StateHasChanged);
            await Task.Delay(1);

            //Console.WriteLine($"Status Updated: {currentPermutation}");

            //The callback is async, however Blazor is single threaded
            //(meaning all work is in a timeshare on a single thread)
            //So, we need to yield so that the UI can update.

        }

        protected async Task SaveRunDefintions(List<RunDefinition> definitions)
        {
            //mock save

            //The callback is async, however Blazor is single threaded
            //(meaning all work is in a timeshare on a single thread)
            //So, we need to yield so that the UI can update.

            await Task.Delay(1).ConfigureAwait(false);
        }

    }
}