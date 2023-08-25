using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Objects
{
    public static Transform resourceTransform;
    public static Queue<Transform> availableResources = new Queue<Transform>();

    public static void MakeAllAvailable()
    {
        foreach (Transform child in resourceTransform)
        {
            availableResources.Enqueue(child);
        }
    }

    public static Transform GetAvailableProduct()
    {
        if (availableResources.Count > 0)
        {
            return availableResources.Dequeue();
        }
        return null;
    }



    public static bool IsAvailable()
    {
        return availableResources.Count > 0;
    }
}