using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStates : MonoBehaviour
{
    [SerializeField] private GameObject stairsBlock;
    [SerializeField] private GameObject stairs;
    //private void Awake()
    //{
    //    #region destroy stairs roof if detect room upward
    //    Vector3 up = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
    //    Collider[] hit = Physics.OverlapSphere(up, 2f);
    //    if (hit.Length > 0)
    //    {
    //        for (int i = 0; i < hit.Length; i++)
    //        {
    //            if (hit[i].GetComponent<GroundStates>())
    //            {
    //                hit[i].GetComponent<GroundStates>().UnlockStairs();
    //                //Debug.Log("roof " + up);

    //                ////spawn stairs
    //                //SpawnStairs();
    //            }
    //        }
    //    }
    //    #endregion

    //    #region destroy stairs ground if detect room downward
    //    Vector3 down = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z);
    //    Collider[] hitDown = Physics.OverlapSphere(down, 2f);
    //    if (hitDown.Length > 0)
    //    {
    //        for (int i = 0; i < hitDown.Length; i++)
    //        {
    //            if (hitDown[i].GetComponent<GroundStates>())
    //            {
    //                UnlockStairs();
    //                //Debug.Log("ground " + down);

    //                ////spawn stairs
    //                //hitDown[i].GetComponent<GroundStates>().SpawnStairs();
    //            }
    //        }
    //    }
    //    #endregion
    //}

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
