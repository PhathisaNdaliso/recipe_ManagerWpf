using System.Collections.Generic;
using System.Text;

public class Recipe
{
    public string RecipeName { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public List<string> Steps { get; set; }
    public static List<Recipe> Recipes { get; set; } = new List<Recipe>();

    public Recipe(string recipeName)
    {
        RecipeName = recipeName;
        Ingredients = new List<Ingredient>();
        Steps = new List<string>();
    }

    public void AddIngredient(string name, int quantity, string unit, int calories, string foodGroup)
    {
        Ingredients.Add(new Ingredient(name, quantity, unit, calories, foodGroup));
    }

    public void AddStep(string step)
    {
        Steps.Add(step);
    }

    public void ScaleRecipe(double factor)
    {
        foreach (var ingredient in Ingredients)
        {
            ingredient.Quantity = (int)(ingredient.OriginalQuantity * factor);
        }
    }

    public void ResetRecipe()
    {
        foreach (var ingredient in Ingredients)
        {
            ingredient.ResetQuantity();
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Recipe: {RecipeName}");
        sb.AppendLine("Ingredients:");
        foreach (var ingredient in Ingredients)
        {
            sb.AppendLine(ingredient.ToString());
        }
        sb.AppendLine("Steps:");
        for (int i = 0; i < Steps.Count; i++)
        {
            sb.AppendLine($"Step {i + 1}: {Steps[i]}");
        }
        return sb.ToString();
    }
}
