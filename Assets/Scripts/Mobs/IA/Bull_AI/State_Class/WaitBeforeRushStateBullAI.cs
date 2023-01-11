using UnityEngine;
using UnityEngine.AI;

namespace State.AIBull
{
    public class WaitBeforeRushStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        RaycastHit hit;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.WaitBeforeRush;
        }

        private void OnEnable()
        {
            globalRef.agent.speed = globalRef.coolDownRushBullSO.speedPatrolToStartPos;
        }

        private void Update()
        {
            GoToStartRushPos();
        }
        private void FixedUpdate()
        {
            CheckObstacle();
        }

        void GoToStartRushPos()
        {
            CoolDownBeforeRush();

            if (globalRef.coolDownRushBullSO.startPos == Vector3.zero) // cherche une nouvelle position si aucune n est defifni
            {
                SelectStartPos();
            }
            else if (globalRef.agent.remainingDistance > 0.5f) // avance vers la position
            {
                globalRef.agent.SetDestination(CheckNavMeshPoint(globalRef.coolDownRushBullSO.startPos));
            }

            if (globalRef.coolDownRushBullSO.startPos != Vector3.zero && globalRef.agent.remainingDistance <= 1f)
            {
                if (globalRef.coolDownRushBullSO.currentDurationStay > 0)
                {
                    globalRef.coolDownRushBullSO.currentDurationStay -= Time.deltaTime;
                }
                else
                {
                    globalRef.coolDownRushBullSO.currentDurationStay = globalRef.coolDownRushBullSO.maxDurationStay;
                    globalRef.bullAIStartPosRush.ResetSelectedBox(globalRef.coolDownRushBullSO.boxSelected);
                    globalRef.coolDownRushBullSO.boxSelected = null;
                    SelectStartPos();
                }
            }
            SmoothLookAtPlayer();
        }

        void SelectStartPos()
        {
            globalRef.bullAIStartPosRush.SelectAI(globalRef);
            globalRef.agent.speed = globalRef.coolDownRushBullSO.speedPatrolToStartPos;
            globalRef.agent.SetDestination(CheckNavMeshPoint(globalRef.coolDownRushBullSO.startPos));
        }
        Vector3 CheckNavMeshPoint(Vector3 _destination)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(_destination, out closestHit, 1, 1))
            {
                _destination = closestHit.position;
            }
            return _destination;
        }

        void CheckObstacle()
        {
            if (globalRef.coolDownRushBullSO.currentCoolDownCheckObstacle <= 0)
            {
                hit = RaycastAIManager.RaycastAI(globalRef.transform.position, globalRef.transform.forward, globalRef.coolDownRushBullSO.mask, Color.red, globalRef.coolDownRushBullSO.distDetect);

                if (hit.transform != null)
                {
                    globalRef.coolDownRushBullSO.currentCoolDownCheckObstacle = globalRef.coolDownRushBullSO.maxCoolDownCheckObstacle;
                    SelectStartPos();
                }
            }
            else
            {
                globalRef.coolDownRushBullSO.currentCoolDownCheckObstacle -= Time.fixedDeltaTime;
            }
        }

        void CoolDownBeforeRush()
        {
            if (globalRef.coolDownRushBullSO.currentCoolDownRush > 0)
            {
                globalRef.coolDownRushBullSO.currentCoolDownRush -= Time.deltaTime;
            }
            else if (globalRef.agent.remainingDistance <= 1f)
            {
                if (globalRef.coolDownRushBullSO.currentDurationStay > 0)
                {
                    ShowSoonAttack();
                    globalRef.coolDownRushBullSO.currentDurationStay -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Rush");
                    stateController.SetActiveState(StateControllerBull.AIState.Rush);
                }
            }
        }
       
        void ShowSoonAttack()
        {
            globalRef.material_Instances.Material.color = globalRef.material_Instances.ColorPreAtatck;
            globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.ColorPreAtatck);
            globalRef.agent.speed = globalRef.coolDownRushBullSO.speedRushToStartPos;
        }

        void SmoothLookAtPlayer()
        {
            Vector3 direction;
            Vector3 relativePos;

            if (globalRef.agent.remainingDistance > 1)
                direction = globalRef.agent.destination;
            else
                direction = globalRef.playerTransform.position;

            //direction = bullAI.playerTransform.position;

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            if (globalRef.coolDownRushBullSO.speedRot < globalRef.coolDownRushBullSO.maxSpeedRot)
                globalRef.coolDownRushBullSO.speedRot += Time.deltaTime / globalRef.coolDownRushBullSO.smoothRot;
            else
            {
                globalRef.coolDownRushBullSO.speedRot = globalRef.coolDownRushBullSO.maxSpeedRot;
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.coolDownRushBullSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        private void OnDisable()
        {
            globalRef.agent.speed = globalRef.coolDownRushBullSO.stopSpeed;
            globalRef.coolDownRushBullSO.rushDestination = globalRef.playerTransform.position + globalRef.transform.forward * globalRef.coolDownRushBullSO.rushInertieSetDistance;
            globalRef.coolDownRushBullSO.currentCoolDownRush = globalRef.coolDownRushBullSO.coolDownRush;
            globalRef.agent.SetDestination(globalRef.coolDownRushBullSO.rushDestination);
            globalRef.coolDownRushBullSO.startPos = Vector3.zero;
            globalRef.bullAIStartPosRush.ResetSelectedBox(globalRef.coolDownRushBullSO.boxSelected);
            globalRef.coolDownRushBullSO.boxSelected = null;
            globalRef.coolDownRushBullSO.speedRot = 0;
            globalRef.material_Instances.Material.color = globalRef.material_Instances.Color;
            globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.Color);
            globalRef.coolDownRushBullSO.currentDurationStay = globalRef.coolDownRushBullSO.maxDurationStay;
        }
    }

    public class CoolDown
    {
        float _maxTime;
        System.Action _CallBack;
        public bool isDone => _maxTime <= 0;


        public CoolDown(float maxTim, System.Action CallBack)
        {
            _maxTime = maxTim;
            _CallBack = CallBack;
        }

        public bool Progress(float elapsedTime)
        {
            _maxTime -= elapsedTime;

            if (_maxTime <= 0)
            {
                _CallBack?.Invoke();
                return true;
            }
            else
                return false;
        }
    }
}