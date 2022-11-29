using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBull
{
    public BullAI bullAI;
    public DeathBullParameterSO deathBullParameterSO;

    public void Death()
    {
        bullAI.agent.speed = deathBullParameterSO.stopSpeed;
    }
}
