using AutoMapper;
using PokeReview.Data;
using PokeReview.Interfaces;
using PokeReview.Models;

namespace PokeReview.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CountryRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public bool CountryExists(int id)
        {
            return _context.Countries.Any(c => c.Id == id);
        }

        public ICollection<Country> GetCountries()
        {
            throw new NotImplementedException();
        }

        public Country GetCountry(int id)
        {
            throw new NotImplementedException();
        }

        public Country GetCountryByOwner(int ownerId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Owner> GetOwnersFromACountry(int countryId)
        {
            throw new NotImplementedException();
        }
    }
}
