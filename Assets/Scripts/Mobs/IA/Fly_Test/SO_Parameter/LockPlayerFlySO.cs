using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlyLockPlayerParameter", menuName = "Fly/LockPlayerParameter")]
public class LockPlayerFlySO : ScriptableObject
{
    public float stopSpeed;
    public Vector3 destinationFinal;
    public float distStopLockPlayer;
}