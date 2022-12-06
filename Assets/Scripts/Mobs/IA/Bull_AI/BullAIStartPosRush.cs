using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BullAIStartPosRush : MonoBehaviour
{
    RaycastHit hit;
    Ray ray;
    LayerMask noMask;

    public List<BullAI> listBullAIScript = new List<BullAI>();
    public List<BoxCollider> listBounds = new List<BoxCollider>();
    public List<BoxCollider> listSelectedBox = new List<BoxCollider>();
    public NavMeshData navMeshData;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            listBullAIScript.Add(transform.GetChild(i).GetComponent<BullAI>());
        }

        listSelectedBox = listBounds;
    }

    public void SelectAI(BullAI bullAI)
    {
        int indexBox = Random.Range(0, listSelectedBox.Count - 1);
        CheckPosition(bullAI, listBounds[indexBox]);
        listSelectedBox.RemoveAt(indexBox);
    }

    void CheckPosition(BullAI bullAI, BoxCollider boxSelected)
    {
        Vector3 boundsForwardPos = new Vector3(Random.Range(boxSelected.bounds.min.x, boxSelected.bounds.max.x),
                                         navMeshData.sourceBounds.center.y,
                                         Random.Range(boxSelected.bounds.min.z, boxSelected.bounds.max.z));

        bullAI.coolDownRushBullParameterSOInstance.startPos = CheckNavMeshPoint(boundsForwardPos);
        bullAI.coolDownRushBullParameterSOInstance.boxSelected = boxSelected;
    }

    Vector3 CheckNavMeshPoint(Vector3 boundsForwardPos)
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(boundsForwardPos, out closestHit, 20, 1))
        {
            boundsForwardPos = closestHit.position;
        }
        return boundsForwardPos;
    }

    public void ResetSelectedBox(BoxCollider boxColliderSelected)
    {
        listSelectedBox.Add(boxColliderSelected);
    }
}