using UnityEngine;

[CreateAssetMenu(fileName = "BaseMoveWallAIParameter", menuName = "WallAI/BaseMoveParameter")]
public class BaseMoveWallAISO : ScriptableObject
{
    [Header("*Speed Parameter")]
    public float speedMovement;

    [Header("*Search new Position")]
    public LayerMask mask;
    public bool findNewPos;
    public Vector3 newPos;
    public int selectedWall;

    [Header("*Attack Rate Parameter")]
    public float maxRateAttack;
    public float rateAttack;

    [Header("*Wall Crack Effect")]
    public GameObject wallCrackPrefab;
    public float decalage;
    public GameObject lastWallCrack;
    public float distSinceLast;
}