public static class GameRules
{
    public const int IngredientsPerBurger = 3;
    public const int PointsPerIngredient  = 100;
    public const int PointsPerHeart       = 10;
    public const int PointsPerStar        = 50;
    public const int PointsPerPriceUnit   = 40;

    
    public static int CalculatePrice(int points) => points / PointsPerPriceUnit;
}
