public class RichText
{
	public string nakedText { get; private set; }

	public string richTextStrikethrough { get; private set; }

	public string richTextUnderline { get; private set; }

	public string richTextForeground { get; private set; }

	public string richTextBackground { get; private set; }

	public int recommendedWidth { get; private set; }

	public int recommendedHeight { get; private set; }

	public bool isTruncated { get; private set; }

	public RichText(string nakedText, string richTextStrikethrough, string richTextUnderline, string richTextForeground, string richTextBackground, int recommendedWidth, int recommendedHeight, bool isTruncated)
	{
		this.nakedText = nakedText;
		this.richTextStrikethrough = richTextStrikethrough;
		this.richTextUnderline = richTextUnderline;
		this.richTextForeground = richTextForeground;
		this.richTextBackground = richTextBackground;
		this.recommendedWidth = recommendedWidth;
		this.recommendedHeight = recommendedHeight;
		this.isTruncated = isTruncated;
	}

	public override int GetHashCode()
	{
		return recommendedWidth + recommendedHeight + (isTruncated ? 1 : 0) + nakedText.GetHashCode() + richTextStrikethrough.GetHashCode() + richTextUnderline.GetHashCode() + richTextForeground.GetHashCode() + richTextBackground.GetHashCode();
	}

	public bool QuickEquals(RichText otherRichText)
	{
		return GetHashCode() == otherRichText?.GetHashCode();
	}
}
