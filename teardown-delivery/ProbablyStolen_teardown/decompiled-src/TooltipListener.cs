public interface TooltipListener : PixelElement
{
	bool HasTooltip();

	RichText GetTooltip();

	RichText GetTooltipAdvanced();
}
