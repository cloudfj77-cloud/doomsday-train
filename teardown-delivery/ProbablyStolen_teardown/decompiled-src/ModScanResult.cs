using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ModScanResult
{
	public HashSet<string> Capabilities = new HashSet<string>();

	public List<ModScanFinding> Findings = new List<ModScanFinding>();

	public List<ModScanFinding> HardFlags = new List<ModScanFinding>();

	public string Error;

	public int MethodsScanned;

	public bool Refused
	{
		get
		{
			if (Error == null)
			{
				return HardFlags.Count > 0;
			}
			return true;
		}
	}

	public string FormatFindings(int max = 12)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		foreach (ModScanFinding item in HardFlags.Concat(Findings))
		{
			if (num++ >= max)
			{
				stringBuilder.Append('\n').Append("...");
				break;
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append('\n');
			}
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}
}
