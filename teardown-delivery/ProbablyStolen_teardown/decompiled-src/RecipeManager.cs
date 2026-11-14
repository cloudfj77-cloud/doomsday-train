using System.Collections.Generic;

public abstract class RecipeManager
{
	public List<CraftingRecipe> availableRecipes { get; }

	public RecipeManager()
	{
		availableRecipes = new List<CraftingRecipe>();
	}

	public RecipeManager AddRecipe(CraftingRecipe recipe)
	{
		availableRecipes.Add(recipe);
		return this;
	}
}
