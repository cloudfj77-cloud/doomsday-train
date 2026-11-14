using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DseCecil;
using DseCecil.Cil;
using HarmonyLib;
using Mono.Collections.Generic;
using UnityEngine;

public static class ModScanner
{
	private sealed class Pass
	{
		private readonly ModScanResult result;

		private readonly AssemblyDefinition assembly;

		private readonly string fileName;

		private readonly HashSet<string> allowedReferences;

		private static readonly string[] StealerPatterns = new string[24]
		{
			"local storage\\leveldb", "local storage/leveldb", "\\login data", "/login data", "\\web data", "/web data", "\\cookies", "/cookies", "loginusers.vdf", "ssfn",
			"\\discord\\", "/discord/", "discordcanary", "discordptb", "\\wallets\\", "wallet.dat", "metamask", "exodus\\", "electrum\\", "\\tdata",
			"/tdata", "config.vdf", "\\steam\\config", "/steam/config"
		};

		private static readonly string[] FileTypes = new string[19]
		{
			"System.IO.File", "System.IO.FileStream", "System.IO.Directory", "System.IO.FileInfo", "System.IO.DirectoryInfo", "System.IO.FileSystemInfo", "System.IO.FileSystemWatcher", "System.IO.DriveInfo", "System.IO.Compression.ZipFile", "UnityEngine.Windows.File",
			"UnityEngine.Windows.Directory", "UnityEngine.ScreenCapture", "Microsoft.Win32.Registry", "Microsoft.Win32.RegistryKey", "ES3File", "ES3Writer", "ES3Reader", "ES3Stream", "ES3Directory"
		};

		private static readonly string[] FilePrefixes = new string[2] { "System.IO.IsolatedStorage.", "System.IO.MemoryMappedFiles." };

		private static readonly string[] AssetBundlePathMethods = new string[3] { "LoadFromFile", "LoadFromFileAsync", "RecompressAssetBundleAsync" };

		private static readonly string[] ES3PathMethods = new string[23]
		{
			"SaveRaw", "AppendRaw", "LoadRawBytes", "LoadRawString", "SaveImage", "LoadImage", "LoadAudio", "CopyFile", "CopyDirectory", "RenameFile",
			"RenameDirectory", "DeleteFile", "DeleteDirectory", "FileExists", "DirectoryExists", "GetFiles", "GetDirectories", "CreateBackup", "RestoreBackup", "GetTimestamp",
			"StoreCachedFile", "CacheFile", "Init"
		};

		private static readonly string[] SteamAuthHelperMethods = new string[2] { "RequestSteamAuthTicket", "CancelSteamAuthTicket" };

		private static readonly string[] TrustKeys = new string[4] { "modTrust", "modOrder", "workshopOwners", "modUiWarningAcceptedVersion" };

		private static readonly string[] PathStringTypes = new string[5] { "System.Xml.Linq.XDocument", "System.Xml.Linq.XElement", "System.Xml.XmlDocument", "System.Xml.XmlReader", "System.Xml.XmlWriter" };

		private static readonly string[] NetworkPrefixes = new string[6] { "System.Net.", "UnityEngine.Networking.UnityWebRequest", "Steamworks.SteamHTTP", "Steamworks.SteamNetworking", "Steamworks.SteamNetworkingSockets", "Steamworks.SteamNetworkingMessages" };

		private static readonly string[] NetworkTypes = new string[2] { "UnityEngine.WWW", "UnityEngine.Ping" };

		private static readonly string[] NetworkExceptions = new string[4] { "System.Net.WebUtility", "System.Net.IPAddress", "System.Net.IPEndPoint", "System.Net.Mime." };

		private static readonly string[] ProcessTypes = new string[2] { "System.Diagnostics.Process", "System.Diagnostics.ProcessStartInfo" };

		private static readonly string[] DynamicCodePrefixes = new string[6] { "System.Reflection.Emit.", "System.Runtime.Loader.", "System.CodeDom.", "Microsoft.CSharp.CSharpCodeProvider", "System.Runtime.Serialization.Formatters.", "System.Runtime.Serialization.NetDataContractSerializer" };

		private static readonly string[] ReflectionExternalPrefixes = new string[2] { "Microsoft.CSharp.RuntimeBinder.", "System.Runtime.CompilerServices.CallSite" };

		private static readonly string[] AssemblyLoadMethods = new string[8] { "Load", "LoadFrom", "LoadFile", "LoadWithPartialName", "ReflectionOnlyLoad", "ReflectionOnlyLoadFrom", "UnsafeLoadFrom", "LoadModule" };

		private static readonly string[] AppDomainLoadMethods = new string[8] { "Load", "DefineDynamicAssembly", "ExecuteAssembly", "ExecuteAssemblyByName", "CreateInstanceFrom", "CreateInstanceFromAndUnwrap", "CreateInstance", "CreateInstanceAndUnwrap" };

		private static readonly string[] NativeTypes = new string[4] { "System.Runtime.InteropServices.Marshal", "System.Runtime.InteropServices.NativeLibrary", "System.Runtime.CompilerServices.Unsafe", "Unity.Collections.LowLevel.Unsafe.UnsafeUtility" };

		private static readonly string[] TypeMemberLookups = new string[15]
		{
			"GetMethod", "GetMethods", "GetField", "GetFields", "GetProperty", "GetProperties", "GetMember", "GetMembers", "GetEvent", "GetEvents",
			"GetConstructor", "GetConstructors", "InvokeMember", "GetNestedType", "GetNestedTypes"
		};

		private static readonly string[] ExternalScopePrefixes = new string[12]
		{
			"mscorlib", "netstandard", "System", "Mono.", "Microsoft.", "UnityEngine", "UnityEditor", "Unity.", "0Harmony", "MonoMod",
			"Steamworks", "com.rlabrecque"
		};

		private static readonly string[] ExternalNamePrefixes = new string[10] { "System.", "Mono.", "Microsoft.", "UnityEngine.", "UnityEditor.", "Unity.", "HarmonyLib.", "MonoMod.", "Steamworks.", "ES3" };

		public Pass(ModScanResult result, AssemblyDefinition assembly, string fileName, HashSet<string> allowedReferences)
		{
			this.result = result;
			this.assembly = assembly;
			this.fileName = fileName;
			this.allowedReferences = allowedReferences;
		}

		public void Run()
		{
			string name = assembly.Name.Name;
			if (name == "0Harmony" || name.StartsWith("MonoMod", StringComparison.Ordinal) || name.StartsWith("Mono.Cecil", StringComparison.Ordinal))
			{
				Hard("bundled-framework", fileName, "assembly name " + name);
			}
			foreach (ModuleDefinition module in assembly.Modules)
			{
				foreach (AssemblyNameReference assemblyReference in module.AssemblyReferences)
				{
					if (!ModLoader.IsGameAssemblyName(assemblyReference.Name) && !allowedReferences.Contains(assemblyReference.Name))
					{
						Hard("unlisted-reference", fileName, "references " + assemblyReference.Name + ", which is not a game assembly and not listed in <Assemblies>");
					}
				}
				ScanResources(module);
				foreach (TypeDefinition type in module.GetTypes())
				{
					ScanType(type);
				}
			}
		}

		private void ScanResources(ModuleDefinition module)
		{
			foreach (Resource resource in module.Resources)
			{
				if (resource is EmbeddedResource embeddedResource)
				{
					byte[] resourceData;
					try
					{
						resourceData = embeddedResource.GetResourceData();
					}
					catch (Exception ex)
					{
						Hard("unreadable", resource.Name, ex.Message);
						continue;
					}
					CheckBlob(resource.Name, resourceData);
				}
			}
		}

		private void CheckBlob(string name, byte[] data)
		{
			if (data != null && data.Length != 0)
			{
				if (StartsWith(data, "MZ") || ContainsAscii(data, "This program cannot be run in DOS mode"))
				{
					Hard("payload-resource", name, "embedded executable");
				}
				else if (!IsKnownMedia(data) && data.Length >= 512 && Entropy(data) > 7.5)
				{
					Hard("opaque-resource", name, $"{data.Length} bytes, entropy {Entropy(data):0.00} bits/byte, unknown format");
				}
			}
		}

		internal static bool IsExecutableHeader(byte[] d)
		{
			if (StartsWith(d, "MZ") || StartsWith(d, "\u007fELF") || StartsWith(d, "#!"))
			{
				return true;
			}
			if (d.Length >= 4)
			{
				uint num = (uint)(d[0] | (d[1] << 8) | (d[2] << 16) | (d[3] << 24));
				if (num == 4277009102u || num == 4277009103u || num == 3472551422u || num == 3489328638u || num == 3199925962u)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsKnownMedia(byte[] d)
		{
			if (StartsWith(d, "\u0089PNG"))
			{
				return true;
			}
			if (StartsWith(d, "BSJB") || StartsWith(d, "Microsoft C/C++ MSF 7.00"))
			{
				return true;
			}
			if (StartsWith(d, "\0\u0001\0\0") || StartsWith(d, "OTTO") || StartsWith(d, "true") || StartsWith(d, "wOFF") || StartsWith(d, "wOF2"))
			{
				return true;
			}
			if (StartsWith(d, "UnityFS"))
			{
				return true;
			}
			if (StartsWith(d, "fLaC") || (d.Length > 8 && d[4] == 102 && d[5] == 116 && d[6] == 121 && d[7] == 112))
			{
				return true;
			}
			if (d.Length > 4 && d[0] == 26 && d[1] == 69 && d[2] == 223 && d[3] == 163)
			{
				return true;
			}
			if (d.Length > 2 && ((d[0] == byte.MaxValue && d[1] == 254) || (d[0] == 254 && d[1] == byte.MaxValue)))
			{
				return true;
			}
			if (d.Length > 3 && d[0] == byte.MaxValue && d[1] == 216 && d[2] == byte.MaxValue)
			{
				return true;
			}
			if (StartsWith(d, "GIF8"))
			{
				return true;
			}
			if (StartsWith(d, "BM"))
			{
				return true;
			}
			if (StartsWith(d, "OggS"))
			{
				return true;
			}
			if (StartsWith(d, "RIFF"))
			{
				return true;
			}
			if (StartsWith(d, "ID3") || (d.Length > 2 && d[0] == byte.MaxValue && (d[1] & 0xE0) == 224))
			{
				return true;
			}
			if (d.Length > 4 && d[0] == 206 && d[1] == 202 && d[2] == 239 && d[3] == 190)
			{
				return true;
			}
			if (StartsWith(d, "<?xml") || StartsWith(d, "{") || StartsWith(d, "[") || StartsWith(d, "<") || (d.Length > 3 && d[0] == 239 && d[1] == 187 && d[2] == 191))
			{
				return MostlyPrintable(d);
			}
			return false;
		}

		internal static bool MostlyPrintable(byte[] d, int max = 256)
		{
			int num = Math.Min(d.Length, max);
			if (num == 0)
			{
				return false;
			}
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				byte b = d[i];
				if ((b >= 32 && b < 127) || b == 10 || b == 13 || b == 9 || b >= 128)
				{
					num2++;
				}
			}
			return (double)num2 >= (double)num * 0.95;
		}

		internal static double Entropy(byte[] data)
		{
			int[] array = new int[256];
			foreach (byte b in data)
			{
				array[b]++;
			}
			double num = 0.0;
			int[] array2 = array;
			foreach (int num2 in array2)
			{
				if (num2 != 0)
				{
					double num3 = (double)num2 / (double)data.Length;
					num -= num3 * Math.Log(num3, 2.0);
				}
			}
			return num;
		}

		private void ScanType(TypeDefinition type)
		{
			string text = type.Namespace ?? "";
			if (text == "HarmonyLib" || text.StartsWith("HarmonyLib.", StringComparison.Ordinal) || text == "MonoMod" || text.StartsWith("MonoMod.", StringComparison.Ordinal) || text.StartsWith("Mono.Cecil", StringComparison.Ordinal))
			{
				Hard("bundled-framework", type.FullName, "framework type compiled into the mod");
			}
			CheckPatchAttributes(type.CustomAttributes, type.FullName);
			foreach (FieldDefinition field in type.Fields)
			{
				if (ContainsPointer(field.FieldType))
				{
					Capability("native", type.FullName + "." + field.Name, "pointer field");
				}
				if (field.HasLayoutInfo || field.RVA != 0)
				{
					byte[] array = null;
					try
					{
						array = field.InitialValue;
					}
					catch
					{
					}
					if (array != null && array.Length != 0)
					{
						CheckBlob(type.FullName + "." + field.Name, array);
					}
				}
			}
			foreach (MethodDefinition method in type.Methods)
			{
				ScanMethod(type, method);
			}
		}

		private void CheckPatchAttributes(IEnumerable<CustomAttribute> attributes, string location)
		{
			foreach (CustomAttribute attribute in attributes)
			{
				if (attribute.AttributeType.FullName != "HarmonyLib.HarmonyPatch")
				{
					continue;
				}
				Collection<CustomAttributeArgument> constructorArguments = attribute.ConstructorArguments;
				if (!attribute.IsResolved || (attribute.Constructor.Parameters.Count > 0 && constructorArguments.Count == 0))
				{
					Hard("unreadable", location, "[HarmonyPatch] arguments could not be read");
					continue;
				}
				foreach (CustomAttributeArgument item in constructorArguments)
				{
					object obj = ((item.Value is CustomAttributeArgument customAttributeArgument) ? customAttributeArgument.Value : item.Value);
					if (obj is TypeReference typeReference && IsGuardType(typeReference))
					{
						Hard("loader-target", location, "[HarmonyPatch] on " + typeReference.FullName);
					}
					if (obj is string text && IsGuardTypeName(text))
					{
						Hard("loader-target", location, "[HarmonyPatch] on \"" + text + "\"");
					}
				}
			}
		}

		private static bool ContainsPointer(TypeReference type)
		{
			while (type != null)
			{
				if (type.IsPointer || type.IsFunctionPointer)
				{
					return true;
				}
				if (type is TypeSpecification typeSpecification)
				{
					type = typeSpecification.ElementType;
					continue;
				}
				return false;
			}
			return false;
		}

		private void ScanMethod(TypeDefinition type, MethodDefinition method)
		{
			string location = type.FullName + "." + method.Name;
			CheckPatchAttributes(method.CustomAttributes, location);
			if (method.IsPInvokeImpl || method.HasPInvokeInfo)
			{
				Capability("native", location, method.HasPInvokeInfo ? ("DllImport " + method.PInvokeInfo.Module.Name + "!" + method.PInvokeInfo.EntryPoint) : "DllImport");
				if (method.HasPInvokeInfo && ModSteamRules.IsSteamNativeLibrary(method.PInvokeInfo.Module?.Name))
				{
					Hard("steam-account", location, "DllImport " + method.PInvokeInfo.Module.Name + "!" + method.PInvokeInfo.EntryPoint);
				}
			}
			if (ContainsPointer(method.ReturnType) || method.Parameters.Any((ParameterDefinition p) => ContainsPointer(p.ParameterType)))
			{
				Capability("native", location, "pointer in signature");
			}
			if (!method.HasBody)
			{
				return;
			}
			result.MethodsScanned++;
			MethodBody body;
			try
			{
				body = method.Body;
			}
			catch (Exception ex)
			{
				Hard("unreadable", location, ex.Message);
				return;
			}
			if (body.HasVariables && body.Variables.Any((VariableDefinition v) => ContainsPointer(v.VariableType)))
			{
				Capability("native", location, "pointer local");
			}
			Collection<Instruction> instructions = body.Instructions;
			HashSet<Instruction> branchTargets = BranchTargets(instructions);
			for (int i = 0; i < instructions.Count; i++)
			{
				Instruction instruction = instructions[i];
				switch (instruction.OpCode.Code)
				{
				case DseCecil.Cil.Code.Calli:
					Capability("native", location, "calli");
					break;
				case DseCecil.Cil.Code.Ldstr:
					CheckString(location, (string)instruction.Operand);
					break;
				case DseCecil.Cil.Code.Call:
				case DseCecil.Cil.Code.Callvirt:
				case DseCecil.Cil.Code.Newobj:
				case DseCecil.Cil.Code.Ldftn:
				case DseCecil.Cil.Code.Ldvirtftn:
					if (instruction.Operand is MethodReference mr)
					{
						CheckCall(location, instructions, i, mr, branchTargets);
					}
					break;
				case DseCecil.Cil.Code.Ldtoken:
					if (instruction.Operand is MethodReference mr2)
					{
						CheckCall(location, instructions, i, mr2, branchTargets);
					}
					else if (instruction.Operand is FieldReference fieldReference2)
					{
						CheckTypeUse(location, fieldReference2.DeclaringType.GetElementType(), fieldReference2.DeclaringType.Name + "." + fieldReference2.Name);
					}
					break;
				case DseCecil.Cil.Code.Stfld:
				case DseCecil.Cil.Code.Stsfld:
					if (instruction.Operand is FieldReference fieldReference && fieldReference.DeclaringType.FullName == "ES3Settings" && fieldReference.Name == "path")
					{
						Capability("files", location, "ES3Settings.path =");
					}
					break;
				}
			}
		}

		private void CheckTypeUse(string location, TypeReference declaring, string target)
		{
			string typeName = declaring.FullName;
			if (FileTypes.Contains(typeName) || FilePrefixes.Any((string p) => typeName.StartsWith(p, StringComparison.Ordinal)))
			{
				Capability("files", location, target);
			}
			else if (IsNetworkType(typeName))
			{
				Capability("network", location, target);
			}
			else if (ProcessTypes.Contains(typeName))
			{
				Hard("process-start", location, target);
			}
			else if (DynamicCodePrefixes.Any((string p) => typeName.StartsWith(p, StringComparison.Ordinal)))
			{
				Capability("dynamic-code", location, target);
			}
			else if (NativeTypes.Contains(typeName))
			{
				Capability("native", location, target);
			}
		}

		private void CheckString(string location, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			string text = value.ToLowerInvariant();
			string[] stealerPatterns = StealerPatterns;
			foreach (string value2 in stealerPatterns)
			{
				if (text.Contains(value2))
				{
					Hard("stealer-strings", location, "\"" + Truncate(value, 60) + "\"");
					break;
				}
			}
			stealerPatterns = ModSteamRules.InternalStrings;
			foreach (string value3 in stealerPatterns)
			{
				if (text.Contains(value3))
				{
					Hard("steam-account", location, "\"" + Truncate(value, 60) + "\"");
					break;
				}
			}
			if (value.Length >= 1024 && IsBase64Alphabet(value))
			{
				Hard("opaque-string", location, $"{value.Length}-character base64 constant");
			}
		}

		private static bool IsBase64Alphabet(string s)
		{
			foreach (char c in s)
			{
				if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9') && c != '+' && c != '/' && c != '=' && c != '\n' && c != '\r')
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsNetworkType(string typeName)
		{
			if (NetworkExceptions.Any((string p) => typeName.StartsWith(p, StringComparison.Ordinal)))
			{
				return false;
			}
			if (!NetworkTypes.Contains(typeName))
			{
				return NetworkPrefixes.Any((string p) => typeName.StartsWith(p, StringComparison.Ordinal));
			}
			return true;
		}

		private void CheckCall(string location, IList<Instruction> instructions, int index, MethodReference mr, HashSet<Instruction> branchTargets)
		{
			TypeReference elementType = mr.DeclaringType.GetElementType();
			string typeName = elementType.FullName;
			string name = mr.Name;
			string text = elementType.Name + "." + name;
			int num = mr.Parameters.Count((ParameterDefinition p) => p.ParameterType.FullName == "System.String");
			bool flag = mr.Parameters.Count > 0 && mr.Parameters[0].ParameterType.FullName == "System.String";
			if (typeName == "ModHelper" && SteamAuthHelperMethods.Contains(name))
			{
				Capability("steam-auth", location, text);
				return;
			}
			if (FileTypes.Contains(typeName) || FilePrefixes.Any((string p) => typeName.StartsWith(p, StringComparison.Ordinal)))
			{
				Capability("files", location, text);
				return;
			}
			if (typeName == "UnityEngine.AssetBundle" && AssetBundlePathMethods.Contains(name))
			{
				Capability("files", location, text + "(path)");
				return;
			}
			if ((typeName == "System.IO.StreamWriter" || typeName == "System.IO.StreamReader") && name == ".ctor" && flag)
			{
				Capability("files", location, text + "(path)");
				return;
			}
			if (PathStringTypes.Contains(typeName) && (name == "Load" || name == "Save" || name == "Create") && flag)
			{
				Capability("files", location, text + "(path)");
				return;
			}
			if (typeName == "ES3")
			{
				if (ES3PathMethods.Contains(name) || num >= 2)
				{
					Capability("files", location, text + "(path)");
					return;
				}
				bool flag2 = name.StartsWith("Load", StringComparison.Ordinal) || name == "KeyExists" || name == "GetKeys";
				if (flag && !mr.HasThis && !flag2)
				{
					Instruction instruction = FindProducer(instructions, index, 0, mr.Parameters.Count, branchTargets);
					if (instruction != null && instruction.OpCode.Code == DseCecil.Cil.Code.Ldstr && TrustKeys.Contains((string)instruction.Operand))
					{
						Hard("loader-target", location, text + "(\"" + (string)instruction.Operand + "\")");
					}
				}
				return;
			}
			if (typeName == "ES3Settings" && name == ".ctor" && num > 0)
			{
				Capability("files", location, text + "(path)");
				return;
			}
			if (IsNetworkType(typeName))
			{
				Capability("network", location, text);
				return;
			}
			switch (ModSteamRules.Classify(typeName, name))
			{
			case ModSteamUse.WorkshopPublish:
				Hard("workshop-publish", location, text);
				return;
			case ModSteamUse.Account:
				Hard("steam-account", location, text);
				return;
			case ModSteamUse.Capability:
				if (!IsValueType(elementType))
				{
					Capability("steam", location, text);
				}
				return;
			}
			if (ReflectionExternalPrefixes.Any((string p) => typeName.StartsWith(p, StringComparison.Ordinal)))
			{
				Capability("reflection-external", location, text + " (dynamic)");
				return;
			}
			if ((typeName == "System.Linq.Expressions.LambdaExpression" || typeName == "System.Linq.Expressions.Expression`1") && name == "Compile")
			{
				Capability("dynamic-code", location, text);
				return;
			}
			if (ProcessTypes.Contains(typeName))
			{
				Hard("process-start", location, text);
				return;
			}
			if (typeName == "UnityEngine.Application" && name == "OpenURL")
			{
				Instruction instruction2 = FindProducer(instructions, index, 0, mr.Parameters.Count, branchTargets);
				string text2 = ((instruction2 != null && instruction2.OpCode.Code == DseCecil.Cil.Code.Ldstr) ? ((string)instruction2.Operand) : null);
				if (text2 != null && (text2.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("http://", StringComparison.OrdinalIgnoreCase)))
				{
					Capability("process", location, text + "(\"" + Truncate(text2, 60) + "\")");
				}
				else
				{
					Hard("process-start", location, (text2 != null) ? (text + "(\"" + Truncate(text2, 60) + "\")") : (text + " with a computed argument"));
				}
				return;
			}
			if (!DynamicCodePrefixes.Any((string p) => typeName.StartsWith(p, StringComparison.Ordinal)) && (!(typeName == "System.Reflection.Assembly") || !AssemblyLoadMethods.Contains(name)) && (!(typeName == "System.AppDomain") || !AppDomainLoadMethods.Contains(name)))
			{
				if (!(typeName == "System.Activator"))
				{
					goto IL_05c3;
				}
				switch (name)
				{
				case "CreateInstance":
					if (mr.Parameters.Count > 0 && mr.Parameters[0].ParameterType.FullName == "System.String")
					{
						break;
					}
					goto IL_05c3;
				case "CreateInstanceFrom":
				case "CreateComInstanceFrom":
					break;
				default:
					goto IL_05c3;
				}
			}
			Capability("dynamic-code", location, text);
			return;
			IL_05c3:
			if (NativeTypes.Contains(typeName))
			{
				Capability("native", location, text);
				return;
			}
			if (typeName == "HarmonyLib.Harmony" && name == "UnpatchAll" && mr.HasThis && mr.Parameters.Count == 1)
			{
				Instruction instruction3 = FindProducer(instructions, index, 1, 2, branchTargets);
				if (instruction3 != null && instruction3.OpCode.Code == DseCecil.Cil.Code.Ldnull)
				{
					Hard("unpatch-all", location, "Harmony.UnpatchAll() without an id");
				}
			}
			if (typeName == "HarmonyLib.Harmony" && name.StartsWith("Unpatch", StringComparison.Ordinal))
			{
				int num2 = (mr.HasThis ? 1 : 0);
				for (int i = 0; i < mr.Parameters.Count; i++)
				{
					if (mr.Parameters[i].ParameterType.FullName != "System.String")
					{
						continue;
					}
					Instruction instruction4 = FindProducer(instructions, index, i + num2, mr.Parameters.Count + num2, branchTargets);
					if (instruction4 != null && instruction4.OpCode.Code == DseCecil.Cil.Code.Ldstr)
					{
						string text3 = (string)instruction4.Operand;
						if (text3 == "*")
						{
							Hard("unpatch-all", location, "Harmony." + name + "(\"*\")");
						}
						else if (text3.StartsWith("dse.", StringComparison.Ordinal))
						{
							Hard("loader-target", location, "Harmony." + name + "(\"" + Truncate(text3, 40) + "\")");
						}
					}
				}
				return;
			}
			if (typeName == "System.Type" && name == "GetType" && mr.Parameters.Count > 0 && mr.Parameters[0].ParameterType.FullName == "System.String")
			{
				ClassifyStringTarget(location, text, FindProducer(instructions, index, 0, mr.Parameters.Count, branchTargets));
				return;
			}
			if (typeName == "System.Type" && TypeMemberLookups.Contains(name))
			{
				ClassifyReceiver(location, text, instructions, index, mr, branchTargets);
				return;
			}
			if (typeName == "System.Reflection.Assembly")
			{
				switch (name)
				{
				case "GetType":
				case "GetTypes":
				case "GetExportedTypes":
				case "CreateInstance":
				{
					Instruction instruction5 = FindProducer(instructions, index, 0, mr.Parameters.Count + 1, branchTargets);
					if (name == "GetType" && flag)
					{
						ClassifyStringTarget(location, text, FindProducer(instructions, index, 1, mr.Parameters.Count + 1, branchTargets));
					}
					if (instruction5 != null && instruction5.Operand is MethodReference methodReference && (methodReference.Name == "GetExecutingAssembly" || methodReference.Name == "GetCallingAssembly"))
					{
						return;
					}
					if (instruction5 != null && instruction5.Operand is MethodReference { Name: "get_Assembly" } methodReference2 && methodReference2.DeclaringType.FullName == "System.Type")
					{
						int callIndex = instructions.IndexOf(instruction5);
						Instruction producer = FindProducer(instructions, callIndex, 0, 1, branchTargets);
						TypeReference typeReference = TypeofTarget(instructions, producer, branchTargets);
						if (typeReference != null)
						{
							ClassifyTypeReference(location, text, typeReference);
							return;
						}
					}
					Capability("reflection-external", location, text + " on a computed assembly");
					return;
				}
				}
			}
			if (!(typeName == "HarmonyLib.AccessTools") && !(typeName == "HarmonyLib.Traverse"))
			{
				return;
			}
			if (mr is GenericInstanceMethod genericInstanceMethod)
			{
				foreach (TypeReference genericArgument in genericInstanceMethod.GenericArguments)
				{
					ClassifyTypeReference(location, text, genericArgument);
				}
			}
			if (mr.Parameters.Count == 0)
			{
				return;
			}
			string fullName = mr.Parameters[0].ParameterType.FullName;
			if (mr.HasThis)
			{
				return;
			}
			if (fullName == "System.String")
			{
				ClassifyStringTarget(location, text, FindProducer(instructions, index, 0, mr.Parameters.Count, branchTargets));
			}
			else if (fullName == "System.Type")
			{
				Instruction producer2 = FindProducer(instructions, index, 0, mr.Parameters.Count, branchTargets);
				TypeReference typeReference2 = TypeofTarget(instructions, producer2, branchTargets);
				if (typeReference2 != null)
				{
					ClassifyTypeReference(location, text, typeReference2);
				}
				else
				{
					Capability("reflection-external", location, text + " on a computed type");
				}
			}
		}

		private void ClassifyReceiver(string location, string target, IList<Instruction> instructions, int index, MethodReference mr, HashSet<Instruction> branchTargets)
		{
			Instruction instruction = FindProducer(instructions, index, 0, mr.Parameters.Count + 1, branchTargets);
			TypeReference typeReference = TypeofTarget(instructions, instruction, branchTargets);
			if (typeReference != null)
			{
				ClassifyTypeReference(location, target, typeReference);
			}
			else if (instruction != null && instruction.Operand is MethodReference methodReference && methodReference.DeclaringType.FullName == "System.Type" && methodReference.Name == "GetType" && methodReference.Parameters.Count > 0)
			{
				int callIndex = instructions.IndexOf(instruction);
				ClassifyStringTarget(location, target, FindProducer(instructions, callIndex, 0, methodReference.Parameters.Count, branchTargets));
			}
			else
			{
				Capability("reflection-external", location, target + " on a computed type");
			}
		}

		private TypeReference TypeofTarget(IList<Instruction> instructions, Instruction producer, HashSet<Instruction> branchTargets)
		{
			if (producer == null)
			{
				return null;
			}
			if (producer.Operand is MethodReference { Name: "GetTypeFromHandle" } methodReference && methodReference.DeclaringType.FullName == "System.Type")
			{
				int callIndex = instructions.IndexOf(producer);
				Instruction instruction = FindProducer(instructions, callIndex, 0, 1, branchTargets);
				if (instruction != null && instruction.OpCode.Code == DseCecil.Cil.Code.Ldtoken && instruction.Operand is TypeReference typeReference)
				{
					return typeReference;
				}
			}
			return null;
		}

		private void ClassifyTypeReference(string location, string target, TypeReference type)
		{
			if (IsGuardType(type))
			{
				Hard("loader-target", location, target + " on " + type.FullName);
			}
			else if (IsExternalScope(type))
			{
				Capability("reflection-external", location, target + " on " + type.FullName);
			}
		}

		private void ClassifyStringTarget(string location, string target, Instruction producer)
		{
			if (producer == null || producer.OpCode.Code != DseCecil.Cil.Code.Ldstr)
			{
				Capability("reflection-external", location, target + " with a computed name");
				return;
			}
			string text = (string)producer.Operand;
			string typeName = text.Split(':')[0].Split(',')[0].Trim();
			if (IsGuardTypeName(typeName))
			{
				Hard("loader-target", location, target + "(\"" + Truncate(text, 60) + "\")");
			}
			else if (ExternalNamePrefixes.Any((string p) => typeName.StartsWith(p, StringComparison.Ordinal)))
			{
				Capability("reflection-external", location, target + "(\"" + Truncate(text, 60) + "\")");
			}
		}

		private bool IsExternalScope(TypeReference type)
		{
			TypeReference typeReference = type;
			while (typeReference.DeclaringType != null)
			{
				typeReference = typeReference.DeclaringType;
			}
			if (typeReference.Scope is ModuleDefinition)
			{
				return false;
			}
			string scopeName = typeReference.Scope?.Name ?? "";
			if (scopeName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
			{
				scopeName = scopeName.Substring(0, scopeName.Length - 4);
			}
			if (scopeName == "Assembly-CSharp" || scopeName == "Assembly-CSharp-firstpass")
			{
				return false;
			}
			return ExternalScopePrefixes.Any((string p) => scopeName.StartsWith(p, StringComparison.Ordinal));
		}

		private static bool IsGuardType(TypeReference type)
		{
			TypeReference typeReference = type;
			while (typeReference.DeclaringType != null)
			{
				typeReference = typeReference.DeclaringType;
			}
			if (typeReference.Scope is ModuleDefinition)
			{
				return false;
			}
			if (string.IsNullOrEmpty(typeReference.Namespace))
			{
				return GuardTypes.Contains(typeReference.Name);
			}
			if (typeReference.Namespace == "Steamworks")
			{
				return ModSteamRules.IsGuardType(typeReference.FullName);
			}
			return false;
		}

		private static bool IsGuardTypeName(string name)
		{
			string text = name.Split('/')[0].Split('+')[0].Trim();
			if (!GuardTypes.Contains(text))
			{
				if (text.StartsWith("Steamworks.", StringComparison.Ordinal) || !text.Contains("."))
				{
					return ModSteamRules.IsGuardType(text);
				}
				return false;
			}
			return true;
		}

		private static bool IsValueType(TypeReference type)
		{
			if (type.IsValueType)
			{
				return true;
			}
			try
			{
				TypeDefinition typeDefinition = type.Resolve();
				if (typeDefinition != null)
				{
					return typeDefinition.IsValueType || typeDefinition.IsEnum;
				}
			}
			catch
			{
			}
			return type.Name.EndsWith("_t", StringComparison.Ordinal);
		}

		private static Instruction FindProducer(IList<Instruction> instructions, int callIndex, int argIndex, int totalArgs, HashSet<Instruction> branchTargets)
		{
			int num = totalArgs - argIndex;
			for (int num2 = callIndex - 1; num2 >= 0; num2--)
			{
				Instruction instruction = instructions[num2];
				if (branchTargets.Contains(instructions[num2 + 1]))
				{
					return null;
				}
				if (instruction.OpCode.Code != 0)
				{
					if (instruction.OpCode.FlowControl == FlowControl.Branch || instruction.OpCode.FlowControl == FlowControl.Cond_Branch || instruction.OpCode.FlowControl == FlowControl.Return || instruction.OpCode.FlowControl == FlowControl.Throw)
					{
						return null;
					}
					int num3 = PushCount(instruction);
					if (num3 < 0)
					{
						return null;
					}
					if (num3 >= num)
					{
						return instruction;
					}
					int num4 = PopCount(instruction);
					if (num4 < 0)
					{
						return null;
					}
					num = num - num3 + num4;
				}
			}
			return null;
		}

		private static HashSet<Instruction> BranchTargets(IList<Instruction> instructions)
		{
			HashSet<Instruction> hashSet = new HashSet<Instruction>();
			foreach (Instruction instruction in instructions)
			{
				if (instruction.Operand is Instruction item)
				{
					hashSet.Add(item);
				}
				else if (instruction.Operand is Instruction[] array)
				{
					Instruction[] array2 = array;
					foreach (Instruction item2 in array2)
					{
						hashSet.Add(item2);
					}
				}
			}
			return hashSet;
		}

		private static int PushCount(Instruction ins)
		{
			switch (ins.OpCode.StackBehaviourPush)
			{
			case StackBehaviour.Push0:
				return 0;
			case StackBehaviour.Push1:
			case StackBehaviour.Pushi:
			case StackBehaviour.Pushi8:
			case StackBehaviour.Pushr4:
			case StackBehaviour.Pushr8:
			case StackBehaviour.Pushref:
				return 1;
			case StackBehaviour.Push1_push1:
				return 2;
			case StackBehaviour.Varpush:
				if (ins.OpCode.Code == DseCecil.Cil.Code.Newobj)
				{
					return 1;
				}
				if (ins.Operand is MethodReference methodReference)
				{
					if (!(methodReference.ReturnType.FullName == "System.Void"))
					{
						return 1;
					}
					return 0;
				}
				return -1;
			default:
				return -1;
			}
		}

		private static int PopCount(Instruction ins)
		{
			switch (ins.OpCode.StackBehaviourPop)
			{
			case StackBehaviour.Pop0:
				return 0;
			case StackBehaviour.Pop1:
			case StackBehaviour.Popi:
			case StackBehaviour.Popref:
				return 1;
			case StackBehaviour.Pop1_pop1:
			case StackBehaviour.Popi_pop1:
			case StackBehaviour.Popi_popi:
			case StackBehaviour.Popi_popi8:
			case StackBehaviour.Popi_popr4:
			case StackBehaviour.Popi_popr8:
			case StackBehaviour.Popref_pop1:
			case StackBehaviour.Popref_popi:
				return 2;
			case StackBehaviour.Popi_popi_popi:
			case StackBehaviour.Popref_popi_popi:
			case StackBehaviour.Popref_popi_popi8:
			case StackBehaviour.Popref_popi_popr4:
			case StackBehaviour.Popref_popi_popr8:
			case StackBehaviour.Popref_popi_popref:
				return 3;
			case StackBehaviour.Varpop:
				if (ins.Operand is MethodReference methodReference)
				{
					int num = methodReference.Parameters.Count;
					if (methodReference.HasThis && ins.OpCode.Code != DseCecil.Cil.Code.Newobj)
					{
						num++;
					}
					return num;
				}
				return -1;
			default:
				return -1;
			}
		}

		private void Capability(string capability, string location, string target)
		{
			result.Capabilities.Add(capability);
			if (result.Findings.Count < 400)
			{
				result.Findings.Add(new ModScanFinding
				{
					Kind = capability,
					Location = location,
					Target = target
				});
			}
		}

		private void Hard(string flag, string location, string target)
		{
			if (result.HardFlags.Count < 100)
			{
				result.HardFlags.Add(new ModScanFinding
				{
					Kind = flag,
					Location = location,
					Target = target,
					IsHardFlag = true
				});
			}
		}

		private static bool StartsWith(byte[] data, string ascii)
		{
			if (data.Length < ascii.Length)
			{
				return false;
			}
			for (int i = 0; i < ascii.Length; i++)
			{
				if (data[i] != (byte)ascii[i])
				{
					return false;
				}
			}
			return true;
		}

		internal static bool ContainsAscii(byte[] data, string ascii)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(ascii);
			for (int i = 0; i <= data.Length - bytes.Length; i++)
			{
				int j;
				for (j = 0; j < bytes.Length && data[i + j] == bytes[j]; j++)
				{
				}
				if (j == bytes.Length)
				{
					return true;
				}
			}
			return false;
		}

		private static string Truncate(string s, int max)
		{
			if (s.Length > max)
			{
				return s.Substring(0, max) + "...";
			}
			return s;
		}
	}

	public const string FLAG_PAYLOAD_RESOURCE = "payload-resource";

	public const string FLAG_OPAQUE_RESOURCE = "opaque-resource";

	public const string FLAG_STEALER_STRINGS = "stealer-strings";

	public const string FLAG_OPAQUE_STRING = "opaque-string";

	public const string FLAG_UNPATCH_ALL = "unpatch-all";

	public const string FLAG_LOADER_TARGET = "loader-target";

	public const string FLAG_BUNDLED_FRAMEWORK = "bundled-framework";

	public const string FLAG_UNREADABLE = "unreadable";

	public const string FLAG_UNLISTED_REFERENCE = "unlisted-reference";

	public const string FLAG_WORKSHOP_PUBLISH = "workshop-publish";

	public const string FLAG_STEAM_ACCOUNT = "steam-account";

	public const string FLAG_PAYLOAD_FILE = "payload-file";

	public const string FLAG_OVERSIZED = "oversized";

	public const string FLAG_PROCESS_START = "process-start";

	public const int VERSION = 3;

	public const int MAX_FOLDER_FILES = 50000;

	private const long MAX_OPAQUE_FILE_BYTES = 8388608L;

	public static readonly string[] GuardTypes = new string[14]
	{
		"ModLoader", "ModTrust", "ModTrustRecord", "ModFingerprint", "ModTrustList", "ModScanner", "ModCapability", "ModSecurityDebug", "ModSteamRules", "ModSteamGuard",
		"ModSteamAuth", "ModWorkshop", "SteamManager", "PaperBookRegistry"
	};

	private static readonly string[] ExecutableExtensions = new string[29]
	{
		".exe", ".dll", ".com", ".scr", ".pif", ".cpl", ".msi", ".msp", ".sys", ".drv",
		".ocx", ".bat", ".cmd", ".ps1", ".psm1", ".psd1", ".vbs", ".vbe", ".js", ".jse",
		".wsf", ".wsh", ".hta", ".lnk", ".url", ".reg", ".jar", ".so", ".dylib"
	};

	public static ModScanResult Scan(IEnumerable<string> dllPaths, IEnumerable<string> allowedReferences = null, string folderPath = null, bool localFolder = false)
	{
		ModScanResult modScanResult = new ModScanResult();
		List<string> list = dllPaths.ToList();
		HashSet<string> allowedReferences2 = new HashSet<string>((allowedReferences ?? Enumerable.Empty<string>()).Concat(list.Select(Path.GetFileNameWithoutExtension)), StringComparer.OrdinalIgnoreCase);
		using DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();
		foreach (string item in ResolverDirectories(list))
		{
			defaultAssemblyResolver.AddSearchDirectory(item);
		}
		foreach (string item2 in list)
		{
			try
			{
				using MemoryStream stream = new MemoryStream(File.ReadAllBytes(item2), writable: false);
				using AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(stream, new ReaderParameters
				{
					ReadingMode = ReadingMode.Immediate,
					InMemory = true,
					AssemblyResolver = defaultAssemblyResolver
				});
				new Pass(modScanResult, assembly, Path.GetFileName(item2), allowedReferences2).Run();
			}
			catch (Exception ex)
			{
				modScanResult.Error = Path.GetFileName(item2) + " is not a readable .NET assembly (" + ex.GetType().Name + ": " + ex.Message + ")";
				return modScanResult;
			}
		}
		if (folderPath != null)
		{
			ScanFolderFiles(modScanResult, folderPath, list, localFolder);
		}
		return modScanResult;
	}

	private static void ScanFolderFiles(ModScanResult result, string folderPath, List<string> listedDlls, bool localFolder)
	{
		string text;
		IEnumerable<string> enumerable;
		try
		{
			text = Path.GetFullPath(folderPath).TrimEnd('\\', '/');
			enumerable = EnumerateModFiles(text, localFolder);
		}
		catch (Exception ex)
		{
			result.Error = "the mod folder could not be listed (" + ex.GetType().Name + ": " + ex.Message + ")";
			return;
		}
		HashSet<string> hashSet = new HashSet<string>(listedDlls.Select(Path.GetFullPath), StringComparer.OrdinalIgnoreCase);
		int num = 0;
		try
		{
			foreach (string item in enumerable)
			{
				if (++num > 50000)
				{
					Flag("oversized", "mod folder", $"more than {50000} files");
					break;
				}
				string fullPath = Path.GetFullPath(item);
				if (hashSet.Contains(fullPath))
				{
					continue;
				}
				string location2 = fullPath.Substring(text.Length).TrimStart('\\', '/').Replace('\\', '/');
				long length = new FileInfo(fullPath).Length;
				string text2 = Path.GetExtension(fullPath).ToLowerInvariant();
				if (ExecutableExtensions.Contains(text2))
				{
					Flag("payload-file", location2, text2 + " file");
				}
				else
				{
					if (length == 0L)
					{
						continue;
					}
					byte[] array = ReadHead(fullPath, 4096);
					if (Pass.IsExecutableHeader(array))
					{
						Flag("payload-file", location2, "executable content");
					}
					else
					{
						if (Pass.IsKnownMedia(array) || Pass.MostlyPrintable(array, array.Length) || length < 512)
						{
							continue;
						}
						if (length > 8388608)
						{
							Flag("opaque-resource", location2, $"{length} bytes of unknown format");
							continue;
						}
						byte[] array2 = File.ReadAllBytes(fullPath);
						double num2 = Pass.Entropy(array2);
						if (num2 > 7.5)
						{
							Flag("opaque-resource", location2, $"{array2.Length} bytes, entropy {num2:0.00} bits/byte, unknown format");
						}
						else if (Pass.ContainsAscii(array2, "This program cannot be run in DOS mode"))
						{
							Flag("payload-file", location2, "embedded executable");
						}
					}
				}
			}
		}
		catch (Exception ex2)
		{
			result.Error = "a file in the mod folder could not be read (" + ex2.GetType().Name + ": " + ex2.Message + ")";
		}
		void Flag(string flag, string location, string target)
		{
			if (result.HardFlags.Count < 100)
			{
				result.HardFlags.Add(new ModScanFinding
				{
					Kind = flag,
					Location = location,
					Target = target,
					IsHardFlag = true
				});
			}
		}
	}

	private static IEnumerable<string> EnumerateModFiles(string root, bool skipDotFolders)
	{
		Stack<string> folders = new Stack<string>();
		folders.Push(root);
		while (folders.Count > 0)
		{
			string folder = folders.Pop();
			foreach (string item in System.IO.Directory.EnumerateFiles(folder))
			{
				yield return item;
			}
			foreach (string item2 in System.IO.Directory.EnumerateDirectories(folder))
			{
				if (!skipDotFolders || !Path.GetFileName(item2).StartsWith(".", StringComparison.Ordinal))
				{
					folders.Push(item2);
				}
			}
		}
	}

	private static byte[] ReadHead(string path, int max)
	{
		using FileStream fileStream = File.OpenRead(path);
		byte[] array = new byte[(int)Math.Min(max, fileStream.Length)];
		int i;
		int num;
		for (i = 0; i < array.Length; i += num)
		{
			num = fileStream.Read(array, i, array.Length - i);
			if (num <= 0)
			{
				break;
			}
		}
		if (i < array.Length)
		{
			Array.Resize(ref array, i);
		}
		return array;
	}

	private static IEnumerable<string> ResolverDirectories(IEnumerable<string> dllPaths)
	{
		HashSet<string> dirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		Add(typeof(object).Assembly.Location);
		Add(typeof(Harmony).Assembly.Location);
		Add(typeof(ModScanner).Assembly.Location);
		Add(typeof(UnityEngine.Object).Assembly.Location);
		foreach (string dllPath in dllPaths)
		{
			Add(dllPath);
		}
		return dirs;
		void Add(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			try
			{
				string directoryName = Path.GetDirectoryName(path);
				if (!string.IsNullOrEmpty(directoryName) && System.IO.Directory.Exists(directoryName))
				{
					dirs.Add(directoryName);
				}
				string text = Path.Combine(directoryName ?? "", "Facades");
				if (System.IO.Directory.Exists(text))
				{
					dirs.Add(text);
				}
			}
			catch
			{
			}
		}
	}
}
