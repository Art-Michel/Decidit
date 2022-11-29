using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bull_BaseIdleParameter", menuName = "Bull/BaseIdleParameter")]
public class BaseIdleBullSO : ScriptableObject
{
    public float stopSpeed;
    public float transitionDurationMax;
    public float currentTransition;
}