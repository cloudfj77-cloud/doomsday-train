public class CraftingRecipeManager : RecipeManager
{
	public RecipeManager AddRecipe(string outputIdentifier, string materialIdentifier, int materialAmount = 1)
	{
		return AddRecipe(outputIdentifier, 1, materialIdentifier, materialAmount);
	}

	public RecipeManager AddRecipe(string outputIdentifier, int outputAmount, string materialIdentifier, int materialAmount = 1)
	{
		AddRecipe(new CraftingRecipe().AddMaterialIdentifier(materialIdentifier, materialAmount).SetCraftingConstantFunc(outputIdentifier, outputAmount));
		return this;
	}

	public RecipeManager AddRecipe(string outputIdentifier, CraftingFilter materialTagsFilter, params CraftingFilter[] otherMaterialTagsFilter)
	{
		return AddRecipe(outputIdentifier, 1, materialTagsFilter, otherMaterialTagsFilter);
	}

	public RecipeManager AddRecipe(string outputIdentifier, int outputAmount, CraftingFilter materialTagsFilter, params CraftingFilter[] otherMaterialTagsFilter)
	{
		CraftingRecipe craftingRecipe = new CraftingRecipe().AddMaterial(materialTagsFilter).SetCraftingConstantFunc(outputIdentifier, outputAmount);
		for (int i = 0; i < otherMaterialTagsFilter.Length; i++)
		{
			CraftingTagsFilter filter = (CraftingTagsFilter)otherMaterialTagsFilter[i];
			craftingRecipe.AddMaterial(filter);
		}
		AddRecipe(craftingRecipe);
		return this;
	}
}
