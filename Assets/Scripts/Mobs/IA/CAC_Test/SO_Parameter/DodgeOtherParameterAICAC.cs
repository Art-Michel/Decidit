using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "TrashMob_DodgeOtherParameter", menuName = "TrashMob/DodgeOtherParameter")]
public class DodgeOtherParameterAICAC : ScriptableObject
{
    public float speed;
    public bool right;
    public bool left;

    public List<GameObject> listOtherAI = new List<GameObject>();

    public LayerMask mask;
}
