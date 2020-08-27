using System;
using UnityEngine;

public class ToggleColliderGroup : MonoBehaviour
{
    public GameObject[] colliders;

    public void Enable()
    {
        Array.ForEach(colliders, c => c.SetActive(false));
    }

    public void Disable()
    {
        Array.ForEach(colliders, c => c.SetActive(true));
    }
}