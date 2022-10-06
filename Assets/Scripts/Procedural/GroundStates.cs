using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStates : MonoBehaviour
{
    [SerializeField] private GameObject stairsBlock;
    [SerializeField] private GameObject stairs;

    /// <summary>
    /// destroy stairs
    /// </summary>
    public void UnlockStairs()
    {
        Destroy(stairsBlock);
    }

    /// <summary>
    /// destroy stairs
    /// </summary>
    public void SpawnStairs()
    {
        stairs.SetActive(true);
    }
}
