using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class ModFingerprint
{
	public string ManifestHash;

	public List<(string path, string hash)> Files = new List<(string, string)>();

	public string Combined;

	public static ModFingerprint Compute(string folderPath, ModManifest manifest)
	{
		ModFingerprint modFingerprint = new ModFingerprint();
		modFingerprint.ManifestHash = HashFile(Path.Combine(folderPath, "manifest.xml"));
		foreach (string assembly in manifest.Assemblies)
		{
			string path = Path.Combine(folderPath, assembly);
			modFingerprint.Files.Add((assembly, File.Exists(path) ? HashFile(path) : "missing"));
		}
		StringBuilder stringBuilder = new StringBuilder(modFingerprint.ManifestHash);
		foreach (var (value, value2) in modFingerprint.Files)
		{
			stringBuilder.Append('\n').Append(value).Append('|')
				.Append(value2);
		}
		modFingerprint.Combined = HashBytes(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
		return modFingerprint;
	}

	public List<string> FileEntries()
	{
		return Files.Select(((string path, string hash) f) => f.path + "|" + f.hash).ToList();
	}

	public static string HashFile(string path)
	{
		using FileStream inputStream = File.OpenRead(path);
		using SHA256 sHA = SHA256.Create();
		return ToHex(sHA.ComputeHash(inputStream));
	}

	public static string HashBytes(byte[] bytes)
	{
		using SHA256 sHA = SHA256.Create();
		return ToHex(sHA.ComputeHash(bytes));
	}

	private static string ToHex(byte[] bytes)
	{
		StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
		foreach (byte b in bytes)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
