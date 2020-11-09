using Microsoft.AspNetCore.Components;
using StockTrader.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StockTrader.Blazor.Client.Pages.Code
{
    public class ApproachesBase : ComponentBase
    {
        [Inject]
        protected HttpClient _http { get; set; }

        public List<ApproachReadDto> approaches { get; set; } = new List<ApproachReadDto>();

        public bool ShowDefine { get; set; }

        protected override async Task OnInitializedAsync()
        {

            try
            {
                approaches = await _http.GetFromJsonAsync<List<ApproachReadDto>>("https://localhost:5051/api/Approaches");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        protected void DefineNewApproach()
        {
            ShowDefine = true;
        }
    }

}