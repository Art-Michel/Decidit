using UnityEngine;

namespace State.WallAI
{
    public class _StateWallAI : MonoBehaviour
    {
        //Which state is this?
        public StateControllerWallAI.WallAIState state { get; protected set; }


        protected StateControllerWallAI stateController;


        //Dependency injection of the MenuController to make it easier to reference it from each state
        public virtual void InitState(StateControllerWallAI stateController)
        {
            this.stateController = stateController;
        }
    }
}