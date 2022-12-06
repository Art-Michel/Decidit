using UnityEngine;

[CreateAssetMenu(fileName = "Bull_CoolDownRushParameter", menuName = "Bull/CoolDownParameter")]
public class CoolDownRushBullParameterSO : ScriptableObject
{
    [Header("Speed Rotation Parameter")]
    [SerializeField] public float speedRot;
    [SerializeField] public float maxSpeedRot;
    [SerializeField] public float smoothRot;

    [Header("Movement Parameter")]
    public float stopSpeed;
    public float speedGoToStartPos;
    public float currentCoolDownRush;
    public float coolDownRush;
    public float rushInertieSetDistance;
    public Vector3 rushDestination;
    public Vector3 startPos;
    public BoxCollider boxSelected;
}