using System.Collections.Generic;
using UnityEngine;

namespace State.FlyAI
{
    public class StateControllerFlyAI : MonoBehaviour
    {
        public _StateFlyAI[] allStates;
        EnemyHealth enemyHealth;

        [SerializeField] float timeToEnable;

        bool once;

        public enum AIState
        {
            BaseMove, LockPlayer, BaseAttack, KnockBack, Death
        }

        private Dictionary<AIState, _StateFlyAI> stateDictionary = new Dictionary<AIState, _StateFlyAI>();

        private _StateFlyAI activeState;

        private Stack<AIState> stateHistory = new Stack<AIState>();

        public static AIState currentState;

        private void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }


        void Start()
        {
            //Put all state into a dictionary
            foreach (_StateFlyAI state in allStates)
            {
                if (state == null)
                {
                    continue;
                }

                //Inject a reference to this script into all states
                state.InitState(stateControllerFlyAI: this);

                //Check if this key already exists, because it means we have forgotten to give a state its unique key
                if (stateDictionary.ContainsKey(state.state))
                {
                    Debug.LogWarning($"The key <b>{state.state}</b> already exists in the menu dictionary!");

                    continue;
                }

                stateDictionary.Add(state.state, state);
            }

            //Deactivate all states
            foreach (AIState state in stateDictionary.Keys)
            {
                stateDictionary[state].gameObject.SetActive(false);
            }
            enemyHealth.IsInvulnerable = true;
        }

        private void OnEnable()
        {
            //Activate the default state
            if(once)
            {
                Invoke("LaunchFirstState", timeToEnable);
                Invoke("CanBeHit", 1f);
                Debug.Log("LaunchFirstState");
            }

            once = true;
        }
        void LaunchFirstState()
        {
            //Activate the default state
            Debug.Log("ActiveEnnemy");
            SetActiveState(AIState.BaseMove);
            enemyHealth.IsInvulnerable = false;
        }
        void CanBeHit()
        {
            enemyHealth.IsInvulnerable = false;
        }

        //Activate a state
        public void SetActiveState(AIState newState, bool isJumpingBack = false)
        {
            //First check if this state exists
            if (!stateDictionary.ContainsKey(newState))
            {
                Debug.LogWarning($"The key <b>{newState}</b> doesn't exist so you can't activate the menu!");

                return;
            }

            //Deactivate the old state
            if (activeState != null)
            {
                activeState.gameObject.SetActive(false);
            }

            //Activate the new state
            activeState = stateDictionary[newState];

            activeState.gameObject.SetActive(true);

            currentState = newState;

            //If we are jumping back we shouldn't add to history because then we will get doubles
            if (!isJumpingBack)
            {
                stateHistory.Push(newState);
            }
        }

        void DisableState()
        {
            //Deactivate the old state
            if (activeState != null)
            {
                activeState.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            DisableState();
            enemyHealth.IsInvulnerable = true;
        }
    }
}