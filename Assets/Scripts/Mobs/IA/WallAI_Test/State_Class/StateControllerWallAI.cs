using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State.WallAI
{
    public class StateControllerWallAI : MonoBehaviour
    {
        public _StateWallAI[] allStates;

        EnemyHealth enemyHealth;

        public enum WallAIState
        {
            BaseMove, BaseAttack, Death
        }

        private Dictionary<WallAIState, _StateWallAI> stateDictionary = new Dictionary<WallAIState, _StateWallAI>();

        private _StateWallAI activeState;

        private Stack<WallAIState> stateHistory = new Stack<WallAIState>();

        private void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }


        void Start()
        {
            //Put all state into a dictionary
            foreach (_StateWallAI state in allStates)
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
            foreach (WallAIState state in stateDictionary.Keys)
            {
                stateDictionary[state].gameObject.SetActive(false);
            }
            enemyHealth.IsInvulnerable = true;
        }

        private void OnEnable()
        {
            //Activate the default state
            Invoke("LaunchFirstState", 1f);
        }

        void LaunchFirstState()
        {
            SetActiveState(WallAIState.BaseMove);
            enemyHealth.IsInvulnerable = false;
        }

        //Activate a state
        public void SetActiveState(WallAIState newState, bool isJumpingBack = false)
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