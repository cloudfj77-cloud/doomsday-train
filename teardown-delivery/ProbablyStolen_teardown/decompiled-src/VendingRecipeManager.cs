public class VendingRecipeManager : RecipeManager
{
	public RecipeManager AddVendingAnyEnergy(int energy, string outputIdentifier, int outputAmount = 1, int stock = -1)
	{
		CraftingRecipe recipe = new CraftingRecipe().AddToolTag("Any Battery", energy, "power_source_item").SetCraftingConstantFunc(outputIdentifier, outputAmount).SetRemainingCraftAmount(stock);
		AddRecipe(recipe);
		return this;
	}
}
