using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "TrashMob_DodgeParameter", menuName = "TrashMob/DodgeParameter")]
public class DodgeParameterAICAC : ScriptableObject
{
    [Header("Rotation Parameter")]
    [SerializeField] public float speedRot;
    [SerializeField] public float maxSpeedRot;
    [SerializeField] public float smoothRot;

    [Header("Movement Parameter")]
    public float dodgeLenght;
    public float dodgeSpeed;
    public bool isDodging = false;
    public bool rightDodge = false;
    public bool leftDodge = false;
    public bool dodgeIsSet = false;
    public Vector3 targetDodgeVector = Vector3.zero;
    public NavMeshHit navHit;
    public LayerMask noMask;
    public Transform targetObjectToDodge;
}