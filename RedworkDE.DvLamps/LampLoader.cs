using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DV.Shops;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedworkDE.DvLamps
{
	public class LampLoader
	{
		private static AssetBundle _bundle;

		[AutoLoad]
		static void Init()
		{
			_bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(typeof(LampLoader).Assembly.Location), "lamps"));
			Log.Info($"Lamp loader: {_bundle}");
		}

		[AutoLoad(LoadTime.GameLoaded)]
		static void CreateShopItems()
		{
			Log.Debug("Create shop items");

			if (!_bundle)
			{
				Log.Error("Failed to load lamps bundle");
				return;
			}

			//var spec = _bundle.LoadAsset<GameObject>("MagicLamp").GetComponent<InventoryItemSpec>();
			var spec = _bundle.LoadAsset<GameObject>("FlashLight").GetComponent<InventoryItemSpec>();

			Log.Debug($"Item spec: {spec}");

			GlobalShopController.Instance.shopItemsData.Add(new ShopItemData()
			{
				item = spec,
				basePrice = 100,
				amount = 100,
				isGlobal = true,
			});
#if PUBLICIZER_ASSEMBLY_CSHARP
			GlobalShopController.Instance.initialItemAmounts
#else
			((List<int>)typeof(GlobalShopController).GetField("initialItemAmounts", BindingFlags.Instance|BindingFlags.NonPublic).GetValue(GlobalShopController.Instance))
#endif
				.Add(100);

			Log.Debug("Added global shop data");

			var shops = Object.FindObjectsOfType<Shop>();
			foreach (var shop in shops)
			{
				Log.Debug($"adding to shop {shop}");

				var findMax = shop.scanItemResourceModules.FindMax(r => r.transform.localPosition.x);
				var resource = Object.Instantiate(findMax);
				resource.gameObject.SetActive(true);
				resource.sellingItemSpec = spec;
				resource.transform.parent = findMax.transform.parent;
				resource.transform.localRotation = findMax.transform.localRotation;
				resource.transform.localPosition = findMax.transform.localPosition + Vector3.right * 0.6f;
#if PUBLICIZER_ASSEMBLY_CSHARP
				resource.Start(); // call start again to fix texts
#else
				typeof(ScanItemResourceModule).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(resource, null);
#endif

				Log.Debug($"new item sign: {resource}");
				
				var arr = new ScanItemResourceModule[shop.scanItemResourceModules.Length + 1];
				Array.Copy(shop.scanItemResourceModules, 0, arr, 0, shop.scanItemResourceModules.Length);
				arr[arr.Length - 1] = resource;
				shop.cashRegister.resourceMachines = Array.ConvertAll(arr, e => (ResourceModule) e);

				resource.ItemPurchased += GlobalShopController.Instance.AddItemToInstatiationQueue;
			}

			Log.Debug("done");
		}
		
		[HarmonyPatch]
		class Resources_Patch
		{
			static MethodBase TargetMethod()
			{
				return typeof(Resources).GetMethods().Single(m => m.Name == nameof(Resources.Load) && !m.ContainsGenericParameters && m.GetParameters().Length == 1);
			}

			static bool Prefix(string path, ref Object __result)
			{
				const string prefix = "RedworkDE.DvLamps.";

				Log.Debug($"Resource load: {path}");

				if (path is {} && _bundle != null && path.StartsWith(prefix))
				{
					__result = _bundle.LoadAsset(path.Substring(prefix.Length));
					return false;
				}

				return true;
			}
		}
	}
}