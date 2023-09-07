using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokeReview.Dto;
using PokeReview.Interfaces;
using PokeReview.Models;

namespace PokeReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId)) return NotFound();

            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(review);
        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewForPokemon(int pokeId)
        {
            var review = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfPokemon(pokeId));

            if (!ModelState.IsValid) return BadRequest();

            return Ok(review);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null) return BadRequest(ModelState);

            var reviews = _reviewRepository.GetReviews()
                .Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.Trim().ToUpper())
                .FirstOrDefault();

            if (reviews != null)
            {
                ModelState.AddModelError("", "Review already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest();

            var reviewMap = _mapper.Map<Review>(reviewCreate);
            
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);

            if (!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created.");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updateReview)
        {
            if (updateReview == null) return BadRequest(ModelState);

            if (reviewId != updateReview.Id) return BadRequest(ModelState);

            if (!_reviewRepository.ReviewExists(reviewId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Review>(updateReview);

            if (!_reviewRepository.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong updating review.");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully updated.");
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var reviewDelete = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReview(reviewDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting review");
            }

            return Ok("Succesfully deleted.");
        }
    }
}
