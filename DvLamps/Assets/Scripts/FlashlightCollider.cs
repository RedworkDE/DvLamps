using DV.CabControls;
using UnityEngine;

public class FlashlightCollider : MonoBehaviour
{
    void Start()
    {
        foreach (var obj in GetComponentInParent<ItemBase>().GetComponentsInChildren<MeshFilter>())
        {
            var col = gameObject.AddComponent<MeshCollider>();
            col.sharedMesh = obj.mesh;
            col.convex = true;
        }
    }
}
