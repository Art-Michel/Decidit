using UnityEngine;
using UnityEngine.AI;

namespace State.WallAI
{
    public class BaseMoveWallAIState : _StateWallAI
    {
        protected StateControllerWallAI stateControllerWallAI;

        public GlobalRefWallAI globalRef;
        BaseMoveWallAISO baseMoveWallAISO;

        RaycastHit hit;

        [SerializeField] bool canTouchPlayer;

        public override void InitState(StateControllerWallAI stateController)
        {
            base.InitState(stateController);
            stateControllerWallAI = stateController;
            state = StateControllerWallAI.WallAIState.BaseMove;
        }

        private void OnEnable()
        {
            try
            {
                globalRef.meshRenderer.enabled = false;
                baseMoveWallAISO = globalRef.baseMoveWallAISO;
            }
            catch
            {
            }
        }

        private void Update()
        {
            MoveAI();

            if (globalRef.enemyHealth._hp <= 0)
            {
                stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.Death, true);
            }

            PlaySound();
        }

        private void FixedUpdate()
        {
            if (!baseMoveWallAISO.findNewPos)
                SelectNewPos();

            if (baseMoveWallAISO.rateAttack <= 0.1f)
            {
                CheckCanTouchPlayer();
            }
        }

        public void MoveAI()
        {
            if (!globalRef.agent.isOnOffMeshLink)
            {
                WallCrackEffect();
            }

            if (!IsMoving())
                baseMoveWallAISO.findNewPos = false;

            SlowSpeed(globalRef.isInEylau);

            /*if (!baseMoveWallAISO.findNewPos)
                globalRef.agent.speed = 0;
            else
                globalRef.agent.speed = baseMoveWallAISO.speedMovement;*/

            LaunchDelayBeforeAttack();
        }
        void SlowSpeed(bool active)
        {
            if (active)
            {
                globalRef.agent.SetDestination(baseMoveWallAISO.newPos);

                globalRef.agent.speed = baseMoveWallAISO.speedMovement;
                globalRef.slowSpeed = globalRef.agent.speed / globalRef.slowRatio;
                globalRef.agent.speed = globalRef.slowSpeed;
            }
            else
            {
                globalRef.agent.SetDestination(baseMoveWallAISO.newPos);
                globalRef.agent.speed = baseMoveWallAISO.speedMovement;
            }
        }
        bool IsMoving()
        {
            if (globalRef.agent.remainingDistance == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        void SelectNewPos()
        {
            if (!IsMoving())
            {
                baseMoveWallAISO.selectedWall = Random.Range(0, 4);
                baseMoveWallAISO.newPos = SearchNewPos(globalRef.wallsList[baseMoveWallAISO.selectedWall].bounds);

                hit = RaycastAIManager.instanceRaycast.RaycastAI(baseMoveWallAISO.newPos, globalRef.playerTransform.position - baseMoveWallAISO.newPos, baseMoveWallAISO.maskCheckTouchPlayer,
                    Color.blue, Vector3.Distance(baseMoveWallAISO.newPos, globalRef.playerTransform.position));

                if (hit.transform != globalRef.playerTransform)
                {
                    baseMoveWallAISO.findNewPos = false;
                }
                else
                {
                    baseMoveWallAISO.findNewPos = true;
                }
            }
        }
        Vector3 SearchNewPos(Bounds bounds)
        {
            return new Vector3(
               Random.Range(bounds.min.x, bounds.max.x),
               Random.Range(bounds.min.y, bounds.max.y),
               Random.Range(bounds.min.z, bounds.max.z)
           );
        }

        void CheckCanTouchPlayer()
        {
            hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.transform.position, globalRef.playerTransform.position - globalRef.transform.position, baseMoveWallAISO.maskCheckTouchPlayer,
                    Color.blue, Vector3.Distance(globalRef.transform.position, globalRef.playerTransform.position));

            if (hit.transform != globalRef.playerTransform)
            {
                canTouchPlayer = false;
                baseMoveWallAISO.findNewPos = false;
            }
            else
            {
                canTouchPlayer = true;
                baseMoveWallAISO.findNewPos = true;
            }
        }

        public void WallCrackEffect()
        {
            baseMoveWallAISO.distSinceLast = Vector3.Distance(globalRef.transform.position, baseMoveWallAISO.lastWallCrack.transform.position);

            if (baseMoveWallAISO.distSinceLast >= baseMoveWallAISO.decalage)
            {
                baseMoveWallAISO.lastWallCrack = Instantiate(baseMoveWallAISO.wallCrackPrefab,
                    globalRef.transform.position,
                    Quaternion.Euler(0, globalRef.orientation, 0));
            }
        }

        void LaunchDelayBeforeAttack()
        {
            if (baseMoveWallAISO.rateAttack > 0)
            {
                baseMoveWallAISO.rateAttack -= Time.deltaTime;
            }
            else
            {
                /*if (!globalRef.agent.isOnOffMeshLink && !IsMoving())
                    stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.BaseAttack);*/

                if (canTouchPlayer)
                    stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.BaseAttack);

            }
        }

        void PlaySound()
        {
            if (IsMoving())
            {
                /*if(!globalRef.audioSourceWallMob.isPlaying)
                    SoundManager.instance.PlaySoundMobByClip(globalRef.audioSourceWallMob, SoundManager.instance.soundAndVolumeWallMob[0], true);*/

                // PLAY SOUND MOVE IN WALL WALLMOB
            }
        }

        // Reset Value When Change State
        private void OnDisable()
        {
            globalRef.baseAttackWallAISO.bulletCount = globalRef.baseAttackWallAISO.maxBulletCount;
            baseMoveWallAISO.rateAttack = baseMoveWallAISO.maxRateAttack;
            baseMoveWallAISO.findNewPos = false;
            globalRef.agent.SetDestination(globalRef.transform.position);
        }
    }
}