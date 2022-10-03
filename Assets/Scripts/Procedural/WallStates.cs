using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallStates : MonoBehaviour
{
    bool canChangeWallState = true;

    private void Update()
    {
        #region create door if detect collidor
        if (canChangeWallState)
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, 1f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    hit[i].transform.gameObject.SetActive(false);
                    canChangeWallState = false;
                }
            }
        }
        #endregion
    }
}
