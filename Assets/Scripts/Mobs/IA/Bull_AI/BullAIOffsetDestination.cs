using UnityEngine;

namespace State.AIBull
{
    public class BullAIOffsetDestination : MonoBehaviour
    {
        [Header("Offset AI Destination")]
        [SerializeField] public float offeset;
        [SerializeField] public float positiveOffeset;
        [SerializeField] public float negativeOffeset;

        BullAIStartPosRush bullAIStartPosRush;
        void Start()
        {
            Invoke("GetAiBullReference", 0.7f);
            Invoke("SetOffsetDestination", 1f);
        }

        void GetAiBullReference()
        {
            bullAIStartPosRush = GetComponent<BullAIStartPosRush>();
        }

        void SetOffsetDestination()
        {
            if (transform.childCount % 2 == 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (i % 2 == 0)
                    {
                        positiveOffeset += offeset;
                        bullAIStartPosRush.listBullAIScript[i].offsetDestination = positiveOffeset;
                    }
                    else
                    {
                        negativeOffeset -= offeset;
                        bullAIStartPosRush.listBullAIScript[i].offsetDestination = negativeOffeset;
                    }
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (i % 2 == 0)
                    {
                        bullAIStartPosRush.listBullAIScript[i].offsetDestination = positiveOffeset;
                        positiveOffeset += offeset;
                    }
                    else
                    {
                        negativeOffeset -= offeset;
                        bullAIStartPosRush.listBullAIScript[i].offsetDestination = negativeOffeset;
                    }
                }
            }

            positiveOffeset = 0;
            negativeOffeset = 0;
        }
    }
}