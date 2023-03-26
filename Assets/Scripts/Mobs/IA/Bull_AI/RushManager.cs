using System.Collections.Generic;
using UnityEngine;

namespace State.AIBull
{
    public class RushManager : MonoBehaviour
    {
        RushBullParameterSO rushBullSO;

        public List<GlobalRefBullAI> listRefBullAI = new List<GlobalRefBullAI>();
        public List<GlobalRefBullAI> cloneListRefBullAI = new List<GlobalRefBullAI>();

        [Header("CoolDown Rush")]
        public float currentRangeTimeRush;

        [SerializeField] float maxTimeNextCharge;
        [SerializeField] float currentTimeNextCharge;

        // Start is called before the first frame update
        void Start()
        {
            currentTimeNextCharge = maxTimeNextCharge;

            for (int i = 0; i < transform.childCount; i++)
            {
                listRefBullAI.Add(transform.GetChild(i).GetComponent<GlobalRefBullAI>());
            }

            rushBullSO = listRefBullAI[0].rushBullSO;

            GetAIList();
            ResetCoolDown();
        }

        void ResetCoolDown()
        {
            currentRangeTimeRush = (int)Random.Range(rushBullSO.rangeTimerRush.x, rushBullSO.rangeTimerRush.y);
        }
        void GetAIList()
        {
            cloneListRefBullAI = new(listRefBullAI);
        }

        // Update is called once per frame
        void Update()
        {
            if(currentRangeTimeRush >0)
            {
                if(!CheckIsRushing())
                    currentRangeTimeRush -= Time.deltaTime;
            }
            else
            {
                if(!CheckIsRushing())
                {
                    if(cloneListRefBullAI.Count >0)
                    {
                        if(cloneListRefBullAI.Count == transform.childCount) // aucun rusher selectionner : 
                        {
                            SelectAI();
                        }
                        else // rusher deja selectioné : 
                        {
                            if (currentTimeNextCharge > 0) // delay avant de selectioné le prochain : 
                                currentTimeNextCharge -= Time.deltaTime;
                            else
                                SelectAI();
                        }
                    }
                    else
                    {
                        ResetCoolDown();
                        GetAIList();
                    }
                }
            }
        }

        bool CheckIsRushing()
        {
            for (int i = 0; i < listRefBullAI.Count; i++)
            {
                if(listRefBullAI[i].launchRush)
                {
                    return true;
                }
            }
            return false;
        }

        void SelectAI()
        {
            currentTimeNextCharge = maxTimeNextCharge;

            int i = Random.Range(0, cloneListRefBullAI.Count - 1);
            if(cloneListRefBullAI[i].enemyHealth._hp >0)
            {
                if (!cloneListRefBullAI[i].agent.isOnOffMeshLink)
                {
                    cloneListRefBullAI[i].launchRush = true;
                    cloneListRefBullAI.RemoveAt(i);
                }
            }
        }

        public void RemoveDeadAI(GlobalRefBullAI globalRef)
        {
            listRefBullAI.Remove(globalRef);
            GetAIList();
        }
    }
}