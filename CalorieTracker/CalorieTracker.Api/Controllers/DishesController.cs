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
    public class DishesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public DishesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetMyDishes()
        {
            var userId = GetUserId();
            var dishes = await _dbContext.Dishes
                .Where(d => d.UserId == userId)
                .Include(d => d.Items)
                .ThenInclude(i => i.Food)
                .ToListAsync();
            return Ok(dishes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> GetDish(int id)
        {
            var userId = GetUserId();
            var dish = await _dbContext.Dishes
                .Include(d => d.Items)
                .ThenInclude(i => i.Food)
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (dish == null) return NotFound();
            return Ok(dish);
        }

        [HttpPost]
        public async Task<ActionResult<Dish>> CreateDish(Dish dish)
        {
            dish.Id = 0;
            dish.UserId = GetUserId();
            foreach (var item in dish.Items)
            {
                item.Id = 0;
            }
            _dbContext.Dishes.Add(dish);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, dish);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDish(int id, Dish update)
        {
            var userId = GetUserId();
            var dish = await _dbContext.Dishes
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (dish == null) return NotFound();

            dish.Name = update.Name;

            // Replace items
            _dbContext.DishItems.RemoveRange(dish.Items);
            dish.Items = update.Items.Select(i => new DishItem
            {
                FoodId = i.FoodId,
                Grams = i.Grams
            }).ToList();
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var userId = GetUserId();
            var dish = await _dbContext.Dishes.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (dish == null) return NotFound();
            _dbContext.Dishes.Remove(dish);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}