using GameBacklog.Data;
using GameBacklog.Models;
using GameBacklog.Services;
using GameBacklog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace GameBacklog.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRawgService _rawgService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GamesController(ApplicationDbContext context, IRawgService rawgService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _rawgService = rawgService;
            _userManager = userManager;
        }

        // GET: Games
        public async Task<IActionResult> Index(string? searchString, GameStatus? statusFilter)
        {
            var userId = _userManager.GetUserId(User);
            var games = _context.Games.AsNoTracking().Where(g => g.UserId == userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString.ToLower();
                games = games.Where(g => g.Title.ToLower().Contains(search));
            }

            if (statusFilter.HasValue)
            {
                games = games.Where(g => g.Status == statusFilter.Value);
            }

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentStatus"] = statusFilter;

            return View(await games.OrderByDescending(g => g.DateAdded).ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var game = await _context.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Platform,Genre,Status,Rating,DateAdded,Notes,CoverImageUrl,RawgId")] Game game)
        {
            game.UserId = _userManager.GetUserId(User)!;
            ModelState.Remove(nameof(Game.UserId));  // remove from validation since we just set it

            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Platform,Genre,Status,Rating,DateAdded,Notes,CoverImageUrl,RawgId")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var existingGame = await _context.Games.AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (existingGame == null)
            {
                return NotFound();  // doesn't exist or isn't yours
            }

            game.UserId = userId!;  // preserve the correct owner
            ModelState.Remove(nameof(Game.UserId));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GameExistsForUserAsync(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var game = await _context.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> GameExistsForUserAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            return await _context.Games
                .AsNoTracking()
                .AnyAsync(e => e.Id == id && e.UserId == userId);
        }

        // GET: Games/Stats
        public async Task<IActionResult> Stats()
        {
            var userId = _userManager.GetUserId(User);
            var query = _context.Games.AsNoTracking().Where(g => g.UserId == userId);

            // Aggregated counts + average rating - single roundtrip
            var basicStats = await query
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    WantToPlay = g.Count(x => x.Status == GameStatus.WantToPlay),
                    Playing = g.Count(x => x.Status == GameStatus.Playing),
                    Completed = g.Count(x => x.Status == GameStatus.Completed),
                    Dropped = g.Count(x => x.Status == GameStatus.Dropped),
                    AvgRating = g.Where(x => x.Rating.HasValue).Average(x => (double?)x.Rating)
                })
                .FirstOrDefaultAsync();

            // Top genre - separate query
            var topGenre = await query
                .Where(g => !string.IsNullOrEmpty(g.Genre))
                .GroupBy(g => g.Genre!)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            // Top platform - separate query
            var topPlatform = await query
                .GroupBy(g => g.Platform)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            // Breakdown by platform - for table display
            var gamesByPlatform = await query
                .GroupBy(g => g.Platform)
                .Select(g => new { Platform = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Platform, x => x.Count);

            // Breakdown by genre
            var gamesByGenre = await query
                .Where(g => !string.IsNullOrEmpty(g.Genre))
                .GroupBy(g => g.Genre!)
                .Select(g => new { Genre = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Genre, x => x.Count);

            var stats = new StatsViewModel
            {
                TotalGames = basicStats?.Total ?? 0,
                WantToPlayCount = basicStats?.WantToPlay ?? 0,
                PlayingCount = basicStats?.Playing ?? 0,
                CompletedCount = basicStats?.Completed ?? 0,
                DroppedCount = basicStats?.Dropped ?? 0,
                AverageRating = basicStats?.AvgRating,
                TopGenre = topGenre,
                TopPlatform = topPlatform,
                GamesByPlatform = gamesByPlatform,
                GamesByGenre = gamesByGenre
            };

            return View(stats);
        }
        // GET: Games/SearchRawg?query=witcher
        [HttpGet]
        public async Task<IActionResult> SearchRawg(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(new List<object>());
            }

            var results = await _rawgService.SearchGamesAsync(query);

            var simplified = results.Select(g => new
            {
                rawgId = g.Id,
                name = g.Name,
                released = g.Released,
                coverImage = g.BackgroundImage,
                genre = g.Genres.FirstOrDefault()?.Name
            });

            return Json(simplified);
        }
    }
}

