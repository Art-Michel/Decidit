using UnityEngine;

[CreateAssetMenu(fileName = "AttractionParameter", menuName = "AttractionState/AttractionParameter")]
public class AttractionSO : ScriptableObject
{
    [Header("Movement")]
    public float speed;
    public float distanceStopAttraction;
    public float friction;

    public Vector3 pointBlackHole;
}