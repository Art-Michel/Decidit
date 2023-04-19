using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State.FlyAI
{
    public class AttractionStateFlyAI : _StateFlyAI
    {
        [SerializeField] GlobalRefFlyAI globalRef;
        [SerializeField] Vector3 dirAttraction;
        float deltaTime;
        [SerializeField] float distDetectGround;
        [SerializeField] float distDestination;
        [SerializeField] Transform childflyAI;

        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);

            state = StateControllerFlyAI.AIState.Attraction;
        }

        private void OnEnable()
        {
            globalRef.characterController.enabled = true;
            globalRef.characterController.Move(Vector3.zero);
        }

        void Update()
        {
            deltaTime = Time.deltaTime;
            distDestination = Vector3.Distance(childflyAI.transform.position, globalRef.AttractionSO.pointBlackHole);
            ApplyAttraction();

            if (!globalRef.isInSynergyAttraction)
            {
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
            }
        }

        void ApplyAttraction()
        {
            Vector3 move;
            dirAttraction = globalRef.AttractionSO.pointBlackHole - globalRef.transform.position;
            Vector3 dirYAttraction = globalRef.AttractionSO.pointBlackHole - childflyAI.transform.position;

            dirAttraction = (dirAttraction.normalized * (dirAttraction.magnitude - globalRef.AttractionSO.friction * deltaTime));
            dirYAttraction = (dirYAttraction.normalized * (dirYAttraction.magnitude - globalRef.AttractionSO.friction * deltaTime));

            move = new Vector3(dirAttraction.x, dirAttraction.y, dirAttraction.z);
            Vector3 moveXZ;
            moveXZ.y = 0;
            moveXZ.x = move.x;
            moveXZ.z = move.z;

            Vector3 moveY;
            moveY.y = dirYAttraction.y;
            moveY.x = 0;
            moveY.z = 0;

            //globalRef.characterController.Move(move * globalRef.attractionSO.speed * deltaTime);
            globalRef.agent.velocity = moveXZ * globalRef.AttractionSO.speed * 100* deltaTime;
            globalRef.agent.baseOffset += (moveY.y * globalRef.AttractionSO.speed * deltaTime);
        }

        void ActiveIdleState()
        {
            stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
        }

        private void OnDisable()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 9)
            {
                Debug.Log("Fly IA Attraction Back In Wall");
                ActiveIdleState();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == 9)
            {
                ActiveIdleState();
            }
        }
    }
}