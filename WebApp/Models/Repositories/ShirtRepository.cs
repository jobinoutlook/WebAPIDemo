namespace WebApp.Models.Repositories
{
    public static class ShirtRepository
    {
        private static List<Shirt> shirts = new List<Shirt>()
        {
            new Shirt { ShirtId = 1, Brand = "My Brand", Color = "Blue", Gender = "Men", Price = 30, Size = 10 },
            new Shirt { ShirtId = 2, Brand = "My Brand", Color = "Black", Gender = "Men", Price = 35, Size = 12 },
            new Shirt { ShirtId = 3, Brand = "Your Brand", Color = "Pink", Gender = "Women", Price = 28, Size = 8 },
            new Shirt { ShirtId = 4, Brand = "Your Brand", Color = "Yello", Gender = "Women", Price = 30, Size = 9 }
        };

        public static bool ShirtExists(int id)
        {
            return shirts.Any(x=>x.ShirtId == id);
        }

        public static Shirt? GetShirtById(int id)
        {
            return shirts.FirstOrDefault(x => x.ShirtId == id);
        }

        public static List<Shirt> GetShirts()
        {
            return shirts;
        }

        public static Shirt? GetShirtByProperties(string? brand,string? gender,string? color,int? size)
        {
            return shirts.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(brand) &&
                !string.IsNullOrWhiteSpace(x.Brand) &&
                x.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&

                !string.IsNullOrWhiteSpace(gender) &&
                !string.IsNullOrWhiteSpace(x.Gender) &&
                x.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase) &&

                !string.IsNullOrWhiteSpace(color) &&
                !string.IsNullOrWhiteSpace(x.Color) &&
                x.Color.Equals(color, StringComparison.OrdinalIgnoreCase) &&
               
                size.HasValue &&
                x.Size.HasValue &&
                size.Value == x.Size.Value);

        }

        public static void AddShirt(Shirt shirt)
        {
            int id = shirts.Max(s => s.ShirtId);
            shirt.ShirtId = id + 1;

            shirts.Add(shirt);
        }

        public static void UpdateShirt(Shirt shirt)
        {
            
                Shirt? shirt_update = shirts.First(x => x.ShirtId == shirt.ShirtId);
                shirt_update?.Brand = shirt.Brand;
                shirt_update?.Price = shirt.Price;
                shirt_update?.Size = shirt.Size;
                shirt_update?.Color = shirt.Color;
                shirt_update?.Gender = shirt.Gender;
            
        }

        public static bool DeleteShirt(int shirtId)
        {
            Shirt? shirt = GetShirtById(shirtId);

            if (shirt != null)
            {
                shirts.Remove(shirt);
                return true;
            }

            return false;
        }
    }
}
