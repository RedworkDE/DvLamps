using System;
using System.Text;
//using Mono.Cecil;

public class AssemblyNameFixup
{
	public static byte[] assomblyBytes = Encoding.ASCII.GetBytes("Assombly-");
	public static byte[] assemblyBytes = Encoding.ASCII.GetBytes("Assembly-");

	public static void Mask(byte[] bytes)
	{
		foreach (var i in bytes.Locate(assemblyBytes)) Buffer.BlockCopy(assomblyBytes, 0, bytes, i, assomblyBytes.Length);
	}

	public static string Mask(string str)
	{
		var bytes = Encoding.UTF8.GetBytes(str);
		var locs = bytes.Locate(assemblyBytes);

		if (locs.Length == 0) return str;

		foreach (var i in locs) Buffer.BlockCopy(assomblyBytes, 0, bytes, i, assomblyBytes.Length);

		return Encoding.UTF8.GetString(bytes);
	}

	//public static void Mask(ModuleDefinition module) => Update(module, Mask);

	public static void Unmask(byte[] bytes)
	{
		foreach (var i in bytes.Locate(assomblyBytes)) Buffer.BlockCopy(assemblyBytes, 0, bytes, i, assemblyBytes.Length);
	}

	public static string Unmask(string str)
	{
		var bytes = Encoding.UTF8.GetBytes(str);
		var locs = bytes.Locate(assomblyBytes);

		if (locs.Length == 0) return str;

		foreach (var i in locs) Buffer.BlockCopy(assemblyBytes, 0, bytes, i, assemblyBytes.Length);

		return Encoding.UTF8.GetString(bytes);
	}

	private static byte[] Copy(byte[] data)
	{
		var arr = new byte[data.Length];
		data.CopyTo(arr,0);
		return arr;
	}

	//public static void Unmask(ModuleDefinition module) => Update(module, Unmask);

	//public static void Update(ModuleDefinition module, Func<string, string> updater)
	//{
	//	module.Name = updater(module.Name);
	//	var assembly = module.Assembly;
	//	assembly.Name.Name = updater(assembly.Name.Name);

	//	foreach (var @ref in module.AssemblyReferences) @ref.Name = updater(@ref.Name);
	//	foreach (var @ref in module.ModuleReferences) @ref.Name = updater(@ref.Name);
	//}
}