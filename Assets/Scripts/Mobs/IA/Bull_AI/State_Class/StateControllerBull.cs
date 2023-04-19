using System.Collections.Generic;
using UnityEngine;

namespace State.AIBull
{
    public class StateControllerBull : MonoBehaviour
    {
        public _StateBull[] allStates;

        EnemyHealth enemyHealth;

        [SerializeField] float timeToEnable;
        bool once;
        public enum AIState
        {
            Idle, BaseMove, BaseAttack, Rush, KnockBack, Attraction, Death
        }
        public AIState currentState;

        private Dictionary<AIState, _StateBull> stateDictionary = new Dictionary<AIState, _StateBull>();

        private _StateBull activeState;

        private Stack<AIState> stateHistory = new Stack<AIState>();

        private void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }


        void Start()
        {
            //Put all state into a dictionary
            foreach (_StateBull state in allStates)
            {
                if (state == null)
                {
                    continue;
                }

                //Inject a reference to this script into all states
                state.InitState(stateController: this);

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
            if(once || !once)
            {
                Invoke("LaunchFirstState", timeToEnable);
                Invoke("CanBeHit", 1f);
            }
            once = true;
        }

        void LaunchFirstState()
        {
            //Activate the default state
            SetActiveState(AIState.Idle);
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
            EnableState(newState, isJumpingBack);
        }
        void EnableState(AIState newState, bool isJumpingBack = false)
        {
            //Deactivate the old state
            if (activeState != null && currentState != newState)
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