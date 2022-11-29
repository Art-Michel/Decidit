using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWallAI 
{
    public WallAI wallAI;
    public DeathWallAISO deathWallAISO;

    public void Death()
    {
        wallAI.animator.SetBool("IsDead", true);
    }
}
