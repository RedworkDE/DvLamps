using System.IO;
using Mono.Cecil;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public class ExportBundle
{
	[MenuItem("Build/Asset Bundle")]
	static void BuildBundle()
	{
		Debug.Log("Building");

		AssetBundleBuild[] build = new AssetBundleBuild[1];
		build[0] = new AssetBundleBuild();
		build[0].assetBundleName = "lamps";
		build[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("lamps");

		Directory.CreateDirectory("build/AssetBundles");

		File.Delete("build/AssetBundles/lamps");
		File.Delete("build/AssetBundles/lamps.manifest");


		var result = BuildPipeline.BuildAssetBundles("build/AssetBundles", build, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);

		if (!result)
		{
			Debug.Log($"Failed to build asset bundle");
		}

		var bytes = File.ReadAllBytes("build/AssetBundles/lamps");
		AssemblyNameFixup.Unmask(bytes);
		File.WriteAllBytes("../RedworkDE.DvLamps/lamps", bytes);

		Debug.Log("Build Successful");
	}

	[MenuItem("Build/Library")]
	static void BuildLibrary()
	{
		var asms = CompilationPipeline.GetAssemblies(AssembliesType.Player);
		foreach (var asm in asms)
		{
			//var module = ModuleDefinition.ReadModule(asm.outputPath);
			//AssemblyNameFixup.Unmask(module);
			//module.Write(Path.Combine("../RedworkDE.DvLamps", module.Name));

			var bytes = File.ReadAllBytes(asm.outputPath);
			AssemblyNameFixup.Unmask(bytes);
			File.WriteAllBytes(Path.Combine("../RedworkDE.DvLamps", Path.GetFileName(asm.outputPath)), bytes);
		}
	}

	[MenuItem("Build/All")]
	static void BuildAll()
	{
		BuildBundle();
		BuildLibrary();
	}
}