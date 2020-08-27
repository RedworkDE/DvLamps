using DV.CabControls;
using DV.CabControls.Spec;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class FlashlightCommon : MonoBehaviour, IStateSave
{
    public Button buttonSpec;
	public Light light;
	public MeshRenderer lightMesh;
	
	private ButtonBase button;
	private ItemBase item;
	private ItemScrolling itemScrolling;

	private FlashlightState state;

	private void Start()
	{
		button = buttonSpec.GetComponent<ButtonBase>();
		button.Used += Button_Used;
		item = GetComponent<ItemBase>();
		item.Used += () => button.Use();

		if (VRManager.IsVREnabled())
			itemScrolling = gameObject.AddComponent<ItemScrollingVR>();
		else
			itemScrolling = gameObject.AddComponent<ItemScrollingNonVR>();

		SetState(FlashlightState.Enabled);
	}

	private void Button_Used()
	{
		SetState((FlashlightState)(((int)state + 1) % (int)FlashlightState.MAX_VALUE));
	}

	private void SetState(FlashlightState newState)
	{
		state = newState;
		switch (state)
		{
			case FlashlightState.Enabled:
				SetFlashlightOn(true);
				break;
			case FlashlightState.Disabled:
				SetFlashlightOn(false);
				break;
		}
	}

	private void SetFlashlightOn(bool on)
	{
		light.enabled = on;

		if (on)
		{
			lightMesh.material.EnableKeyword("_EMISSION");
		}
		else
		{
			lightMesh.material.DisableKeyword("_EMISSION");
		}
	}

	public JObject GetStateSaveData()
	{
		return new JObject()
		{
			{"state", new JValue(state)}
		};
	}

	public void SetStateSaveData(JObject data)
	{
		if (data is null) return;
		var stateVal = data["state"];
		state = stateVal.Value<FlashlightState>();
	}

	public enum FlashlightState
	{
		Enabled,
		Disabled,
		MAX_VALUE
	}
}
