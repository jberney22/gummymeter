using GummyMeter.Data;
using GummyMeter.Models;
using GummyMeter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;

namespace GummyMeter.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TmdbService _tmdbService;

        public MoviesController(AppDbContext context, TmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

       

        public async Task<IActionResult> GetTrending()
        {
            var moviesJson = await _tmdbService.GetTrendingMoviesAsync();
            var movies = ParseMovies(moviesJson);
            return PartialView("_TrendingSlider", movies);
        }

        [OutputCache(Duration = 0)]
        public async Task<IActionResult> GetTopRated()
        {
            var moviesJson = await _tmdbService.GetTopRatedMoviesAsync();
            var movies = ParseMovies(moviesJson);
            return PartialView("_MovieCategories", movies);
        }

        [OutputCache(Duration = 0)]
        public async Task<IActionResult> GetPopular()
        {
            var moviesJson = await _tmdbService.GetPopularMoviesAsync();
            var movies = ParseMovies(moviesJson);
            return PartialView("_MovieCategories", movies);
        }

        [OutputCache(Duration = 0)]
        public async Task<IActionResult> GetNowPlaying()
        {
            var moviesJson = await _tmdbService.GetNowPlayingMoviesAsync();
            var movies = ParseMovies(moviesJson);
            return PartialView("_MovieCategories", movies);
        }

        [OutputCache(Duration = 0)]
        public async Task<IActionResult> GetUpcoming()
        {
            var moviesJson = await _tmdbService.GetUpcomingMoviesAsync();
            var movies = ParseMovies(moviesJson);
            return PartialView("_MovieCategories", movies);
        }

        // helper
        private List<MovieViewModel> ParseMovies(JsonElement moviesJson)
        {
            return moviesJson.GetProperty("results")
                .EnumerateArray()
                .Select(m => new MovieViewModel
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Title = m.GetProperty("title").GetString() ?? "Untitled",
                    PosterPath = m.TryGetProperty("poster_path", out var poster) ? poster.GetString() : null,
                    VoteAverage = m.GetProperty("vote_average").GetDouble()
                })
                .ToList();
        }


        public async Task<IActionResult> Index()
        {
            MovieViewModel2 movieViewModel2 = new MovieViewModel2();
            var trendingMoviesJson = await _tmdbService.GetTrendingMoviesAsync();

            var trendingMovies = trendingMoviesJson.GetProperty("results")
                .EnumerateArray()
                .Select(m => new MovieViewModel
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Title = m.GetProperty("title").GetString() ?? "Untitled",
                    PosterPath = m.TryGetProperty("poster_path", out var poster) ? poster.GetString() : null,
                    VoteAverage = m.GetProperty("vote_average").GetDouble()
                })
                .ToList();

            movieViewModel2.TrendingMovies = trendingMovies;

            var popularMoviesJson = await _tmdbService.GetPopularMoviesAsync();
            var popularMovies = popularMoviesJson.GetProperty("results")
                .EnumerateArray()
                .Select(m => new MovieViewModel
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Title = m.GetProperty("title").GetString() ?? "Untitled",
                    PosterPath = m.TryGetProperty("poster_path", out var poster) ? poster.GetString() : null,
                    VoteAverage = m.GetProperty("vote_average").GetDouble()
                })
                .ToList();

            movieViewModel2.PopularMovies = popularMovies;

            var upComingMoviesJson = await _tmdbService.GetUpcomingMoviesAsync();
            var upcomingMovies = upComingMoviesJson.GetProperty("results")
                .EnumerateArray()
                .Select(m => new MovieViewModel
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Title = m.GetProperty("title").GetString() ?? "Untitled",
                    PosterPath = m.TryGetProperty("poster_path", out var poster) ? poster.GetString() : null,
                    VoteAverage = m.GetProperty("vote_average").GetDouble()
                })
                .ToList();

            movieViewModel2.UpcomingMovies = upcomingMovies;

            var nowPlayingMoviesJson = await _tmdbService.GetNowPlayingMoviesAsync();
            var nowplayingMovies = nowPlayingMoviesJson.GetProperty("results")
                .EnumerateArray()
                .Select(m => new MovieViewModel
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Title = m.GetProperty("title").GetString() ?? "Untitled",
                    PosterPath = m.TryGetProperty("poster_path", out var poster) ? poster.GetString() : null,
                    VoteAverage = m.GetProperty("vote_average").GetDouble()
                })
                .ToList();

            movieViewModel2.NowPlayingMovies = nowplayingMovies;


            var topRatedJson = await  _tmdbService.GetTopRatedMoviesAsync();
            var topRatedMovies = topRatedJson.GetProperty("results")
                .EnumerateArray()
                .Select(m => new MovieViewModel
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Title = m.GetProperty("title").GetString() ?? "Untitled",
                    PosterPath = m.TryGetProperty("poster_path", out var poster) ? poster.GetString() : null,
                    VoteAverage = m.GetProperty("vote_average").GetDouble()
                })
                .ToList();

            movieViewModel2.TopRatedMovies = topRatedMovies;




            return View(movieViewModel2);
        

        }

        public IActionResult Search() => View();

        [HttpGet]
        public async Task<IActionResult> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }

            var searchResultsJson = await _tmdbService.SearchMoviesAsync(query, page);

            if (!searchResultsJson.TryGetProperty("results", out var results))
            {
                return View(new List<MovieViewModel>()); // Return empty list if TMDB fails
            }

            var movies = searchResultsJson.GetProperty("results")
                .EnumerateArray()
                .Select(m => new MovieViewModel
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Title = m.GetProperty("title").GetString() ?? "Untitled",
                    PosterPath = m.TryGetProperty("poster_path", out var poster) ? poster.GetString() : "https://eticketsolutions.com/demo/themes/e-ticket/img/movie.jpg",
                    VoteAverage = m.GetProperty("vote_average").GetDouble()
                })
                .ToList();

            ViewBag.SearchQuery = query;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = searchResultsJson.GetProperty("total_pages").GetInt32();
            ViewBag.TotalResults = searchResultsJson.GetProperty("total_results").GetInt32();
            return View(movies);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int id, string content)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var movieId = id.ToString();

            // Only allow 1 review per user per movie
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.MovieId == movieId);
            if (existingReview != null)
            {
                return RedirectToAction("Details", new { id });
            }

            var review = new Review
            {
                UserId = user.Id,
                MovieId = movieId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var movieJson = await _tmdbService.GetMovieDetailsAsync(id);
            var creditsJson = await _tmdbService.GetMovieCreditsAsync(id);

            var videosJson = await _tmdbService.GetMovieVideosAsync(id);

            // Get first YouTube trailer
            var trailerKey = videosJson.GetProperty("results")
                .EnumerateArray()
                .FirstOrDefault(v =>
                    v.TryGetProperty("type", out var type) && type.GetString() == "Trailer" &&
                    v.TryGetProperty("site", out var site) && site.GetString() == "YouTube")
                .GetProperty("key").GetString();

            if (movieJson.ValueKind == JsonValueKind.Undefined)
            {
                return RedirectToAction("Index");
            }

            var movie = new MovieViewModel
            {
                Id = movieJson.GetProperty("id").GetInt32(),
                Title = movieJson.GetProperty("title").GetString() ?? "Untitled",
                PosterPath = movieJson.TryGetProperty("poster_path", out var poster) ? poster.GetString() : null,
                VoteAverage = movieJson.GetProperty("vote_average").GetDouble(),
                ReleaseDate = movieJson.GetProperty("release_date").GetString(),
                Overview = movieJson.GetProperty("overview").GetString(),
                Genres = movieJson.TryGetProperty("genres", out var genresArray)
                                    ? genresArray.EnumerateArray()
                                        .Select(g => g.GetProperty("name").GetString() ?? "")
                                        .ToList()
                                    : new List<string>()
            };

            var reviews = await _context.Reviews
                .Where(r => r.MovieId == id.ToString())
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var cast = creditsJson.GetProperty("cast")
                .EnumerateArray()
               // .Take(8) // Top 8 cast members
                .Select(c => new CastMember
                {
                    Name = c.GetProperty("name").GetString() ?? "",
                    Character = c.GetProperty("character").GetString() ?? "",
                    ProfilePath = c.TryGetProperty("profile_path", out var profile) ? profile.GetString() : null
                }).ToList();

            var director = creditsJson.GetProperty("crew")
                .EnumerateArray()
                .FirstOrDefault(c => c.TryGetProperty("job", out var job) && job.GetString() == "Director")
                .GetProperty("name").GetString() ?? "";

            var viewModel = new MovieDetailViewModel
            {
                MovieId = id,
                Movie = movie,
                Reviews = reviews,
                Cast = cast,
                Director = director,
                TrailerYoutubeKey = trailerKey
            };

            return View(viewModel);
        }


    }
}
