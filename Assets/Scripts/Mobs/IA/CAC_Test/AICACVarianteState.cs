using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State.AICAC
{
    public class AICACVarianteState : _StateAICAC
    {
        [Header("Offset AI Destination")]
        [SerializeField] public float offesetAnticipation;
        [SerializeField] public float offesetBase;
        [SerializeField] public float positiveOffeset;
        [SerializeField] public float negativeOffeset;

        [Header("List AI Surround")]
        [SerializeField] List<GlobalRefAICAC> aiCACScriptsList = new List<GlobalRefAICAC>();
        [SerializeField] List<GlobalRefAICAC> aiCACSurroundSelectedList = new List<GlobalRefAICAC>();
        [SerializeField] List<SurroundParameterAICAC> listSurroundAICACSO = new List<SurroundParameterAICAC>();

        [Header("Number AI Surround")]
        [SerializeField] int maxAISurround;
        [SerializeField] int numberAISurrouned;
        int index = 0;

        [Header("CoolDown AI Surround")]
        [SerializeField] float currentCoolDownSurround;
        [SerializeField] float maxCoolDownSurround;

        [Header("CoolDown Anticip Direction")]
        [SerializeField] float currentCoolDownAnticipDirection;
        [SerializeField] float maxCoolDownAnticipDirection;
        [SerializeField] float currentCoolDownStopAnticipDirection;
        [SerializeField] float maxCoolDownStopAnticipDirection;
        [SerializeField] bool activeAnticip;
        [SerializeField] bool activeSurround;

        void Start()
        {
            SetListActiveAI();
        }

        void SetListActiveAI()
        {
            aiCACScriptsList.Clear();
            aiCACSurroundSelectedList.Clear();
            listSurroundAICACSO.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                aiCACScriptsList.Add(transform.GetChild(i).GetComponent<GlobalRefAICAC>());
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                for (int j = 0; j < transform.childCount; j++)
                {
                    aiCACScriptsList[j].listOtherCACAI.Add(transform.GetChild(i).gameObject);
                }
            }

            if (transform.childCount != 2)
                maxAISurround = Mathf.CeilToInt((float)aiCACScriptsList.Count / 2f);
            else
            {
                maxAISurround = 2;
            }

            for (int i = 0; i < aiCACScriptsList.Count; i++)
            {
                listSurroundAICACSO.Add(aiCACScriptsList[i].surroundAICACSO);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (activeAnticip)
                SetOffsetDestination();
            else
            {
                CoolDownAnticipDirection();
                SetOffsetAticipationDestination();
            }

            if(activeSurround)
            {
                CoolDownSurround();
                SetSurroundDirection();
            }
        }

        /// <summary>
        /// Anticip Lateral Direction Move
        /// </summary>
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
                for (int i = 0; i < transform.childCount; i++)
                {
                    aiCACScriptsList[i].offsetDestination = positiveOffeset;
                    positiveOffeset += offesetAnticipation;
                }
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                for (int i = 0; i < transform.childCount; i++)
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
            if (transform.childCount % 2 == 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (i % 2 == 0)
                    {
                        positiveOffeset += offesetBase;
                        //Debug.Log(i % 2);
                        aiCACScriptsList[i].offsetDestination = positiveOffeset;
                    }
                    else
                    {
                        negativeOffeset -= offesetBase;
                        //Debug.Log(i % 2);
                        aiCACScriptsList[i].offsetDestination = negativeOffeset;
                    }
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (i % 2 == 0)
                    {
                        //Debug.Log(i % 2);
                        aiCACScriptsList[i].offsetDestination = positiveOffeset;
                        positiveOffeset += offesetBase;
                    }
                    else
                    {
                        negativeOffeset -= offesetBase;
                        //Debug.Log(i % 2);
                        aiCACScriptsList[i].offsetDestination = negativeOffeset;
                    }
                }
            }

            positiveOffeset = 0;
            negativeOffeset = 0;
        }


        /// <summary>
        /// Surround Move
        /// </summary>
        void CoolDownSurround()
        {
            if (currentCoolDownSurround > 0)
            {
                currentCoolDownSurround -= Time.deltaTime;
            }
            else
            {
                currentCoolDownSurround = maxCoolDownSurround;
                SurroundPlayer();
            }
        }
        void SetSurroundDirection()
        {
            if (aiCACSurroundSelectedList.Count == 2)
            {
                if (aiCACSurroundSelectedList[0].surroundAICACSO.right && aiCACSurroundSelectedList[1].surroundAICACSO.right)
                {
                    aiCACSurroundSelectedList[1].surroundAICACSO.right = false;
                    aiCACSurroundSelectedList[1].surroundAICACSO.left = true;
                }
                else if (aiCACSurroundSelectedList[0].surroundAICACSO.left && aiCACSurroundSelectedList[1].surroundAICACSO.left)
                {
                    aiCACSurroundSelectedList[1].surroundAICACSO.left = false;
                    aiCACSurroundSelectedList[1].surroundAICACSO.right = true;
                }
            }
        }
        public void SurroundPlayer()
        {
            SelectedAI();
        }
        void SelectedAI()
        {
            for (int i = 0; i < maxAISurround; i++)
            {
                if (aiCACSurroundSelectedList.Count == 0) // aucune IA Selected
                {
                    SelectedFirstSurroundAI(index);
                }
                else
                {
                    SelectedOtherSurroundAI(index);
                }
            }

            for (int i = 0; i < aiCACSurroundSelectedList.Count; i++)
            {
                if (i == 1)
                {
                    if (aiCACSurroundSelectedList[0].surroundAICACSO.right)
                        aiCACSurroundSelectedList[1].surroundAICACSO.left = true;
                    else
                        aiCACSurroundSelectedList[1].surroundAICACSO.right = true;
                }
                aiCACSurroundSelectedList[i].ActiveState(StateControllerAICAC.AIState.SurroundPlayer);
            }
        }
        void SelectedFirstSurroundAI(int shortestDist)
        {
            for (int i = 0; i < aiCACScriptsList.Count - 1; i++)
            {
                if (aiCACScriptsList[i].distPlayer > aiCACScriptsList[i + 1].distPlayer)
                {
                    shortestDist = aiCACScriptsList.IndexOf(aiCACScriptsList[i]);
                }
                else
                {
                    shortestDist = aiCACScriptsList.IndexOf(aiCACScriptsList[i + 1]);
                }
            }
            if (aiCACSurroundSelectedList.Count < 2)
                aiCACSurroundSelectedList.Add(aiCACScriptsList[shortestDist]);
        }

        void SelectedOtherSurroundAI(int shortestDist)
        {
            for (int i = 0; i < aiCACScriptsList.Count - 1; i++)
            {
                if (aiCACScriptsList[i].distPlayer > aiCACScriptsList[i + 1].distPlayer)
                {
                    if (!aiCACSurroundSelectedList.Contains(aiCACScriptsList[i]))
                    {
                        shortestDist = aiCACScriptsList.IndexOf(aiCACScriptsList[i]);
                    }
                }
                else
                {
                    if (!aiCACSurroundSelectedList.Contains(aiCACScriptsList[i + 1]))
                        shortestDist = aiCACScriptsList.IndexOf(aiCACScriptsList[i + 1]);
                }
            }
            if (aiCACSurroundSelectedList.Count < 2)
                aiCACSurroundSelectedList.Add(aiCACScriptsList[shortestDist]);
        }
        public void RemoveAISelectedAnticip(GlobalRefAICAC globalRef)
        {
            aiCACScriptsList.Remove(globalRef);
        }
        public void RemoveAISelectedSurround(GlobalRefAICAC globalRef)
        {
            aiCACSurroundSelectedList.Remove(globalRef);
        }
    }
}