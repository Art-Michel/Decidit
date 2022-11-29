using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAICAC
{
    public StateManagerAICAC stateManagerAICAC;
    public DeathParameterAICAC deathParameterAICACSO;

    public void Death()
    {
        stateManagerAICAC.agent.speed = deathParameterAICACSO.stopSpeed;
    }
}