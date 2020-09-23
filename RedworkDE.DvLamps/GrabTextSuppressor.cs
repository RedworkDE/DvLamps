
using DV.CabControls.NonVR;
using HarmonyLib;

namespace RedworkDE.DvLamps
{
	class GrabTextSuppressor
	{
		[HarmonyPatch(typeof(ItemInteractionTextScanNonVr), nameof(ItemInteractionTextScanNonVr.Update)), HarmonyPrefix]
		static bool Patch(ItemInteractionTextScanNonVr __instance)
		{
			if (SingletonBehaviour<InteractionTextControllerNonVr>.Exists && !__instance.grabber.IsGrabbingOrInMouseLook() && __instance.ScanHit(false) && __instance.currentHit.rigidbody?.GetComponent<ItemNonVR>())
			{
				var grabber = __instance.currentHit.rigidbody?.GetComponent<AGrabHandler>();
				if (grabber && !grabber.interactionAllowed) return false;

				SingletonBehaviour<InteractionTextControllerNonVr>.Instance.DisplayText(InteractionInfoType.GrabItem);
			}

			return false;
		}
	}
}
