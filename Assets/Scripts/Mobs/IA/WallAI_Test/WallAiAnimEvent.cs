using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAiAnimEvent : MonoBehaviour
{
    [SerializeField] WallAI wallAI;

    ////////////////////////  ANIMATION EVENT \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    void StartAttack()
    {
        wallAI.StartAttack();
    }

    void EndAttack()
    {
        wallAI.EndAttack();
    }

    /*private void OnTriggerStay(Collider other)
    {
        Debug.Log("Va niquer ta mere");
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Va niquer ta mere");
            wallAI.orientation = other.transform.localEulerAngles.y - 90;
        }
    }*/
}