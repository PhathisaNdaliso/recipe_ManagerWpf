using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RecipeManagerWpf
{
    public partial class MainWindow : Window
    {
        private Recipe currentRecipe;

        public MainWindow()
        {
            InitializeComponent();
            LoadRecipes();
        }

        private void AddIngredients_Click(object sender, RoutedEventArgs e)
        {
            int numIngredients;
            if (!int.TryParse(NumIngredientsTextBox.Text, out numIngredients) || numIngredients <= 0)
            {
                MessageBox.Show("Please enter a valid number of ingredients.");
                return;
            }

            IngredientsPanel.Items.Clear();
            for (int i = 0; i < numIngredients; i++)
            {
                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                var nameTextBox = new TextBox { Width = 100, Margin = new Thickness(5), Tag = "Name" };
                nameTextBox.GotFocus += RemovePlaceholderText;
                nameTextBox.LostFocus += AddPlaceholderText;
                AddPlaceholderText(nameTextBox, null);

                var quantityTextBox = new TextBox { Width = 50, Margin = new Thickness(5), Tag = "Quantity" };
                quantityTextBox.GotFocus += RemovePlaceholderText;
                quantityTextBox.LostFocus += AddPlaceholderText;
                AddPlaceholderText(quantityTextBox, null);

                var unitTextBox = new TextBox { Width = 50, Margin = new Thickness(5), Tag = "Unit" };
                unitTextBox.GotFocus += RemovePlaceholderText;
                unitTextBox.LostFocus += AddPlaceholderText;
                AddPlaceholderText(unitTextBox, null);

                var caloriesTextBox = new TextBox { Width = 50, Margin = new Thickness(5), Tag = "Calories" };
                caloriesTextBox.GotFocus += RemovePlaceholderText;
                caloriesTextBox.LostFocus += AddPlaceholderText;
                AddPlaceholderText(caloriesTextBox, null);

                var foodGroupComboBox = new ComboBox { Width = 100, Margin = new Thickness(5) };
                foodGroupComboBox.Items.Add("Fruits and Vegetables");
                foodGroupComboBox.Items.Add("Proteins");
                foodGroupComboBox.Items.Add("Grains");
                foodGroupComboBox.Items.Add("Dairy");
                foodGroupComboBox.Items.Add("Carbohydrates");
                foodGroupComboBox.Items.Add("Fats and sugars");
                foodGroupComboBox.Items.Add("Drinks");

                stackPanel.Children.Add(nameTextBox);
                stackPanel.Children.Add(quantityTextBox);
                stackPanel.Children.Add(unitTextBox);
                stackPanel.Children.Add(caloriesTextBox);
                stackPanel.Children.Add(foodGroupComboBox);

                IngredientsPanel.Items.Add(stackPanel);
            }
        }

        private void RemovePlaceholderText(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddPlaceholderText(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = txtRecipeName.Text;
            string steps = txtSteps.Text;

            if (string.IsNullOrWhiteSpace(recipeName) || string.IsNullOrWhiteSpace(steps))
            {
                MessageBox.Show("Please fill in the recipe name and steps.");
                return;
            }

            Recipe newRecipe = new Recipe(recipeName);

            foreach (var item in IngredientsPanel.Items)
            {
                var stackPanel = item as StackPanel;
                string name = (stackPanel.Children[0] as TextBox).Text;
                if (!int.TryParse((stackPanel.Children[1] as TextBox).Text, out int quantity) ||
                    !int.TryParse((stackPanel.Children[3] as TextBox).Text, out int calories))
                {
                    MessageBox.Show("Please enter valid quantity and calories.");
                    return;
                }

                string unit = (stackPanel.Children[2] as TextBox).Text;
                string foodGroup = (stackPanel.Children[4] as ComboBox).SelectedItem.ToString();

                newRecipe.AddIngredient(name, quantity, unit, calories, foodGroup);
            }

            string[] stepLines = steps.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string step in stepLines)
            {
                newRecipe.AddStep(step.Trim());
            }

            Recipe.Recipes.Add(newRecipe);
            MessageBox.Show("Recipe added successfully!");

            LoadRecipes();
        }

        private void DisplayRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipesListBox.SelectedItem != null)
            {
                string recipeName = RecipesListBox.SelectedItem.ToString();
                Recipe recipe = Recipe.Recipes.FirstOrDefault(r => r.RecipeName == recipeName);
                if (recipe != null)
                {
                    MessageBox.Show(recipe.ToString());
                }
            }
        }

        private void ScaleRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem == null || ScaleFactorComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a recipe and a scale factor.");
                return;
            }

            string recipeName = RecipeComboBox.SelectedItem.ToString();
            double scaleFactor = Convert.ToDouble((ScaleFactorComboBox.SelectedItem as ComboBoxItem).Content);

            Recipe recipe = Recipe.Recipes.FirstOrDefault(r => r.RecipeName == recipeName);
            if (recipe != null)
            {
                Recipe scaledRecipe = recipe.ScaleRecipe(scaleFactor);
                ScaledRecipeTextBox.Text = scaledRecipe.ToString();
            }
        }

        private void ResetRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (ResetRecipeComboBox.SelectedItem != null)
            {
                string recipeName = ResetRecipeComboBox.SelectedItem.ToString();
                Recipe recipe = Recipe.Recipes.FirstOrDefault(r => r.RecipeName == recipeName);
                if (recipe != null)
                {
                    recipe.ResetQuantities();
                    MessageBox.Show($"Recipe {recipeName} reset to original quantities.");
                }
            }
        }

        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteRecipeComboBox.SelectedItem != null)
            {
                string recipeName = DeleteRecipeComboBox.SelectedItem.ToString();
                Recipe recipe = Recipe.Recipes.FirstOrDefault(r => r.RecipeName == recipeName);
                if (recipe != null)
                {
                    Recipe.Recipes.Remove(recipe);
                    MessageBox.Show($"Recipe {recipeName} deleted.");
                    LoadRecipes();
                }
            }
        }

        private void DeleteAllRecipes_Click(object sender, RoutedEventArgs e)
        {
            Recipe.Recipes.Clear();
            MessageBox.Show("All recipes deleted.");
            LoadRecipes();
        }

        private void LoadRecipes()
        {
            RecipesListBox.ItemsSource = Recipe.Recipes.Select(r => r.RecipeName).ToList();
            RecipeComboBox.ItemsSource = Recipe.Recipes.Select(r => r.RecipeName).ToList();
            ResetRecipeComboBox.ItemsSource = Recipe.Recipes.Select(r => r.RecipeName).ToList();
            DeleteRecipeComboBox.ItemsSource = Recipe.Recipes.Select(r => r.RecipeName).ToList();
        }

        private void FilterByIngredient_Click(object sender, RoutedEventArgs e)
        {
            string filter = txtFilter.Text;
            var filteredRecipes = FilterRecipesByIngredient(filter);
            DisplayFilteredRecipes(filteredRecipes);
        }

        private void FilterByFoodGroup_Click(object sender, RoutedEventArgs e)
        {
            string filter = txtFilter.Text;
            var filteredRecipes = FilterRecipesByFoodGroup(filter);
            DisplayFilteredRecipes(filteredRecipes);
        }

        private void FilterByCalories_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtFilter.Text, out int filter))
            {
                var filteredRecipes = FilterRecipesByCalories(filter);
                DisplayFilteredRecipes(filteredRecipes);
            }
            else
            {
                MessageBox.Show("Please enter a valid number for calories.");
            }
        }

        private List<Recipe> FilterRecipesByIngredient(string ingredient)
        {
            return Recipe.Recipes.Where(r => r.Ingredients.Any(i => i.Name.IndexOf(ingredient, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
        }

        private List<Recipe> FilterRecipesByFoodGroup(string foodGroup)
        {
            return Recipe.Recipes.Where(r => r.Ingredients.Any(i => i.FoodGroup.IndexOf(foodGroup, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
        }

        private List<Recipe> FilterRecipesByCalories(int maxCalories)
        {
            return Recipe.Recipes.Where(r => r.Ingredients.Sum(i => i.Calories) <= maxCalories).ToList();
        }

        private void DisplayFilteredRecipes(List<Recipe> filteredRecipes)
        {
            FilteredRecipesListBox.ItemsSource = filteredRecipes.Select(r => r.ToString()).ToList();
        }
    }

    public class Recipe
    {
        public static List<Recipe> Recipes { get; } = new List<Recipe>();

        public string RecipeName { get; }
        public List<Ingredient> Ingredients { get; }
        public List<string> Steps { get; }

        public Recipe(string recipeName)
        {
            RecipeName = recipeName;
            Ingredients = new List<Ingredient>();
            Steps = new List<string>();
        }

        public void AddIngredient(string name, int quantity, string unitOfMeasurement, int calories, string foodGroup)
        {
            Ingredients.Add(new Ingredient(name, quantity, unitOfMeasurement, calories, foodGroup));
        }

        public void AddStep(string step)
        {
            Steps.Add(step);
        }

        public Recipe ScaleRecipe(double factor)
        {
            Recipe scaledRecipe = new Recipe($"{RecipeName} (Scaled by {factor})");
            foreach (var ingredient in Ingredients)
            {
                scaledRecipe.AddIngredient(ingredient.Name, (int)(ingredient.Quantity * factor), ingredient.UnitOfMeasurement, ingredient.Calories, ingredient.FoodGroup);
            }

            scaledRecipe.Steps.AddRange(Steps);
            return scaledRecipe;
        }

        public void ResetQuantities()
        {
            foreach (var ingredient in Ingredients)
            {
                ingredient.Quantity = ingredient.originalQuantity;
            }
        }

        public override string ToString()
        {
            string ingredients = string.Join(Environment.NewLine, Ingredients);
            string steps = string.Join(Environment.NewLine, Steps.Select((step, index) => $"Step {index + 1}: {step}"));
            return $"Recipe: {RecipeName}\n\nIngredients:\n{ingredients}\n\nSteps:\n{steps}";
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string UnitOfMeasurement { get; set; }
        public int Calories { get; set; }
        public string FoodGroup { get; set; }
        public int originalQuantity { get; set; }

        public Ingredient(string name, int quantity, string unitOfMeasurement, int calories, string foodGroup)
        {
            Name = name;
            Quantity = quantity;
            UnitOfMeasurement = unitOfMeasurement;
            Calories = calories;
            FoodGroup = foodGroup;
            originalQuantity = quantity;
        }

        public override string ToString()
        {
            return $"{Quantity} {UnitOfMeasurement} of {Name} ({Calories} cal, {FoodGroup})";
        }
    }
}
