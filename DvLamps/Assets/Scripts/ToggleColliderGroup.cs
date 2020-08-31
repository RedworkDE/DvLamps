using System;
using UnityEngine;

public class ToggleColliderGroup : MonoBehaviour
{
    public GameObject[] colliders;

    public void SetActive(bool value)
    {
        Array.ForEach(colliders, c => c.SetActive(value));
    }
}