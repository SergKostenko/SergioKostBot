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
    public class FoodsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public FoodsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetMyFoods()
        {
            var userId = GetUserId();
            var foods = await _dbContext.Foods.Where(f => f.UserId == userId).ToListAsync();
            return Ok(foods);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            var userId = GetUserId();
            var food = await _dbContext.Foods.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            if (food == null) return NotFound();
            return Ok(food);
        }

        [HttpPost]
        public async Task<ActionResult<Food>> CreateFood(Food food)
        {
            food.Id = 0;
            food.UserId = GetUserId();
            _dbContext.Foods.Add(food);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFood), new { id = food.Id }, food);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFood(int id, Food update)
        {
            var userId = GetUserId();
            var food = await _dbContext.Foods.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            if (food == null) return NotFound();

            food.Name = update.Name;
            food.CaloriesPer100g = update.CaloriesPer100g;
            food.ProteinPer100g = update.ProteinPer100g;
            food.CarbsPer100g = update.CarbsPer100g;
            food.FatPer100g = update.FatPer100g;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var userId = GetUserId();
            var food = await _dbContext.Foods.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            if (food == null) return NotFound();
            _dbContext.Foods.Remove(food);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}