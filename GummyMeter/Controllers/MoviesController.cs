using GummyMeter.Data;
using GummyMeter.Models;
using GummyMeter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace GummyMeter.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TmdbService _tmdbService;
        private static readonly Random _rng = new();

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
                    FakeTomatometer = new Random().Next(0, 101),
                    FakePopcornmeter = new Random().Next(0, 101)
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
                    VoteAverage = m.GetProperty("vote_average").GetDouble(),
                    FakeTomatometer = new Random().Next(0, 101),
                    FakePopcornmeter = new Random().Next(0, 101)
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
                    VoteAverage = m.GetProperty("vote_average").GetDouble(),
                    FakeTomatometer = new Random().Next(0, 101),
                    FakePopcornmeter = new Random().Next(0, 101)
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
                    VoteAverage = m.GetProperty("vote_average").GetDouble(),
                    FakeTomatometer = new Random().Next(0, 101),
                    FakePopcornmeter = new Random().Next(0, 101)
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
                    VoteAverage = m.GetProperty("vote_average").GetDouble(),
                    FakeTomatometer = new Random().Next(0, 101),
                    FakePopcornmeter = new Random().Next(0, 101)
                })
                .ToList();

            movieViewModel2.TopRatedMovies = topRatedMovies;




            return View(movieViewModel2);
        

        }

        public IActionResult Search() => View();


        [HttpGet("/Movies/Category/{category}")]
        public async Task<IActionResult> Category(string category, int page = 1)
        {
            JsonElement json;
            try
            {
                json = await _tmdbService.GetMoviesByCategoryAsync(category, page);
            }
            catch (ArgumentException)
            {
                return NotFound(); // unknown category
            }

            var movies = ParseMovies(json);
            var movieIds = movies.Select(m => m.Id).ToList();
            var counts = await _context.Reviews
               .Where(r => movieIds.Contains(Convert.ToInt32(r.MovieId)))
               .GroupBy(r => r.MovieId)
               .Select(g => new { MovieId = g.Key, Count = g.Count() })
               .ToListAsync();

            // 4) Assign each movie’s ReviewCount (0 if none)
            foreach (var m in movies)
                m.ReviewCount = counts
                    .FirstOrDefault(c => int.Parse(c.MovieId) == m.Id)?.Count ?? 0;

            ViewBag.Category = category;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = json.GetProperty("total_pages").GetInt32();
            return View(movies);
        }


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

            var movieIds = movies.Select(m => m.Id).ToList();
            var counts = await _context.Reviews
               .Where(r => movieIds.Contains(Convert.ToInt32(r.MovieId)))
               .GroupBy(r => r.MovieId)
               .Select(g => new { MovieId = g.Key, Count = g.Count() })
               .ToListAsync();

            // 4) Assign each movie’s ReviewCount (0 if none)
            foreach (var m in movies)
                m.ReviewCount = counts
                    .FirstOrDefault(c => int.Parse(c.MovieId) == m.Id)?.Count ?? 0;



            ViewBag.SearchQuery = query;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = searchResultsJson.GetProperty("total_pages").GetInt32();
            ViewBag.TotalResults = searchResultsJson.GetProperty("total_results").GetInt32();
            return View(movies);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReviewAjax(int movieId, string content, string subject, int rate)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            //var movieIdStr = movieId.ToString();

            // validate
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest(new { error = "Review cannot be empty." });


            // Only allow 1 review per user per movie
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.MovieId == movieId.ToString());
            if (existingReview != null)
            {
                return BadRequest(new { error = "You’ve already reviewed this movie." });
            }

            var review = new Review
            {
                UserId = user.Id,
                MovieId = movieId.ToString(),
                Content = content,
                CreatedAt = DateTime.UtcNow,
                Subject = subject
                
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await RateMovieAjax(movieId, rate);


            // return RedirectToAction("Details", new { id = movieId });
            // return the new review data
            return Ok(new
            {
                username = user.Username,
                content = review.Content,
                createdAt = review.CreatedAt.ToLocalTime().ToString("g")
            });
        }

       
        private async Task<IActionResult> RateMovieAjax(int movieId, int score)
        {
            if (score < 1 || score > 5)
                return BadRequest("Invalid score");

            var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var existing = await _context.Ratings
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == uid);

            if (existing != null)
            {
                existing.Score = score;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    MovieId = movieId,
                    UserId = uid,
                    Score = score
                });
            }
            await _context.SaveChangesAsync();

            // recompute average
            var avg = await _context.Ratings
                .Where(r => r.MovieId == movieId)
                .AverageAsync(r => r.Score);

            return Ok(new
            {
                average = Math.Round(avg, 2),
                userScore = score
            });
        }


        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return null;



            string? trailerKey = "";
            var movieJson = await _tmdbService.GetMovieDetailsAsync(id);
            var creditsJson = await _tmdbService.GetMovieCreditsAsync(id);

            var videosJson = await _tmdbService.GetMovieVideosAsync(id);

            var _mpaaRating = await _tmdbService.GetUsMpaaRatingAsync(id);


            var reviews = await _context.Reviews
                .Where(r => r.MovieId == id.ToString())
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var criticReviews = reviews.Where(r => r.IsCritic).ToList();
            var audienceReviews = reviews.Where(r => !r.IsCritic).ToList();

            double tomatometer = criticReviews.Any()
               ? criticReviews.Count(r => r.Score >= 3) * 100.0 / criticReviews.Count
               : 0;

            double popcornmeter = audienceReviews.Any()
                ? audienceReviews.Count(r => r.Score >= 3) * 100.0 / audienceReviews.Count
                : 0;

            if (videosJson.TryGetProperty("results", out JsonElement results)
                && results.ValueKind == JsonValueKind.Array
                && results.GetArrayLength() > 0)
            {
                // Get first YouTube trailer
                trailerKey = videosJson.GetProperty("results")
                .EnumerateArray()
                .FirstOrDefault(v =>
                    v.TryGetProperty("type", out var type) && type.GetString() == "Trailer" &&
                    v.TryGetProperty("site", out var site) && site.GetString() == "YouTube")
                .GetProperty("key").GetString();
            }
                

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
                                    : new List<string>(),
                MPARating = _mpaaRating,
                Runtime = movieJson.GetProperty("runtime").GetInt32().ToString(),
                FakeTomatometer = new Random().Next(0, 101),
                FakePopcornmeter = new Random().Next(0, 101)

            };

            movie.GenresFormatted = string.Join(",", movie.Genres);
        

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

            var directorProfilePath = creditsJson.GetProperty("crew")
                .EnumerateArray()
                .FirstOrDefault(c => c.TryGetProperty("job", out var job) && job.GetString() == "Director")
                .GetProperty("profile_path").GetString() ?? "";

            var writers = creditsJson.GetProperty("crew")
               .EnumerateArray()
               .Where(c => c.TryGetProperty("department", out var job) && job.GetString() == "Writing")
               .Select(c => c.GetProperty("name").GetString() ?? "").Distinct().ToList();

            var producers = creditsJson.GetProperty("crew")
               .EnumerateArray()
                .Where(c => c.TryGetProperty("department", out var job) && job.GetString() == "Production")
               .Select(c => new CastMember
               {
                   Name = c.GetProperty("name").GetString() ?? "",
                   //Character = c.GetProperty("character").GetString() ?? "",
                   ProfilePath = c.TryGetProperty("profile_path", out var profile) ? profile.GetString() : null
               }).ToList();


            // 1) compute average
            var ratings = await _context.Ratings
                .Where(r => r.MovieId == id)
                .ToListAsync();

            var avg = ratings.Any()
                ? ratings.Average(r => r.Score)
                : 0;

            // 2) find current user’s rating if signed in
            int? userScore = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                userScore = ratings.FirstOrDefault(r => r.UserId == uid)?.Score;
            }

            var viewModel = new MovieDetailViewModel
            {
                MovieId = id,
                Movie = movie,
                Reviews = reviews,
                Cast = cast,
                Director = director,
                TrailerYoutubeKey = trailerKey,
                Writers = writers,
                Ratings = ratings,
                Avg = avg,
                UserRating = userScore,
                Producers = producers,
                Tomatometer = tomatometer,
                Popcornmeter = popcornmeter,
            };

            return View(viewModel);
        }




    }
}
