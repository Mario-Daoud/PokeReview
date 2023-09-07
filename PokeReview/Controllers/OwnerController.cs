using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokeReview.Dto;
using PokeReview.Interfaces;
using PokeReview.Models;

namespace PokeReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId)) return NotFound();

            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId)) return NotFound();

            var pokemon = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {
            if (ownerCreate == null) return BadRequest(ModelState);

            var owner = _ownerRepository.GetOwners()
                .Where(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.Trim().ToUpper())
                .FirstOrDefault();

            if (owner != null)
            {
                ModelState.AddModelError("", "Country already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest();

            var ownerMap = _mapper.Map<Owner>(ownerCreate);
            ownerMap.Country = _countryRepository.GetCountry(countryId);

            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created.");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updateOwner)
        {
            if (updateOwner == null) return BadRequest(ModelState);

            if (ownerId != updateOwner.Id) return BadRequest(ModelState);

            if (!_ownerRepository.OwnerExists(ownerId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ownerMap = _mapper.Map<Owner>(updateOwner);

            if (!_ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner.");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully updated.");
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwnerry(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId)) return NotFound();

            var ownerDelete = _ownerRepository.GetOwner(ownerId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_ownerRepository.DeleteOwner(ownerDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner.");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully deleted");
        }
    }
}
