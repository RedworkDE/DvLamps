using UnityEngine;
using VRTK;

public class FlashlightVR : MonoBehaviour
{
	public float RotateSpeed;
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
		    transform.rotation = Quaternion.RotateTowards(transform.rotation, _headset.rotation, RotateSpeed * Time.deltaTime);
	    }
    }

    private bool IsInBelt()
    {
	    return transform.parent.GetComponent<VRTK_BeltSnapDropZone>();
    }
}
