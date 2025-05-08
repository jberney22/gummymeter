using GummyMeter.Data;
using GummyMeter.Models;
using GummyMeter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TmdbService _tmdbService;

        public FavoritesController(AppDbContext context, TmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var favoritesQuery = _context.Favorites
                .Where(f => f.UserId == user.Id)
                .OrderByDescending(f => f.FavoritedAt);

            var totalFavorites = await favoritesQuery.CountAsync();
            var favorites = await favoritesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var detailedFavorites = new List<(Favorite favorite, System.Text.Json.JsonElement movie)>();

            foreach (var favorite in favorites)
            {
                var movie = await _tmdbService.GetMovieDetailsAsync(int.Parse(favorite.MovieId));
                detailedFavorites.Add((favorite, movie));
            }

            ViewBag.Favorites = detailedFavorites;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalFavorites / pageSize);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(string movieId, string movieTitle)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            if (await _context.Favorites.AnyAsync(f => f.UserId == user.Id && f.MovieId == movieId))
            {
                return RedirectToAction("Index", "Favorites");
            }

            var favorite = new Favorite
            {
                UserId = user.Id,
                MovieId = movieId,
                MovieTitle = movieTitle
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Favorites");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int favoriteId)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == favoriteId && f.UserId == user.Id);
            if (favorite == null) return NotFound();

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Favorites");
        }

        [HttpPost]
        public async Task<IActionResult> Update(int favoriteId, bool watched, int? rating)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == favoriteId && f.UserId == user.Id);
            if (favorite == null) return NotFound();

            favorite.Watched = watched;
            favorite.Rating = rating;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Favorites");
        }
    }
}
