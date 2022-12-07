using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashMob_SurroundParameter", menuName = "TrashMob/SurroundParameter")]
public class SurroundParameterAICAC : ScriptableObject
{
    public float surroundSpeed;
    public float speedSmooth;
    public float stopSurroundDistance;
    public bool right;
    public bool left;

    public LayerMask mask;
}