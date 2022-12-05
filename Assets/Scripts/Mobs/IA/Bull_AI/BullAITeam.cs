using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BullAITeam : MonoBehaviour
{
    RaycastHit hit;
    Ray ray;
    LayerMask noMask;

    [SerializeField] List<BullAI> listBullAIScript = new List<BullAI>();
    [SerializeField] List<BoxCollider> listBounds = new List<BoxCollider>();
    [SerializeField] List<BoxCollider> listSelectedBox = new List<BoxCollider>();
    [SerializeField] NavMeshData navMeshData;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            listBullAIScript.Add(transform.GetChild(i).GetComponent<BullAI>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i =0; i < listBullAIScript.Count; i++)
        {
            int indexBox = Random.Range(0, listBounds.Count-1);

            while(listBounds.Contains(listSelectedBox[indexBox]))
            {
                indexBox = Random.Range(0, listBounds.Count - 1);
                listSelectedBox.Add(listBounds[i]);
            }

            CheckPosition(listBullAIScript[i], listBounds[indexBox]);
        }
    }

    void CheckPosition(BullAI bullAI, BoxCollider boxSelected)
    {
        Vector3 boundsForwardPos = new Vector3(Random.Range(boxSelected.bounds.min.x, boxSelected.bounds.max.x),
                                         navMeshData.sourceBounds.center.y,
                                         Random.Range(boxSelected.bounds.min.z, boxSelected.bounds.max.z));

        CheckNavMeshPoint(boundsForwardPos);
        bullAI.coolDownRushBullParameterSOInstance.startPos = boundsForwardPos;
        bullAI.coolDownRushBullParameterSOInstance.boxSelected = boxSelected;
    }

    void CheckNavMeshPoint(Vector3 boundsForwardPos)
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(boundsForwardPos, out closestHit, 20, 1))
        {
            boundsForwardPos = closestHit.position;
        }

        transform.position = boundsForwardPos;
    }

    public void ResetSelectedBox(BoxCollider boxColliderSelected)
    {
        listSelectedBox.Remove(boxColliderSelected);
    }
}