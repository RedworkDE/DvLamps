﻿using System;
using System.Linq;
using System.Reflection;
using DV.Shops;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedworkDE.DvLamps
{
	public class LampLoader : AutoLoad<LampLoader>
	{
		private static readonly AssetBundle _bundle;

		static LampLoader()
		{
			_bundle = AssetBundle.LoadFromStream(typeof(LampLoader).Assembly.GetManifestResourceStream(typeof(LampLoader), "lamps"));
			Logger.LogInfo($"Lamp loader: {_bundle}");

			WorldStreamingInit.LoadingFinished += CreateShopItems;
		}

		static void CreateShopItems()
		{
			Logger.LogDebug("Create shop items");

			var spec = _bundle.LoadAsset<GameObject>("MagicLamp").GetComponent<InventoryItemSpec>();

			Logger.LogDebug($"Item spec: {spec}");

			GlobalShopController.Instance.shopItemsData.Add(new ShopItemData()
			{
				item = spec,
				basePrice = 10000,
				amount = 1,
				isGlobal = true,
			});
			GlobalShopController.Instance.initialItemAmounts.Add(1);

			Logger.LogDebug("Added global shop data");

			var shops = Object.FindObjectsOfType<Shop>();
			foreach (var shop in shops)
			{
				Logger.LogDebug($"adding to shop {shop}");

				var findMax = shop.scanItemResourceModules.FindMax(r => r.transform.localPosition.x);
				var resource = Object.Instantiate(findMax);
				resource.sellingItemSpec = spec;
				resource.transform.parent = findMax.transform.parent;
				resource.transform.localRotation = findMax.transform.localRotation;
				resource.transform.localPosition = findMax.transform.localPosition + Vector3.right * 0.6f;
				resource.Start(); // call start again to fix texts

				Logger.LogDebug($"new item sign: {resource}");
				
				var arr = new ScanItemResourceModule[shop.scanItemResourceModules.Length + 1];
				Array.Copy(shop.scanItemResourceModules, 0, arr, 0, shop.scanItemResourceModules.Length);
				arr[arr.Length - 1] = resource;
				shop.cashRegister.resourceMachines = Array.ConvertAll(arr, e => (ResourceModule) e);

				resource.ItemPurchased += GlobalShopController.Instance.AddItemToInstatiationQueue;
			}

			Logger.LogDebug("done");
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

				Logger.LogDebug($"Resource load: {path}");

				if (path.StartsWith(prefix))
				{
					__result = _bundle.LoadAsset(path.Substring(prefix.Length));
					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(Inventory), nameof(Inventory.AddItemToInventory), typeof(GameObject), typeof(int)), HarmonyPrefix]
		static void Inventory_AddItemToInventory_Patch(GameObject item)
		{
			// would like to do this from magic item, but while there is a before remove there is no before add event

			Logger.LogDebug($"Add item to inventory: {item}");

			var lamp = item.GetComponent<MagicLamp>();
			if (lamp) lamp.Disable();
		}
	}
}