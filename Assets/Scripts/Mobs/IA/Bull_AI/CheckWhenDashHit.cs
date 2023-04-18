using UnityEngine;

namespace State.AIBull
{
    public class CheckWhenDashHit : Hitbox
    {
        [SerializeField] BaseRushStateBullAI rushState;
        protected override void Hit(Transform targetCollider)
        {
            base.Hit(targetCollider);
            rushState.StopRush();
        }
    }
}