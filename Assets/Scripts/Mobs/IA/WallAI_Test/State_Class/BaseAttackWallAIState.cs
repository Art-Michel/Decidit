using System.Collections;
using UnityEngine;

namespace State.WallAI
{
    public class BaseAttackWallAIState : _StateWallAI
    {
        protected StateControllerWallAI stateControllerWallAI;
        [SerializeField] bool activeAttack;

        BaseAttackWallAISO baseAttackWallAISO;
        [SerializeField] GlobalRefWallAI globalRef;

        [SerializeField] bool isFiring;

        public override void InitState(StateControllerWallAI stateController)
        {
            base.InitState(stateController);
            stateControllerWallAI = stateController;
            state = StateControllerWallAI.WallAIState.BaseAttack;
        }

        private void Start()
        {
            baseAttackWallAISO = globalRef.baseAttackWallAISO;
        }

        private void Update()
        {
            if (!activeAttack && baseAttackWallAISO.currentRafaleCount > 0)
            {
                LaunchAttack();
            }
            else
            {
                ReturnInWall();
            }

            if (globalRef.enemyHealth._hp <= 0)
            {
                stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.Death, true);
            }

            if(!isFiring)
                CheckIfPlayerIsBack();
            else
            {
                globalRef.myAnimator.speed = 0;
            }
        }

        void CheckIfPlayerIsBack()
        {
            Vector3 playerForward = globalRef.playerTransform.GetChild(0).forward;

            float dot = Vector3.Dot(playerForward, (globalRef.transform.position - globalRef.playerTransform.position).normalized);
            if (dot > 0.65f)
            {
                globalRef.myAnimator.speed = 1;
            }
            else
            {
                globalRef.myAnimator.speed = baseAttackWallAISO.speedSlowAnimAttack;
            }
        }

        void LaunchAttack()
        {
            if(this.enabled)
            {
                activeAttack = true;
                globalRef.agent.speed = baseAttackWallAISO.stopSpeed;
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "LaunchAttack");
            }
        }

        public float CalculateSpeedProjectile()
        {
            // v= d/t;
            // t = d/v;
            // d = v*t;

            Vector3 directionPlayer = Player.Instance.FinalMovement.normalized;

            switch (directionPlayer.magnitude)
            {
                case 0:
                    baseAttackWallAISO.playerPredicDir = globalRef.playerTransform.position;
                    baseAttackWallAISO.vPlayer = directionPlayer.magnitude;
                    baseAttackWallAISO.vProjectileGotToPredicPos = baseAttackWallAISO.defaultForceBullet;
                    break;
                default:
                    baseAttackWallAISO.playerPredicDir = globalRef.playerTransform.position + (directionPlayer * baseAttackWallAISO.distAnticipGround);
                    baseAttackWallAISO.vPlayer = directionPlayer.magnitude * baseAttackWallAISO.vMultiplier;

                    baseAttackWallAISO.timePlayerGoToPredicPos = Vector3.Distance(globalRef.playerTransform.position, baseAttackWallAISO.playerPredicDir) / baseAttackWallAISO.vPlayer;
                    baseAttackWallAISO.vProjectileGotToPredicPos = Vector3.Distance(globalRef.transform.position, baseAttackWallAISO.playerPredicDir) / baseAttackWallAISO.timePlayerGoToPredicPos;
                    break;
            }
            return baseAttackWallAISO.vProjectileGotToPredicPos;
        }

        public void CheckCanFire()
        {
            if (baseAttackWallAISO.currentRafaleCount > 0)
            {
                if (baseAttackWallAISO.bulletCount > 0)
                {
                    StartCoroutine("LaunchProjectile");
                }
                else
                {
                    if(baseAttackWallAISO.bulletCount <=0 && baseAttackWallAISO.currentRafaleCount >0)
                    {
                        ResetBulletCount();
                    }
                }
            }
        }

        IEnumerator LaunchProjectile()
        {
            isFiring = true;
            yield return new WaitForSeconds(0.2f);

            if (baseAttackWallAISO.bulletCount > 0)
            {
                globalRef.spawnBullet.LookAt(baseAttackWallAISO.playerPredicDir);
                Rigidbody cloneBullet = Instantiate(baseAttackWallAISO.bulletPrefab, globalRef.spawnBullet.position, globalRef.spawnBullet.rotation);
                cloneBullet.AddRelativeForce(Vector3.forward * CalculateSpeedProjectile(), ForceMode.VelocityChange);
                baseAttackWallAISO.bulletCount--;
                //SoundManager.instance.PlaySoundMobOneShot(globalRef.audioSourceWallMob, SoundManager.instance.soundAndVolumeWallMob[4]);
                //PLAY SOUND SHOOT WALL AI
                // TO DO lucas va te faire encul�
                SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/Shoot", 1f, gameObject);
                StartCoroutine("LaunchProjectile");
                yield break;
            }
            else
            {
                isFiring = false;

                if (baseAttackWallAISO.bulletCount <= 0 && baseAttackWallAISO.currentRafaleCount > 0)
                {
                    ResetBulletCount();
                }

                if (baseAttackWallAISO.currentRafaleCount>0)
                    baseAttackWallAISO.currentRafaleCount--;

                StopCoroutine("LaunchProjectile");
                yield break;
            }
        }

        void ReturnInWall()
        {
            if(baseAttackWallAISO.currentRafaleCount <=0)
            {
                AnimatorManager.instance.DisableAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "LaunchAttack");
                activeAttack = false;
            }
        }

        void ResetBulletCount()
        {
            baseAttackWallAISO.bulletCount = baseAttackWallAISO.maxBulletCount;
        }
        void ResetRafalCount()
        {
            baseAttackWallAISO.currentRafaleCount = baseAttackWallAISO.maxRafaleCount;
        }

        private void OnDisable()
        {
            ResetBulletCount();
            ResetRafalCount();
            globalRef.myAnimator.speed = 1;
        }

        ////////////////////////  ANIMATION EVENT \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public void StartAttack()
        {
            CalculateSpeedProjectile();
            CheckCanFire();
        }
        public void EndAttack()
        {
            ReturnInWall();
        }
        public void PlayInWallSound()
        {
            //SoundManager.instance.PlaySoundMobOneShot(globalRef.audioSourceWallMob, SoundManager.instance.soundAndVolumeWallMob[3]);
            //PLAY IN WALL AI
            // TO DO lucas va te faire encul�
            SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/ExitEnterWall", 1f, gameObject);
        }
        public void PlayOutWallSound()
        {
            globalRef.meshRenderer.enabled = true;
            SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/ExitEnterWall", 1f, gameObject);
            SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/PreShoot", 1f, gameObject);
        }

        public void ReturnBaseMove()
        {
            globalRef.meshRenderer.enabled = false;
            stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.BaseMove, true);
        }
    }
}