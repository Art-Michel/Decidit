using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAICAC
{
    public StateManagerAICAC stateManagerAICAC;
    public DeathParameterAICAC deathParameterAICACSO;

    bool once;

    public void Death()
    {
        stateManagerAICAC.transform.parent = null;
        stateManagerAICAC.agent.speed = deathParameterAICACSO.stopSpeed;
        if (!once)
        {
            Debug.Log("ezfedfegefe");
            stateManagerAICAC.aICACVarianteState.SetListActiveAI();
            once = true;
        }
    }
}