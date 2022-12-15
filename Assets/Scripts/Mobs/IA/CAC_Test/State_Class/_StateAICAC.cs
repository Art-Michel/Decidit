using UnityEngine;

namespace State.AICAC
{
    public class _StateAICAC : MonoBehaviour
    {
        //Which state is this?
        public StateControllerAICAC.AIState state { get; protected set; }


        protected StateControllerAICAC stateControllerAICAC;


        //Dependency injection of the MenuController to make it easier to reference it from each state
        public virtual void InitState(StateControllerAICAC stateControllerAICAC)
        {
            this.stateControllerAICAC = stateControllerAICAC;
        }
    }
}
