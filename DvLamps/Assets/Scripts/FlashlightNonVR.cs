﻿using System;
using System.Collections;
using UnityEngine;

public class FlashlightNonVR : MonoBehaviour
{
	public static Action<Grabber> ThrowGrabber;
	public static KeyCode[] toggleFlashLightKeys = {KeyCode.P};
	
	[HideInInspector]
	public bool IsHeadAttached;
	public ToggleColliderGroup collider;
	private AGrabHandler grabHandler;
	private Grabber grabber;

	void Start()
	{
		if (VRManager.IsVREnabled())
		{
			Destroy(this);
			return;
		}

		grabHandler = GetComponent<AGrabHandler>();
		grabber = FindObjectOfType<Grabber>();
	}

	void Update()
	{
		if (toggleFlashLightKeys.IsDown())
		{
			if (IsHeadAttached)
			{
				IsHeadAttached = false; // avoid reenabling flashlight when adding to inventory

				if (!grabber.IsHoldingItem())
				{
					// if no item in hand, pick it up
					grabber.ForcePickItem(grabHandler);
				}
				else 
				{
					// otherwise add to inventory
					Inventory.Instance.AddItemToInventory(gameObject);
					// if this doesnt work, the item will just drop onto the ground
				}

				grabHandler.interactionAllowed = true;
				collider.Enable();
			}
			else if (grabHandler.IsGrabbed()) 
			{
				// only attach to head if held in hand
				// then drop it begin tracking the camera

				ThrowGrabber(grabber);
				grabHandler.interactionAllowed = false; // suppress being able to pick item up, while attached to head at some angles
				IsHeadAttached = true;
				collider.Disable();
			}
		}
	}

	void LateUpdate()
	{
		if (IsHeadAttached)
		{
			transform.position = Camera.main.transform.position + Camera.main.transform.rotation * Vector3.up * 0.3f;
			transform.rotation = Camera.main.transform.rotation;
		}
	}
	
	void OnDisable()
	{
		if (IsHeadAttached) 
		{
			CoroutineManager.Instance.Run(EnableObject());
		}
	}
	
	IEnumerator EnableObject() 
	{
		yield return null;
		gameObject.SetActive(true);
	}
}

