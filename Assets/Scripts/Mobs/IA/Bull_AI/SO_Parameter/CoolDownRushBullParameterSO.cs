using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bull_CoolDownRushParameter", menuName = "Bull/CoolDownParameter")]
public class CoolDownRushBullParameterSO : ScriptableObject
{
    public float stopSpeed;
    public float currentCoolDownRush;
    public float coolDownRush;
    public float rushInertieSetDistance;
    public Vector3 rushDestination;
}