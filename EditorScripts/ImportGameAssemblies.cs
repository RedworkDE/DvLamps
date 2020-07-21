using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ImportGameAssemblies
{
	private const string ASSEMBLIES_LOCAL = @"Assets\Assemblies";
	private const string ASSEMBLIES_REMOTE = @"DerailValley_Data\Managed";
	private const string DERAIL_VALLEY_PATH_HINT = @"Assets\Editor\DvInstallPathHint.txt";
	private static string[] _buildinSearchPaths = { @"C:\DerailValley", @"C:\Program Files (x86)\Steam\steamapps\common\Derail Valley", @"D:\Program Files (x86)\Steam\steamapps\common\Derail Valley"};

	private static string _dvInstall;
	private static string DvInstall => _dvInstall = _dvInstall ?? FindDvInstall();

	private static string[] _blacklist = {"System.", "UnityEngine.", "mscorlib.dll"};
	private static string[] _whitelist = { "System.Runtime.CompilerServices.Unsafe.dll", "UnityEngine.UI.dll" };

	static ImportGameAssemblies()
	{
		UpdateAssemblies();
	}

	private static string FindDvInstall()
	{
		foreach (var path in _buildinSearchPaths)
			if (Directory.Exists(path))
				return path;

		if (!File.Exists(DERAIL_VALLEY_PATH_HINT)) File.Create(DERAIL_VALLEY_PATH_HINT).Dispose();

		foreach (var path in File.ReadAllLines(DERAIL_VALLEY_PATH_HINT))
			if (Directory.Exists(path))
				return path;

		Debug.LogError("Unable to find Derail Valley! Please add your installation path to " + DERAIL_VALLEY_PATH_HINT);

		return null;
	}


	private static bool UpdateAssembly(string dllName)
	{
		var tn = AssemblyNameFixup.Mask(dllName);

		var target = new FileInfo(Path.Combine(ASSEMBLIES_LOCAL, tn));
		var targetMeta = new FileInfo(Path.Combine(ASSEMBLIES_LOCAL, tn + ".meta"));
		var source = new FileInfo(Path.Combine(DvInstall, ASSEMBLIES_REMOTE, dllName));

		if (!source.Exists) return false;
		if (target.Exists && target.LastWriteTimeUtc > source.LastWriteTimeUtc) return false;

		//var module = ModuleDefinition.ReadModule(source.FullName);
		//AssemblyNameFixup.Mask(module);
		//module.Write(target.FullName);

		var bytes = File.ReadAllBytes(source.FullName);
		AssemblyNameFixup.Mask(bytes);
		File.WriteAllBytes(target.FullName, bytes);

		if (!targetMeta.Exists)
		{
			var md5 = new MD5Cng().ComputeHash(Encoding.UTF8.GetBytes(dllName));
			var guid = new Guid(md5);
			File.WriteAllText(targetMeta.FullName, $"fileFormatVersion: 2\nguid: {guid:N}\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n");
		}

		return true;
	}

	[MenuItem("Build/Update Assemblies")]
	static void UpdateAssemblies()
	{
		var asmPath = Path.Combine(DvInstall, ASSEMBLIES_REMOTE);

		if (!Directory.Exists(asmPath)) return;

		var update = false;

		foreach (var file in Directory.EnumerateFiles(asmPath, "*.dll"))
		{
			var fn = Path.GetFileName(file);

			if (Array.Exists(_blacklist, item => fn.StartsWith(item)) && !Array.Exists(_whitelist, item => fn.StartsWith(item))) continue;

			update |= UpdateAssembly(fn);
		}

		if (update) AssetDatabase.Refresh();
	}
}
