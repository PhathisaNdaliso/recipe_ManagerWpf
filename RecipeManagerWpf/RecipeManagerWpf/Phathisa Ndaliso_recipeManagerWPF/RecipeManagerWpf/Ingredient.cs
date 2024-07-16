public class Ingredient
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public string UnitOfMeasurement { get; set; }
    public int Calories { get; set; }
    public string FoodGroup { get; set; }
    public int OriginalQuantity { get; set; }

    public Ingredient(string name, int quantity, string unitOfMeasurement, int calories, string foodGroup)
    {
        Name = name;
        Quantity = quantity;
        UnitOfMeasurement = unitOfMeasurement;
        Calories = calories;
        FoodGroup = foodGroup;
        OriginalQuantity = quantity;
    }

    public void ResetQuantity()
    {
        Quantity = OriginalQuantity;
    }

    public override string ToString()
    {
        return $"{Quantity} {UnitOfMeasurement} of {Name} ({Calories} cal, {FoodGroup})";
    }
}
