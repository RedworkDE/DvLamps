using UnityEngine;

public class MagicLamp : MonoBehaviour
{
	public ParticleSystem particles;
	private Transform particleTracker;

	void Start()
	{
		particleTracker = new GameObject().transform;
		particleTracker.parent = particles.transform.parent;
		particleTracker.localPosition = particles.transform.localPosition;
		particleTracker.localRotation = particles.transform.localRotation;
		particles.transform.SetParent(PlayerManager.PlayerTransform, false);

	}

	void Update()
	{
		particles.transform.position = particleTracker.position;
		particles.transform.rotation = particleTracker.rotation;
	}

	public void OnEnable()
	{
		particles.Play(false);
	}

	public void OnDisable()
	{
		particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
	}
}
