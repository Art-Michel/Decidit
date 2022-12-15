using UnityEngine;

namespace State.WallAI
{
    public class WallAiAnimEvent : MonoBehaviour
    {
        [SerializeField] BaseAttackWallAIState baseAttackWallAIState;

        ////////////////////////  ANIMATION EVENT \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        void StartAttack()
        {
            baseAttackWallAIState.StartAttack();
        }

        void EndAttack()
        {
            baseAttackWallAIState.EndAttack();
        }
    }
}