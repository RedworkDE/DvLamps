using UnityEngine;
using VRTK;

public class FlashlightVR : MonoBehaviour
{
	private Transform _headset;

    void Start()
    {
	    if (!VRManager.IsVREnabled())
	    {
		    Destroy(this);
			return;
	    }

        _headset = VRTK_SDK_Bridge.GetHeadset();
    }

    void LateUpdate()
    {
	    if (IsInBelt())
	    {
			FlashlightCommon.OrientFlashlight(transform, _headset);
	    }
    }

    private bool IsInBelt()
    {
	    return transform.parent.GetComponent<VRTK_BeltSnapDropZone>();
    }
}
