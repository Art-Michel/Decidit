using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAIManager : MonoBehaviour
{
    static RaycastAIManager instance;

    static RaycastHit hit;

    static bool isActive;
    private void Awake()
    {
        instance = GetComponent<RaycastAIManager>();
    }

    static public RaycastHit RaycastAI(Vector3 origine, Vector3 direction, LayerMask mask, Color colorDebug, float lenght)
    {
        Ray ray;
        RaycastHit hit;

        ray = new Ray(origine, direction);
        Physics.Raycast(ray, out hit, lenght, mask);

        Debug.DrawRay(origine, direction*lenght, colorDebug);
        return hit;
    }
}