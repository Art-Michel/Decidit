using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RushParameter", menuName = "Bull/RushParameter")]
public class RushBullParameterSO : ScriptableObject
{
    public float rushSpeed;
    public int damageRushAttack;
    public Vector3 rushDestination;
    public float rushInertieSetDistance;
    public bool stopLockPlayer;
    public BoxCollider detectOtherAICollider;
    public List<GameObject> ennemiInCollider = new List<GameObject>();
    public RaycastHit hit;
    public LayerMask mask;
}
