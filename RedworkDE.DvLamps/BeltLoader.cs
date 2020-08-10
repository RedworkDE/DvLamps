using UnityEngine;
using VRTK;

namespace RedworkDE.DvLamps
{
	public class BeltLoader
	{

		[AutoLoad(LoadTime.GameLoaded)]
		private static void Loaded()
		{
			if (!VRManager.IsVREnabled()) return;

			var belt = Object.FindObjectOfType<VRTK_HipTracking>();

			// copy the belt to make head tracked slots
			var head = Object.Instantiate(belt, belt.transform.parent, true);
			head.HeadOffset = 0;

			// disable all except for the center one
			for (int i = 1; i < head.transform.childCount; i++) head.transform.GetChild(i).gameObject.SetActive(false);

			head.transform.GetChild(0).localPosition = Vector3.up * 0.3f;

			// delete possible items in the slot, that shouldn't be there
			var drop = head.transform.GetChild(0).GetChild(0);
			for (int i = 1; i < drop.childCount; i++) Object.Destroy(drop.GetChild(i).gameObject);


		}
	}
}