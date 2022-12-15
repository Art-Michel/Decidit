using UnityEngine;

namespace State.AIBull
{
    public class _StateBull : MonoBehaviour
    {
        //Which state is this?
        public StateControllerBull.AIState state { get; protected set; }


        protected StateControllerBull stateController;


        //Dependency injection of the MenuController to make it easier to reference it from each state
        public virtual void InitState(StateControllerBull stateController)
        {
            this.stateController = stateController;
        }
    }
}
