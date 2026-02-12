using AutoMapper;
using evuNETcore.DTOs;
using evuNETcore.Entity;

namespace evuNETcore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Regla 1: Cuando tengas un 'ExchangeRate' (DB) y quieras un 'ExchangeRateDto' (Salida),
            // cópialo automático. Y viceversa (ReverseMap).
            CreateMap<ExchangeRate, ExchangeRateDto>().ReverseMap();

            // Regla 2: Cuando recibas un 'CreateExchangeRateDto' (Entrada),
            // conviértelo en una entidad 'ExchangeRate' para poder guardarlo en la DB.
            CreateMap<CreateExchangeRateDto, ExchangeRate>();
        }
    }
}
