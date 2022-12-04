using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bull_BaseMoveParameter", menuName = "Bull/BaseMoveParameter")]
public class BaseMoveBullParameterSO : ScriptableObject
{
    public float baseSpeed;
    public float maxCoolDownRush;
    public float currentCoolDownWaitRush;
}