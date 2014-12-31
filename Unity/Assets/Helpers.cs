using UnityEngine;
using System.Collections;

public class Helpers
{

    public static Bounds GetBoundsOfChildren(GameObject parent)
    {
        var totalBounds = parent.GetComponentInChildren<MeshRenderer>().bounds;

        foreach (var c in parent.GetComponentsInChildren<MeshRenderer>())
        {
            totalBounds.Encapsulate(c.bounds);
        }

        return totalBounds;
    }
}
