using UnityEngine;

namespace State.FlyAI
{
    public class FlyMobAttackManager : MonoBehaviour
    {
        [SerializeField] int countAiAttackRange;

        bool SpreadShotActive;

        void SetAIVariantAttackRange(GlobalRefFlyAI globalRef)
        {
            if(!SpreadShotActive)
            {
                globalRef.SpreadShot = true;
                SpreadShotActive = true;
                globalRef.stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseAttack);
            }
            else
            {
                SpreadShotActive = false;
                globalRef.stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseAttack);
            }
        }

        public void DownCount()
        {
            countAiAttackRange--;
        }

        public void CountAIAttack(GlobalRefFlyAI globalRef)
        {
            countAiAttackRange++;
        }

        public void ChooseAttackType(GlobalRefFlyAI globalRef)
        {
            if(countAiAttackRange ==0)
            {
                int i = Random.Range(0, 2);
                if (i == 0)
                    globalRef.stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseRangeAttack);
                else
                    globalRef.stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseAttack);
            }
            else
            {
                SetAIVariantAttackRange(globalRef);
            }
        }
    }
}