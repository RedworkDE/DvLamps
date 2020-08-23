using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedworkDE.DvLamps
{
	class StorageExtender
	{
		private static bool _savingStorage;

		// this MUST appear before the RequestStorageSaveGameData because of inlineing
		[HarmonyPatch(typeof(Inventory), nameof(Inventory.GetItemsArray)), HarmonyPostfix]
		public static void Inventory_GetItemsArray_Postfix(ref GameObject[] __result)
		{
			if (!_savingStorage) return;

			var vr = VRManager.IsVREnabled();

			var go = default(GameObject);
			foreach (var item in StorageController.Instance.StorageWorld.itemList)
			{
				if (vr)
				{
					if (item.GetComponentInParent<BeltLoader>())
					{
						go = item.gameObject;
						break;
					}
				}
				else
				{
					var flashLight = item.GetComponent<FlashlightNonVR>();
					if (!flashLight) continue;
					if (flashLight.IsHeadAttached)
					{
						go = flashLight.gameObject;
						break;
					}
				}
			}

			var empty = Array.IndexOf(__result, null);
			if (empty == -1) return;
			var copy = __result.ToArray();
			copy[empty] = go;
			__result = copy;

			Log.Debug($"Adding head attached item to inventory {go}");
		}

		[HarmonyPatch(typeof(StorageBase), nameof(StorageBase.RequestStorageSaveGameData)), HarmonyPrefix]
		public static void StorageBase_RequestStorageSaveGameData_Prefix(StorageBase __instance)
		{
			_savingStorage = true;
		}

		[HarmonyPatch(typeof(StorageBase), nameof(StorageBase.RequestStorageSaveGameData)), HarmonyPostfix]
		public static void StorageBase_RequestStorageSaveGameData_Postfix(StorageBase __instance, List<StorageItemData> __result)
		{
			_savingStorage = false;

			if (__instance.storageType != StorageType.World) return;

			var vr = VRManager.IsVREnabled();

			var go = -1;
			var num = 0;
			foreach (var item in __instance.itemList)
			{
				if (StorageBase.itemTypesExcludedFromSave.Any(t => item.GetComponent(t))) continue;

				if (vr)
				{
					if (item.GetComponentInParent<BeltLoader>())
					{
						go = num;
						break;
					}
				}
				else
				{
					var flashLight = item.GetComponent<FlashlightNonVR>();
					if (!flashLight) continue;
					if (flashLight.IsHeadAttached)
					{
						go = num;
						break;
					}
				}
				num++;
			}

			if (go == -1) return;
			if (Array.IndexOf(Inventory.Instance.items, null) == -1) return;

			Log.Debug("Removing head-attached item from world");

			__result.RemoveAt(go);
		}
	}
}
