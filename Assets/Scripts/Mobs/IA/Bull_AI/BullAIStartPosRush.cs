using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace State.AIBull
{
    public class BullAIStartPosRush : MonoBehaviour
    {
        public List<GlobalRefBullAI> listBullAIScript = new List<GlobalRefBullAI>();
        [SerializeField] List<BoxCollider> listBounds = new List<BoxCollider>();
        [SerializeField] List<BoxCollider> listSelectedBox;
        [SerializeField] NavMeshData navMeshData;

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                try
                {
                    listBullAIScript.Add(transform.GetChild(i).GetComponent<GlobalRefBullAI>());
                }
                catch
                {
                    Debug.LogError("Miss BullAI component");
                }
            }

            listSelectedBox = new List<BoxCollider>(listBounds);
        }

        public void SelectAI(GlobalRefBullAI bullAI)
        {
            try
            {
                int indexBox = Random.Range(0, listSelectedBox.Count - 1);
                CheckPosition(bullAI, listBounds[indexBox]);
                listSelectedBox.Remove(listBounds[indexBox]);
            }
            catch
            {
                Debug.LogError("List Empty");
                listSelectedBox = new List<BoxCollider>(listBounds);
            }
        }

        void CheckPosition(GlobalRefBullAI bullAI, BoxCollider boxSelected)
        {
            Vector3 boundsForwardPos = new Vector3(Random.Range(boxSelected.bounds.min.x, boxSelected.bounds.max.x),
                                             navMeshData.sourceBounds.center.y,
                                             Random.Range(boxSelected.bounds.min.z, boxSelected.bounds.max.z));

            bullAI.coolDownRushBullSO.startPos = CheckNavMeshPoint(boundsForwardPos);
            bullAI.coolDownRushBullSO.boxSelected = boxSelected;
        }

        Vector3 CheckNavMeshPoint(Vector3 boundsForwardPos)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(boundsForwardPos, out closestHit, 1, 1))
            {
                boundsForwardPos = closestHit.position;
            }
            return boundsForwardPos;
        }

        public void ResetSelectedBox(BoxCollider boxColliderSelected)
        {
            if(listSelectedBox.Count < listBounds.Count)
                listSelectedBox.Add(boxColliderSelected);
        }
    }
}