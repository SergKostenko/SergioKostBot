namespace CalorieTracker.Api.Models
{
    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CaloriesPer100g { get; set; }
        public int ProteinPer100g { get; set; } // grams
        public int CarbsPer100g { get; set; }   // grams
        public int FatPer100g { get; set; }     // grams
        public string UserId { get; set; } = string.Empty; // owner
    }

    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty; // owner
        public ICollection<DishItem> Items { get; set; } = new List<DishItem>();
    }

    public class DishItem
    {
        public int Id { get; set; }
        public int DishId { get; set; }
        public Dish Dish { get; set; } = null!;
        public int FoodId { get; set; }
        public Food Food { get; set; } = null!;
        public int Grams { get; set; }
    }

    public class DailyLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public ICollection<DailyLogEntry> Entries { get; set; } = new List<DailyLogEntry>();
    }

    public class DailyLogEntry
    {
        public int Id { get; set; }
        public int DailyLogId { get; set; }
        public DailyLog DailyLog { get; set; } = null!;
        public int? FoodId { get; set; }
        public Food? Food { get; set; }
        public int? DishId { get; set; }
        public Dish? Dish { get; set; }
        public int Grams { get; set; }
    }
}