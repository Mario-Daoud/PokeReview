using AutoMapper;
using PokeReview.Dto;
using PokeReview.Models;

namespace PokeReview.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Pokemon, PokemonDto>();
        }
    }
}
