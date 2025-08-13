using CalorieTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public LogsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("{date}")]
        public async Task<ActionResult<object>> GetLog(string date)
        {
            if (!DateOnly.TryParse(date, out var day)) return BadRequest("Invalid date");
            var userId = GetUserId();
            var log = await _dbContext.DailyLogs
                .Include(l => l.Entries)
                .ThenInclude(e => e.Food!)
                .Include(l => l.Entries)
                .ThenInclude(e => e.Dish!)
                .ThenInclude(d => d.Items)
                .ThenInclude(i => i.Food)
                .FirstOrDefaultAsync(l => l.UserId == userId && l.Date == day);

            if (log == null)
            {
                return Ok(new { date = day, totalCalories = 0, entries = Array.Empty<object>() });
            }

            var entries = log.Entries.Select(e => new
            {
                e.Id,
                e.FoodId,
                e.DishId,
                e.Grams,
                calories = CalculateEntryCalories(e)
            }).ToList();
            var totalCalories = entries.Sum(e => e.calories);
            return Ok(new { date = day, totalCalories, entries });
        }

        [HttpPost("{date}/entries")]
        public async Task<ActionResult> AddEntry(string date, [FromBody] DailyLogEntry entry)
        {
            if (!DateOnly.TryParse(date, out var day)) return BadRequest("Invalid date");
            var userId = GetUserId();

            var log = await _dbContext.DailyLogs.FirstOrDefaultAsync(l => l.UserId == userId && l.Date == day);
            if (log == null)
            {
                log = new DailyLog { UserId = userId, Date = day };
                _dbContext.DailyLogs.Add(log);
                await _dbContext.SaveChangesAsync();
            }

            entry.Id = 0;
            entry.DailyLogId = log.Id;
            _dbContext.DailyLogEntries.Add(entry);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("entries/{id}")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            var userId = GetUserId();
            var entry = await _dbContext.DailyLogEntries
                .Include(e => e.DailyLog)
                .FirstOrDefaultAsync(e => e.Id == id && e.DailyLog.UserId == userId);
            if (entry == null) return NotFound();
            _dbContext.DailyLogEntries.Remove(entry);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private int CalculateEntryCalories(DailyLogEntry entry)
        {
            if (entry.Food != null)
            {
                return (int)Math.Round(entry.Grams * entry.Food.CaloriesPer100g / 100.0);
            }
            if (entry.Dish != null)
            {
                var total = 0;
                foreach (var item in entry.Dish.Items)
                {
                    total += (int)Math.Round(item.Grams * item.Food.CaloriesPer100g / 100.0);
                }
                // Scale by grams consumed vs sum of item grams
                var dishWeight = entry.Dish.Items.Sum(i => i.Grams);
                if (dishWeight <= 0) return 0;
                return (int)Math.Round(total * (entry.Grams / (double)dishWeight));
            }
            return 0;
        }
    }
}