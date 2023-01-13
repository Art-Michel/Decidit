using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavLinkState : MonoBehaviour
{
    [SerializeField] bool isActive;

    NavMeshLinkData navLinkData;
    NavMeshLink navLink;
    [SerializeField] OffMeshLink offMeshLink;

    // Start is called before the first frame update
    void Start()
    {
        navLink = GetComponent<NavMeshLink>();
        navLinkData = GetComponent<NavMeshLinkData>();
    }

    void Update()
    {
    }
}