using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICACVarianteState : MonoBehaviour
{
    [Header("Offset AI Destination")]
    [SerializeField] public float offesetAnticipation;
    [SerializeField] public float offesetBase;
    [SerializeField] public float positiveOffeset;
    [SerializeField] public float negativeOffeset;

    [Header("List AI Surround")]
    [SerializeField] List<StateManagerAICAC> aiCACScriptsList= new List<StateManagerAICAC>();
    [SerializeField] List<StateManagerAICAC> aiCACSurroundSelectedList = new List<StateManagerAICAC>();
    [SerializeField] List<SurroundParameterAICAC> listSurroundParameterAICACSO = new List<SurroundParameterAICAC>();

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

    void Start()
    {
        SetListActiveAI();
    }

    public void SetListActiveAI()
    {
        aiCACScriptsList.Clear();
        aiCACSurroundSelectedList.Clear();
        listSurroundParameterAICACSO.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            aiCACScriptsList.Add(transform.GetChild(i).GetComponent<StateManagerAICAC>());
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                aiCACScriptsList[j].listOtherCACAI.Add(transform.GetChild(i).gameObject);
            }
        }

        if(transform.childCount != 2)
            maxAISurround = Mathf.CeilToInt((float)aiCACScriptsList.Count / 2f);
        else
        {
            maxAISurround = 2;
        }

        for (int i = 0; i < aiCACScriptsList.Count; i++)
        {
            listSurroundParameterAICACSO.Add(aiCACScriptsList[i].surroundParameterAICACSOInstance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!activeAnticip)
            CoolDownAnticipDirection();

        if (activeAnticip)
            SetOffsetDestination();
        else
            SetOffsetAticipationDestination();


        CoolDownSurround();
        SetSurroundDirection();
    }

    /// <summary>
    /// Anticip Lateral Direction Move
    /// </summary>
    void CoolDownAnticipDirection()
    {
        if(currentCoolDownAnticipDirection >0)
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
                aiCACScriptsList[i].baseMoveParameterAICACSOInstance.activeAnticipDestination = true;
            }
            activeAnticip = true;
            currentCoolDownAnticipDirection = maxCoolDownAnticipDirection;
        }
    }
    void CoolDownStopAnticipDirection()
    {
        if(currentCoolDownStopAnticipDirection >0)
        {
            currentCoolDownStopAnticipDirection -= Time.deltaTime;
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                aiCACScriptsList[i].baseMoveParameterAICACSOInstance.activeAnticipDestination = false;
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
                    Debug.Log(i % 2);
                    aiCACScriptsList[i].offsetDestination = positiveOffeset;
                }
                else
                {
                    negativeOffeset -= offesetBase;
                    Debug.Log(i % 2);
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
                    Debug.Log(i % 2);
                    aiCACScriptsList[i].offsetDestination = positiveOffeset;
                    positiveOffeset += offesetBase;
                }
                else
                {
                    negativeOffeset -= offesetBase;
                    Debug.Log(i % 2);
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
        if(currentCoolDownSurround >0)
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
            if (aiCACSurroundSelectedList[0].surroundParameterAICACSOInstance.right && aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.right)
            {
                aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.right = false;
                aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.left = true;
            }
            else if (aiCACSurroundSelectedList[0].surroundParameterAICACSOInstance.left && aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.left)
            {
                aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.left = false;
                aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.right = true;
            }
        }
    }
    public void SurroundPlayer()
    {
        SelectedAI();
    }
    void SelectedAI()
    {
        for (int i =0; i < maxAISurround; i++)
        {
            if(aiCACSurroundSelectedList.Count == 0) // aucune IA Selected
            {
                SelectedFirstSurroundAI(index);
            }
            else
            {
                SelectedOtherSurroundAI(index);
            }
        }

        for(int i =0; i < aiCACSurroundSelectedList.Count; i++)
        {
            if (i == 1)
            {
                if (aiCACSurroundSelectedList[0].surroundParameterAICACSOInstance.right)
                    aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.left = true;
                else
                    aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.right = true;
            }

            aiCACSurroundSelectedList[i].SwitchToNewState(5);
        }
    }
    void SelectedFirstSurroundAI(int shortestDist)
    {
        for (int i = 0; i < aiCACScriptsList.Count-1; i++)
        {
            if(aiCACScriptsList[i].distplayer > aiCACScriptsList[i+1].distplayer)
            {
                shortestDist = aiCACScriptsList.IndexOf(aiCACScriptsList[i]);
            }
            else
            {
                shortestDist = aiCACScriptsList.IndexOf(aiCACScriptsList[i+1]);
            }
        }
        if(aiCACSurroundSelectedList.Count < 2)
            aiCACSurroundSelectedList.Add(aiCACScriptsList[shortestDist]);
    }

    void SelectedOtherSurroundAI(int shortestDist)
    {
        for (int i = 0; i < aiCACScriptsList.Count - 1; i++)
        {
            if (aiCACScriptsList[i].distplayer > aiCACScriptsList[i + 1].distplayer)
            {
                if(!aiCACSurroundSelectedList.Contains(aiCACScriptsList[i]))
                {
                    shortestDist = aiCACScriptsList.IndexOf(aiCACScriptsList[i]);
                }
            }
            else
            {
                if (!aiCACSurroundSelectedList.Contains(aiCACScriptsList[i+1]))
                    shortestDist = aiCACScriptsList.IndexOf(aiCACScriptsList[i + 1]);
            }
        }
        if (aiCACSurroundSelectedList.Count < 2)
            aiCACSurroundSelectedList.Add(aiCACScriptsList[shortestDist]);
    }
    public void RemoveAISelected(StateManagerAICAC stateManagerAICAC)
    {
        Debug.Log(aiCACSurroundSelectedList.Count);
        aiCACSurroundSelectedList.Remove(stateManagerAICAC);
    }
}