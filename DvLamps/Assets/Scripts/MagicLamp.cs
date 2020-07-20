using UnityEngine;

public class MagicLamp : MonoBehaviour
{
	public ParticleSystem _particles;

	void Start()
	{
		Inventory.Instance.ItemRemovedFromInventory += (go, n) =>
		{
			Debug.Log("Item removed from inventory: " + go);

			var lamp = go.GetComponent<MagicLamp>();
			if (lamp) lamp.Enable();
		};
	}

	void Awake()
	{
		_particles.Play(false);
	}

	public void Enable()
	{
		_particles.transform.SetParent(transform, false);
		_particles.Play(false);
	}

	public void Disable()
	{
		_particles.transform.SetParent(PlayerManager.PlayerTransform, false);
		_particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
	}
}
