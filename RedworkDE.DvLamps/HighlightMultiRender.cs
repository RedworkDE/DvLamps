using DV.CabControls;
using Facepunch;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace RedworkDE.DvLamps
{
	class HighlightMultiRender
	{
		[HarmonyPatch(typeof(Highlighter), nameof(Highlighter.OnHover)), HarmonyPrefix]
		static bool OnHover(Highlighter __instance, GameObject go)
		{
			var tag = go.GetComponentInChildren<HighlightTag>();
			var renderers = !tag ? go.GetComponentsInChildren<Renderer>() : tag.GetObject().GetComponentsInChildren<Renderer>();

			if (renderers.Length != 0)
			{
				Highlight.Instance.ImageEffectMaterial = __instance.originalMaterial;
				for (int i = 0; i < renderers.Length; i++)
					Highlight.AddRenderer(renderers[i]);
				Highlight.Rebuild();
			}
			else
			{
				Highlight.ClearAll();
				Highlight.Rebuild();
			}
			var hoverReactions = go.GetComponents<IHoverReaction>();
			for (int i = 0; i < hoverReactions.Length; i++)
				hoverReactions[i].OnHovered();

			return false;
		}

		[HarmonyPatch(typeof(HighlightNearbyItems), nameof(HighlightNearbyItems.FindNearbyRenderers)), HarmonyPrefix]
		static bool FindNearbyRenderers(HighlightNearbyItems __instance)
		{
			__instance.currentlyHighlighted.Clear();
			Transform reference = __instance.GetReference();
			if ((bool)reference)
			{
				var collection = Physics.OverlapSphere(reference.position, 12f, __instance.layerMask)
					.Select(GetGrabbableComponent)
					.Where((MonoBehaviour obj) => obj != null)
					.Distinct()
					.SelectMany(GetRenderer)
					.Where((Renderer rend) => rend != null);
				__instance.currentlyHighlighted.AddRange(collection);
			}

			return false;
		}

		private static MonoBehaviour GetGrabbableComponent(Collider col)
		{
			if (VRManager.IsVREnabled())
			{
				return col.GetComponentInParent<Telegrabbable>();
			}
			return col.GetComponentInParent<ItemBase>();
		}

		private static Renderer[] GetRenderer(MonoBehaviour obj)
		{
			HighlightTag componentInChildren = obj.GetComponentInChildren<HighlightTag>();
			if (componentInChildren && (componentInChildren.overrideDistance == 0f || Vector3.Magnitude(PlayerManager.PlayerTransform.position - obj.transform.position) < componentInChildren.overrideDistance))
			{
				return componentInChildren.GetObject()?.GetComponentsInChildren<Renderer>();
			}
			return obj.GetComponentsInChildren<Renderer>();
		}
	}
}
