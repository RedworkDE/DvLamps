using UnityEngine;
using VRTK;

namespace RedworkDE.DvLamps
{
	public class BeltLoader : MonoBehaviour
	{

		[AutoLoad(LoadTime.GameLoaded)]
		private static void Loaded()
		{
			if (!VRManager.IsVREnabled()) return;

			var belt = FindObjectOfType<VRTK_HipTracking>();

			// copy the belt to make head tracked slots
			var head = Instantiate(belt, belt.transform.parent, true);
			head.HeadOffset = 0;

			// disable all except for the center one
			for (int i = 1; i < head.transform.childCount; i++) head.transform.GetChild(i).gameObject.SetActive(false);

			head.transform.GetChild(0).localPosition = Vector3.up * 0.3f + Vector3.forward * 0.1f;

			// delete possible items in the slot, that shouldn't be there
			var drop = head.transform.GetChild(0).GetChild(0);
			for (int i = 1; i < drop.childCount; i++) Destroy(drop.GetChild(i).gameObject);

			belt.gameObject.AddComponent<BeltLoader>(); // marker to differentiate this 
		}
	}
}