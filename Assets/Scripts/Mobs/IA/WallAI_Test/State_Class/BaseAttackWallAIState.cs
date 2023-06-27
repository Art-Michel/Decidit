using System.Collections;
using UnityEngine;
using State.FlyAI;
using System.Collections.Generic;

namespace State.WallAI
{
    public class BaseAttackWallAIState : _StateFlyAI
    {
        [SerializeField] bool activeAttack;

        BaseAttackWallAISO baseAttackWallAISO;
        [SerializeField] GlobalRefFlyAI globalRef;
        [SerializeField] LockPlayerStateFlyAI lockPlayerState;
        [SerializeField] Transform childflyAI;

        [SerializeField] bool isFiring;
        List<Quaternion> bullets;
        float currentAngle;
        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);
            state = StateControllerFlyAI.AIState.BaseRangeAttack;
        }

        private void OnEnable()
        {
           globalRef.flyMobAttackManager.CountAIAttack(globalRef);
        }

        private void Start()
        {
            baseAttackWallAISO = globalRef.baseAttackWallAISO;

            bullets = new List<Quaternion>(baseAttackWallAISO.maxBulletCountSpread);
            for (int i = 0; i < baseAttackWallAISO.maxBulletCountSpread; i++)
            {
                bullets.Add(Quaternion.Euler(Vector3.zero));
            }
        }

        private void Update()
        {
            SmoothLookAtYAxisAttack();

            if (!activeAttack && baseAttackWallAISO.currentRafaleCount > 1 && baseAttackWallAISO.currentShootCount > 0)
            {
                if(baseAttackWallAISO.bulletCount >0 || baseAttackWallAISO.bulletCountSpread >0) LaunchAttack();
            }
            else StopAttack();
        }

        void LaunchAttack()
        {
            if(this.enabled)
            {
                Debug.Log("LaunchAttack");
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

        void CheckCanFire()
        {
            if(!globalRef.SpreadShot)
            {
                if (baseAttackWallAISO.currentRafaleCount > 0)
                {
                    if (baseAttackWallAISO.bulletCount > 0)
                    {
                        StartCoroutine("LaunchProjectileAnticipation");
                    }
                    else
                    {
                        ResetBulletCount();
                        baseAttackWallAISO.currentRafaleCount--;
                        StartCoroutine("LaunchProjectileAnticipation");
                    }
                }
            }
            else
            {
                if (baseAttackWallAISO.currentShootCount > 0)
                {
                    StartCoroutine("LaunchProjectileSpread");
                }
            }
        }

        IEnumerator LaunchProjectileAnticipation()
        {
            isFiring = true;
            yield return new WaitForSeconds(0.2f);

            if (baseAttackWallAISO.bulletCount > 0)
            {
                if(baseAttackWallAISO.currentRafaleCount == 1)
                    AnimatorManager.instance.DisableAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "LaunchAttack");

                globalRef.spawnBullet.LookAt(baseAttackWallAISO.playerPredicDir);
                //Rigidbody cloneBullet = Instantiate(baseAttackWallAISO.bulletPrefab, globalRef.spawnBullet.position, globalRef.spawnBullet.rotation);
                //cloneBullet.AddRelativeForce(Vector3.forward * CalculateSpeedProjectile(), ForceMode.VelocityChange);

                globalRef.poolBullet.CallBullet(globalRef.spawnBullet.position, globalRef.spawnBullet.rotation, 
                                                Vector3.forward * CalculateSpeedProjectile(), ForceMode.VelocityChange);

                baseAttackWallAISO.bulletCount--;
                SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/Shoot", 1f, gameObject);
                StartCoroutine("LaunchProjectileAnticipation");
                //PLAY SOUND SHOOT WALL AI
                // TO DO lucas va te faire encul�
                yield break;
            }
        }

        IEnumerator LaunchProjectileSpread()
        {
            isFiring = true;
            yield return new WaitForSeconds(0.2f);

            float positiveAngle =0;
            float negativeAngle =0;
            globalRef.spawnBullet.LookAt(globalRef.playerTransform.position);
            for (int i = 0; i < baseAttackWallAISO.maxBulletCountSpread; i++)
            {
                //Rigidbody cloneBullet = Instantiate(baseAttackWallAISO.bulletPrefab, globalRef.spawnBullet.position, globalRef.spawnBullet.rotation);
                //cloneBullet.transform.rotation = Quaternion.Euler(cloneBullet.transform.eulerAngles.x, cloneBullet.transform.eulerAngles.y + currentAngle, cloneBullet.transform.eulerAngles.z);
                //cloneBullet.AddForce(cloneBullet.transform.forward * baseAttackWallAISO.forceBulletSpread, ForceMode.VelocityChange);
                globalRef.poolBullet.CallBulletSpread(globalRef.spawnBullet.position, globalRef.spawnBullet.rotation,
                                                baseAttackWallAISO.forceBulletSpread, ForceMode.VelocityChange, bullets[i], baseAttackWallAISO.spreadangle, currentAngle);
                baseAttackWallAISO.bulletCount--;

                if (i % 2 == 0)
                {
                    positiveAngle += baseAttackWallAISO.spreadangle;
                    currentAngle = positiveAngle;
                }
                else
                {
                    negativeAngle -= baseAttackWallAISO.spreadangle;
                    currentAngle = negativeAngle;
                }
            }
            //SoundManager.instance.PlaySoundMobOneShot(globalRef.audioSourceWallMob, SoundManager.instance.soundAndVolumeWallMob[4]);
            //PLAY SOUND SHOOT WALL AI
            // TO DO lucas va te faire encul�
            SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/Shoot", 1f, gameObject);
            StartCoroutine("CoolDownSpreadShot");
            yield break;
        }
        IEnumerator CoolDownSpreadShot()
        {
            currentAngle = 0;
            isFiring = false;
            baseAttackWallAISO.currentShootCount--;
            yield return new WaitForSeconds(baseAttackWallAISO.coolDownSpread);
            if (baseAttackWallAISO.currentShootCount >0)
                StartCoroutine("LaunchProjectileSpread");
            yield break;
        }

        void StopAttack()
        {
            if (baseAttackWallAISO.currentRafaleCount <= 1 || baseAttackWallAISO.currentShootCount <=0)
            {
                AnimatorManager.instance.DisableAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "LaunchAttack");
                activeAttack = false;

                if(baseAttackWallAISO.bulletCount <=0 || baseAttackWallAISO.bulletCountSpread <=0)
                    ReturnBaseMove();
            }
        }

        void ResetBulletCount()
        {
            if(baseAttackWallAISO != null)
                baseAttackWallAISO.bulletCount = baseAttackWallAISO.maxBulletCount;
        }
        void ResetRafalCount()
        {
            if (baseAttackWallAISO != null)
                baseAttackWallAISO.currentRafaleCount = baseAttackWallAISO.maxRafaleCount;

            if (baseAttackWallAISO != null)
                baseAttackWallAISO.currentShootCount = baseAttackWallAISO.maxShootCount;
        }

        void SmoothLookAtYAxisAttack()
        {
            Vector3 relativePos;

            relativePos.x = globalRef.playerTransform.position.x - globalRef.transform.position.x;
            relativePos.y = globalRef.playerTransform.position.y - globalRef.transform.position.y;
            relativePos.z = globalRef.playerTransform.position.z - globalRef.transform.position.z;

            SlowRotation(globalRef.isInEylau, relativePos);

            Quaternion rotation = Quaternion.Slerp(childflyAI.localRotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.baseAttackFlySO.speedRotationAIAttack);
            childflyAI.localRotation = rotation;
        }
        void SlowRotation(bool active, Vector3 relativePos)
        {
            if (active)
            {
                if (globalRef.baseAttackFlySO.speedRotationAIAttack < globalRef.baseAttackFlySO.maxSpeedRotationAILock)
                    globalRef.baseAttackFlySO.speedRotationAIAttack += (Time.deltaTime / (globalRef.baseAttackFlySO.smoothRotationLock * globalRef.slowRatio));
                else
                    globalRef.baseAttackFlySO.speedRotationAIAttack = globalRef.baseAttackFlySO.maxSpeedRotationAILock;
            }
            else
            {
                if (globalRef.baseAttackFlySO.speedRotationAIAttack < globalRef.baseAttackFlySO.maxSpeedRotationAIAttack)
                    globalRef.baseAttackFlySO.speedRotationAIAttack += (Time.deltaTime / globalRef.baseAttackFlySO.smoothRotationAttack);
                else
                    globalRef.baseAttackFlySO.speedRotationAIAttack = globalRef.baseAttackFlySO.maxSpeedRotationAIAttack;
            }
        }

        private void OnDisable()
        {
            AnimatorManager.instance.DisableAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "LaunchAttack");
            ResetBulletCount();
            ResetRafalCount();
            globalRef.myAnimator.speed = 1;
            globalRef.SpreadShot = false;
            globalRef.baseAttackFlySO.speedRotationAIAttack = 0;
            globalRef.baseAttackFlySO.currentSpeedYAttack = 0;
            globalRef.flyMobAttackManager.DownCount();
            activeAttack = false;
            isFiring = false;
        }

        ////////////////////////  ANIMATION EVENT \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public void StartAttack()
        {
            CalculateSpeedProjectile();

            if(this.gameObject.activeInHierarchy)
                CheckCanFire();
        }
        public void EndAttack()
        {
            StopAttack();
        }
        public void PlayInWallSound()
        {
            //SoundManager.instance.PlaySoundMobOneShot(globalRef.audioSourceWallMob, SoundManager.instance.soundAndVolumeWallMob[3]);
            //PLAY IN WALL AI
            // TO DO lucas va te faire encul�
            //SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/ExitEnterWall", 1f, gameObject);
        }
        public void PlayOutWallSound()
        {
           // SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/ExitEnterWall", 1f, gameObject);
            //SoundManager.Instance.PlaySound("event:/SFX_IA/Menas_SFX(Mur)/PreShoot", 1f, gameObject);
        }

        public void ReturnBaseMove()
        {
            stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
        }
    }
}