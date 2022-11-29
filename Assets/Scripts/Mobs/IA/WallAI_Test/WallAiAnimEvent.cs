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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            wallAI.orientation = other.transform.eulerAngles.y - 90;
        }
    }
}