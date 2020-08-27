using DV.CabControls;
using DV.CabControls.Spec;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class FlashlightCommon : MonoBehaviour, IStateSave
{
    public Button buttonSpec;
	public Light light;
	public MeshRenderer lightMesh;

	public MeshRenderer[] meshes;
	public Texture[] variants;
	private int variant;

	private ButtonBase button;
	private ItemBase item;
	private ItemScrolling itemScrolling;

	private FlashlightState state;

	void Awake()
	{
		SetState(FlashlightState.Enabled);
		SetVariant(-1);
	}

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
	}

	private void Button_Used()
	{
		SetState((FlashlightState)(((int)state + 1) % (int)FlashlightState.MAX_VALUE));
	}

	private void SetState(FlashlightState newState)
	{
		Debug.Log("set state: " + newState);

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

	private void SetVariant(int num)
	{
		variant = num < 0 || num >= variants.Length ? Random.Range(0, variants.Length) : num;
		foreach (var mesh in meshes)
			mesh.material.SetTexture("_MainTex", variants[variant]);
	}

	public JObject GetStateSaveData()
	{
		return JObject.FromObject(new SaveData()
		{
			state = state,
			variant = variant
		});
	}

	public void SetStateSaveData(JObject data)
	{
		if (data is null) return;
		var stateData = data.ToObject<SaveData>();
		SetState(stateData.state);
		SetVariant(stateData.variant);
	}

	public enum FlashlightState
	{
		Enabled,
		Disabled,
		MAX_VALUE
	}

	public class SaveData
	{
		public FlashlightState state = FlashlightState.Enabled;
		public int variant = -1;
	}
}
