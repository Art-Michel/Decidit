using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State.AIBull
{
    public class ManagerShrednoss : MonoBehaviour
    {
        [Header("Offset AI Destination")]
        [SerializeField] public float offeset;
        [SerializeField] public float positiveOffeset;
        [SerializeField] public float negativeOffeset;

        public List<GlobalRefBullAI> listRefBullAI = new List<GlobalRefBullAI>();

        void Start()
        {
           /* for (int i = 0; i < transform.childCount; i++)
            {
                listRefBullAI.Add(transform.GetChild(i).GetComponent<GlobalRefBullAI>());
            }*/
        }
        public void GetRef(GlobalRefBullAI globalRefBullAI)
        {
            listRefBullAI.Add(globalRefBullAI);
        }
        public void RemoveRef(GlobalRefBullAI globalRefAICAC)
        {
            listRefBullAI.Remove(globalRefAICAC);
        }


        private void Update()
        {
            SetOffsetDestination();
        }

        void SetOffsetDestination()
        {
            if (listRefBullAI.Count % 2 == 0)
            {
                for (int i = 0; i < listRefBullAI.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        positiveOffeset += offeset;
                        listRefBullAI[i].offsetDestination = positiveOffeset;
                    }
                    else
                    {
                        negativeOffeset -= offeset;
                        listRefBullAI[i].offsetDestination = negativeOffeset;
                    }
                }
            }
            else
            {
                for (int i = 0; i < listRefBullAI.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        listRefBullAI[i].offsetDestination = positiveOffeset;
                        positiveOffeset += offeset;
                    }
                    else
                    {
                        negativeOffeset -= offeset;
                        listRefBullAI[i].offsetDestination = negativeOffeset;
                    }
                }
            }

            positiveOffeset = 0;
            negativeOffeset = 0;
        }
    }

}