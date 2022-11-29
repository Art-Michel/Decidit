using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICACVarianteState : MonoBehaviour
{
    [SerializeField] List<StateManagerAICAC> aiCACScriptsList= new List<StateManagerAICAC>();
    [SerializeField] List<StateManagerAICAC> aiCACSurroundSelectedList = new List<StateManagerAICAC>();
    [SerializeField] List<SurroundParameterAICAC> listSurroundParameterAICACSO = new List<SurroundParameterAICAC>();

    [SerializeField] int maxAISurround;
    [SerializeField] int numberAISurrouned;
    int index = 0;

    [SerializeField] float currentCoolDownDurround;
    [SerializeField] float maxCoolDownDurround;

    void Start()
    {
        for (int i =0; i < transform.childCount; i++)
        {
            aiCACScriptsList.Add(transform.GetChild(i).GetComponent<StateManagerAICAC>());
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            for(int j = 0; j < transform.childCount; j++)
            {
                aiCACScriptsList[j].listOtherCACAI.Add(transform.GetChild(i).gameObject);
            }
        }

        maxAISurround = Mathf.CeilToInt((float)aiCACScriptsList.Count / 2f);

        currentCoolDownDurround = maxCoolDownDurround;

        for (int i = 0; i < aiCACScriptsList.Count; i++)
        {
            listSurroundParameterAICACSO.Add(aiCACScriptsList[i].surroundParameterAICACSOInstance);

        }
    }

    // Update is called once per frame
    void Update()
    {
        CoolDown();

        if(aiCACSurroundSelectedList.Count == 2)
        {
            if (aiCACSurroundSelectedList[0].surroundParameterAICACSOInstance.right && aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.right)
            {
                aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.right = false;
                aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.left = true;
            }
            else if(aiCACSurroundSelectedList[0].surroundParameterAICACSOInstance.left && aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.left)
            {
                aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.left = false;
                aiCACSurroundSelectedList[1].surroundParameterAICACSOInstance.right = true;
            }
        }
    }

    void CoolDown()
    {
        if(currentCoolDownDurround >0)
        {
            currentCoolDownDurround -= Time.deltaTime;
        }
        else
        {
            currentCoolDownDurround = maxCoolDownDurround;
            SurroundPlayer();
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

        /*while (aiCACSurroundSelectedList.Count > 0)
        {
            for (int i = 0; i < aiCACSurroundSelectedList.Count; i++)
            {
                aiCACSurroundSelectedList.RemoveAt(i);
            }
        }*/
    }
}