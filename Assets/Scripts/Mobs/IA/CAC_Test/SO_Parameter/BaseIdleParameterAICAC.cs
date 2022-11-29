using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashMob_BaseIdleParameter", menuName = "TrashMob/BaseIdleParameter")]
public class BaseIdleParameterAICAC : ScriptableObject
{
    [SerializeField] public float maxDelayIdleState;
    [SerializeField] public float currentDelayIdleState;
}
