using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State.AICAC
{
    public class ManagerAnticipMoveTrash : MonoBehaviour
    {
        [SerializeField] List<GlobalRefAICAC> aiCACScriptsList = new List<GlobalRefAICAC>();

        [Header("Offset AI Destination")]
        [SerializeField] public float offesetAnticipation;
        [SerializeField] public float offesetBase;
        [SerializeField] public float positiveOffeset;
        [SerializeField] public float negativeOffeset;

        [Header("CoolDown Anticip Direction")]
        [SerializeField] float currentCoolDownAnticipDirection;
        [SerializeField] float maxCoolDownAnticipDirection;
        [SerializeField] float currentCoolDownStopAnticipDirection;
        [SerializeField] float maxCoolDownStopAnticipDirection;
        [SerializeField] bool activeAnticip;

        void Start()
        {

        }

        public void GetRef(GlobalRefAICAC globalRefAICAC)
        {
            aiCACScriptsList.Add(globalRefAICAC);
        }
        public void RemoveRef(GlobalRefAICAC globalRefAICAC)
        {
            aiCACScriptsList.Remove(globalRefAICAC);
        }

        void Update()
        {
            if (activeAnticip)
                SetOffsetDestination();
            else
            {
                CoolDownAnticipDirection();
                SetOffsetAticipationDestination();
            }
        }

        void CoolDownAnticipDirection()
        {
            if (currentCoolDownAnticipDirection > 0)
            {
                if (Input.GetAxis("Horizontal") != 0)
                {
                    currentCoolDownAnticipDirection -= Time.deltaTime;
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    aiCACScriptsList[i].baseMoveAICACSO.activeAnticipDestination = true;
                }
                activeAnticip = true;
                currentCoolDownAnticipDirection = maxCoolDownAnticipDirection;
            }
        }

        void CoolDownStopAnticipDirection()
        {
            if (currentCoolDownStopAnticipDirection > 0)
            {
                currentCoolDownStopAnticipDirection -= Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    aiCACScriptsList[i].baseMoveAICACSO.activeAnticipDestination = false;
                }
                activeAnticip = false;
                currentCoolDownStopAnticipDirection = maxCoolDownStopAnticipDirection;
            }
        }

        void SetOffsetDestination()
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                for (int i = 0; i < aiCACScriptsList.Count; i++)
                {
                    aiCACScriptsList[i].offsetDestination = positiveOffeset;
                    positiveOffeset += offesetAnticipation;
                }
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                for (int i = 0; i < aiCACScriptsList.Count; i++)
                {
                    aiCACScriptsList[i].offsetDestination = positiveOffeset;
                    positiveOffeset -= offesetAnticipation;
                }
            }
            else
            {
                CoolDownStopAnticipDirection();
            }

            positiveOffeset = 0;
            negativeOffeset = 0;
        }

        void SetOffsetAticipationDestination()
        {
            if (aiCACScriptsList.Count % 2 == 0)
            {
                for (int i = 0; i < aiCACScriptsList.Count; i++)
                {
                    if (i <= aiCACScriptsList.Count)
                    {
                        if (i % 2 == 0)
                        {
                            if (aiCACScriptsList.Contains(aiCACScriptsList[i]))
                            {
                                positiveOffeset += offesetBase;
                                aiCACScriptsList[i].offsetDestination = positiveOffeset;
                            }
                        }
                        else
                        {
                            if (aiCACScriptsList.Contains(aiCACScriptsList[i]))
                            {
                                negativeOffeset -= offesetBase;
                                aiCACScriptsList[i].offsetDestination = negativeOffeset;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Object was destroy");
                    }
                }
            }
            else
            {
                for (int i = 0; i < aiCACScriptsList.Count; i++)
                {
                    if (i <= aiCACScriptsList.Count)
                    {
                        if (i % 2 == 0)
                        {
                            if (aiCACScriptsList.Contains(aiCACScriptsList[i]))
                            {
                                aiCACScriptsList[i].offsetDestination = positiveOffeset;
                                positiveOffeset += offesetBase;
                            }
                        }
                        else
                        {
                            if (aiCACScriptsList.Contains(aiCACScriptsList[i]))
                            {
                                negativeOffeset -= offesetBase;
                                aiCACScriptsList[i].offsetDestination = negativeOffeset;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Object was destroy");
                    }
                }
            }

            positiveOffeset = 0;
            negativeOffeset = 0;
        }
    }
}