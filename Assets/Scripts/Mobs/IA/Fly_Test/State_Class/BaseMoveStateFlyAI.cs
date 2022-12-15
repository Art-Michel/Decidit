using UnityEngine;

namespace State.FlyAI
{
    public class BaseMoveStateFlyAI : _StateFlyAI
    {
        [SerializeField] GlobalRefFlyAI globalRef;
        BaseMoveFlySO baseMoveFlySO;

        [SerializeField] GameObject flyAI;
        [SerializeField] Transform childflyAI;
        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);

            state = StateControllerFlyAI.AIState.BaseMove;
        }

        private void Start()
        {
            baseMoveFlySO = globalRef.baseMoveFlySO;
        }

        private void Update()
        {
            if(state == StateControllerFlyAI.AIState.BaseMove)
            {
                SmoothLookAtYAxisPatrol();
            }
        }

        public void SmoothLookAtYAxisPatrol()
        {
            Vector3 relativePos;

            relativePos.x = baseMoveFlySO.destinationFinal.x - flyAI.transform.position.x;
            relativePos.y = baseMoveFlySO.destinationFinal.y - flyAI.transform.position.y;
            relativePos.z = baseMoveFlySO.destinationFinal.z - flyAI.transform.position.z;

            Quaternion rotation = Quaternion.Slerp(childflyAI.localRotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveFlySO.speedRotationAIPatrol);
            childflyAI.localRotation = rotation;

            if (baseMoveFlySO.speedRotationAIPatrol < baseMoveFlySO.maxSpeedRotationAIPatrol)
            {
                baseMoveFlySO.speedRotationAIPatrol += (Time.deltaTime / baseMoveFlySO.smoothRotationPatrol);
                baseMoveFlySO.lerpSpeedYValuePatrol += (Time.deltaTime / baseMoveFlySO.ySpeedSmootherPatrol);
            }

            ApplyFlyingMove();
            DelayBeforeAttack();
        }
        ////////////// Set Destination \\\\\\\\\\\\\\\\\\\\\
        Vector3 SearchNewPos() // défini la position aléatoire choisi dans la fonction "RandomPointInBounds()" si la distance entre le point et l'IA est suffisament grande
        {
            Debug.Log("searchPos");

            if (baseMoveFlySO.distDestinationFinal < 20)
            {
                baseMoveFlySO.destinationFinal = RandomPointInBounds(globalRef.myCollider.bounds);

                baseMoveFlySO.newPosIsSet = false;
                baseMoveFlySO.speedRotationAIPatrol = 0;
                baseMoveFlySO.currentSpeedYPatrol = 0;
                baseMoveFlySO.lerpSpeedYValuePatrol = 0;
                return baseMoveFlySO.newPos = new Vector3(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
            }
            else
            {
                Debug.Log("pos is find");
                baseMoveFlySO.newPosIsSet = true;
                baseMoveFlySO.speedRotationAIPatrol = 0;
                baseMoveFlySO.currentSpeedYPatrol = 0;
                baseMoveFlySO.lerpSpeedYValuePatrol = 0;

                Vector3 destinationFinal2D = new Vector2(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
                Vector3 transformPos2D = new Vector2(flyAI.transform.position.x, flyAI.transform.position.z);

                baseMoveFlySO.timeGoToDestinationPatrol = Vector3.Distance(destinationFinal2D, transformPos2D) / globalRef.agent.speed;
                baseMoveFlySO.maxSpeedYTranslationPatrol = Mathf.Abs(baseMoveFlySO.destinationFinal.y - flyAI.transform.position.y) / baseMoveFlySO.timeGoToDestinationPatrol;

                return baseMoveFlySO.newPos = new Vector3(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
            }
        }
        Vector3 RandomPointInBounds(Bounds bounds) // renvoie une position aléatoire dans un trigger collider
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        void ApplyFlyingMove()
        {
            baseMoveFlySO.distDestinationFinal = Vector3.Distance(flyAI.transform.position, baseMoveFlySO.destinationFinal);
            globalRef.agent.speed = baseMoveFlySO.baseMoveSpeed;

            if (baseMoveFlySO.newPosIsSet)
            {
                if (baseMoveFlySO.distDestinationFinal > 7)
                {
                    globalRef.agent.SetDestination(new Vector3(flyAI.transform.position.x, 0, flyAI.transform.position.z) + childflyAI.TransformDirection(Vector3.forward));
                    baseMoveFlySO.currentSpeedYPatrol = Mathf.Lerp(baseMoveFlySO.currentSpeedYPatrol, baseMoveFlySO.maxSpeedYTranslationPatrol, baseMoveFlySO.lerpSpeedYValuePatrol);

                    if (Mathf.Abs(flyAI.transform.position.y - baseMoveFlySO.destinationFinal.y) > 1)
                    {
                        if (flyAI.transform.position.y < baseMoveFlySO.destinationFinal.y)
                        {
                            globalRef.agent.baseOffset += baseMoveFlySO.currentSpeedYPatrol * Time.deltaTime;
                        }
                        else
                        {
                            globalRef.agent.baseOffset -= baseMoveFlySO.currentSpeedYPatrol * Time.deltaTime;
                        }
                    }
                }
                else
                    baseMoveFlySO.newPosIsSet = false;
            }
            else
                SearchNewPos();
        }

        void DelayBeforeAttack()
        {
            if (baseMoveFlySO.currentRateAttack > 0)
            {
                baseMoveFlySO.currentRateAttack -= Time.deltaTime;
            }
            else
            {
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.LockPlayer);
                //flyAINavMesh.SwitchToNewState(1);
            }
        }

        public void ForceLaunchAttack()
        {
            baseMoveFlySO.currentRateAttack = 0;
        }

        void OnDisable()
        {
            baseMoveFlySO.currentRateAttack = baseMoveFlySO.maxRateAttack;
            baseMoveFlySO.timeGoToDestinationPatrol = 0;
            baseMoveFlySO.maxSpeedYTranslationPatrol = 0;
            baseMoveFlySO.currentSpeedYPatrol = 0;
            baseMoveFlySO.lerpSpeedYValuePatrol = 0;
            baseMoveFlySO.speedRotationAIPatrol = 0;

            baseMoveFlySO.newPosIsSet = false;
        }

    }
}