using GameBacklog.Data;
using GameBacklog.Models;
using GameBacklog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameBacklog.ViewModels;

namespace GameBacklog.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Games
        // GET: Games
        public async Task<IActionResult> Index(string? searchString, GameStatus? statusFilter)
        {
            var games = _context.Games.AsQueryable();

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

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Create([Bind("Id,Title,Platform,Genre,Status,Rating,DateAdded,Notes")] Game game)
        {
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

            var game = await _context.Games.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Platform,Genre,Status,Rating,DateAdded,Notes")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
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

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    
    // GET: Games/Stats
public async Task<IActionResult> Stats()
        {
            var games = await _context.Games.ToListAsync();

            var stats = new StatsViewModel
            {
                TotalGames = games.Count,
                WantToPlayCount = games.Count(g => g.Status == GameStatus.WantToPlay),
                PlayingCount = games.Count(g => g.Status == GameStatus.Playing),
                CompletedCount = games.Count(g => g.Status == GameStatus.Completed),
                DroppedCount = games.Count(g => g.Status == GameStatus.Dropped),
                AverageRating = games.Where(g => g.Rating.HasValue)
                                     .Select(g => (double)g.Rating!.Value)
                                     .DefaultIfEmpty()
                                     .Average(),
                TopGenre = games.Where(g => !string.IsNullOrEmpty(g.Genre))
                                .GroupBy(g => g.Genre!)
                                .OrderByDescending(g => g.Count())
                                .Select(g => g.Key)
                                .FirstOrDefault(),
                TopPlatform = games.GroupBy(g => g.Platform)
                                   .OrderByDescending(g => g.Count())
                                   .Select(g => g.Key)
                                   .FirstOrDefault(),
                GamesByPlatform = games.GroupBy(g => g.Platform)
                                       .ToDictionary(g => g.Key, g => g.Count()),
                GamesByGenre = games.Where(g => !string.IsNullOrEmpty(g.Genre))
                                    .GroupBy(g => g.Genre!)
                                    .ToDictionary(g => g.Key, g => g.Count())
            };

            // Ak nie sú žiadne hry s hodnotením, AverageRating je 0
            if (!games.Any(g => g.Rating.HasValue))
            {
                stats.AverageRating = null;
            }

            return View(stats);
        }
    }
}
