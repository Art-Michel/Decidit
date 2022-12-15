using UnityEngine;

namespace State.FlyAI
{
    public class _StateFlyAI : MonoBehaviour
    {
        //Which state is this?
        public StateControllerFlyAI.AIState state { get; protected set; }


        protected StateControllerFlyAI stateControllerFlyAI;


        //Dependency injection of the MenuController to make it easier to reference it from each state
        public virtual void InitState(StateControllerFlyAI stateControllerFlyAI)
        {
            this.stateControllerFlyAI = stateControllerFlyAI;
        }
    }
}
