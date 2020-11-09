using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StockTrader.Dtos;
using StockTrader.Data.Models;

namespace StockTrader.API.MapperProfiles
{
    public class StockTraderProfiles : Profile
    {
        public StockTraderProfiles()
        {
            CreateMap<StockData, StockDataReadDto>();
            CreateMap<Stock, StockReadDto>();

            CreateMap<Approach, ApproachReadDto>();
            CreateMap<ApproachCreateDto, Approach>();

            //The run read dto will not return the definition by default
            //The run definition dto will get the definition of the run at a different endpoint
            CreateMap<Run, RunReadDto>();
            CreateMap<Run, RunDefinitionReadDto>();
            CreateMap<RunCreateDto, Run>();

        }

    }
}