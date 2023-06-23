using System.Collections;
using System.Collections.Generic;
using CameraShake;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class Synergies : LocalManager<Synergies>
{
    public enum Chants
    {
        NONE,
        ARAGON,
        MUSE,
        EYLAU
    }

    [Foldout("Nuage d'Aragon")]
    public List<AragonCloud> ActiveClouds;

    [Foldout("Malades")]
    public List<EnemyHealth> Hospital;

    [Foldout("Cimetière")]
    [SerializeField] EylauArea _eylauArea;

    #region Synergize with projectile
    public void Synergize(SynergyProjectile bullet, Transform collider)
    {
        // PlayerManager.Instance.StartSlowMo(0.0f, 0.1f);
        // SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/MuseMalade/Impact", 1f, gameObject);

        Chants bulletChant = bullet.Chant;
        Chants colliderChant = collider.GetComponent<SynergyTrigger>().Chant;

        switch (bulletChant)
        {
            case Chants.ARAGON:
                switch (colliderChant)
                {
                    case Chants.ARAGON:
                        //Nothing!
                        break;
                    case Chants.MUSE:
                        // FugueOnMalade(bullet.transform.position);
                        //Nothing, since the synergy is about the bullets directly tracking the enemies;
                        break;
                    case Chants.EYLAU:
                        if (!_eylauArea.IsActive)
                            break;
                        Vector3 position = bullet.transform.position;
                        position.y += bullet.Direction.normalized.y * 8;
                        FugueOnCimetiere(position);
                        break;
                }
                break;

            case Chants.MUSE:
                switch (colliderChant)
                {
                    case Chants.ARAGON:
                        MuseOnAragon(bullet, collider.GetComponentInParent<AragonCloud>());
                        break;
                    case Chants.MUSE:
                        //Nothing!
                        break;
                    case Chants.EYLAU:
                        if (!_eylauArea.IsActive)
                            break;
                        MuseOnCimetiere(bullet.transform.position);
                        bullet.Explode(bullet.transform.forward);
                        break;
                }
                break;

            case Chants.EYLAU:
                switch (colliderChant)
                {
                    case Chants.ARAGON:
                        EylauOnAragon(bullet);
                        break;
                    case Chants.MUSE:
                        EylauOnMalade(bullet.transform.position, bullet.Damage);
                        break;
                    case Chants.EYLAU:
                        //Nothing!
                        break;
                }
                break;
        }
    }
    #endregion

    #region Synergize with hitbox
    public void Synergize(SynergyHitbox hitbox, Transform collider)
    {
        Chants bulletChant = hitbox.Chant;
        Chants colliderChant = collider.GetComponent<SynergyTrigger>().Chant;

        switch (bulletChant)
        {
            case Chants.MUSE:
                switch (colliderChant)
                {
                    case Chants.EYLAU:
                        if (!_eylauArea.IsActive)
                            break;
                        MuseOnCimetiere(hitbox.transform.position);
                        break;
                }
                break;
        }
    }

    public void Synergize(SynergyExplosion hitbox, Transform collider)
    {
        Chants bulletChant = hitbox.Chant;
        Chants colliderChant = collider.GetComponent<SynergyTrigger>().Chant;

        switch (bulletChant)
        {
            case Chants.MUSE:
                switch (colliderChant)
                {
                    case Chants.EYLAU:
                        if (!_eylauArea.IsActive)
                            break;
                        MuseOnCimetiere(hitbox.transform.position);
                        break;
                }
                break;
        }
    }
    #endregion

    #region Muse -> Nuage

    [Foldout("Muse -> Nuage d'Aragon")]
    [SerializeField] Pooler _acidicProjectilePooler;
    [Foldout("Muse -> Nuage d'Aragon")]
    [SerializeField] float _delayBetweenClouds;


    public void MuseOnAragon(SynergyProjectile bullet, AragonCloud hitCloud)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, gameObject);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/MuseOnAragon/MuseOnAragonHit", 1f, gameObject);
        PlayerManager.Instance.StartFlash(0.1f, 1);
        int hitCloudNb = ActiveClouds.IndexOf(hitCloud);

        foreach (AragonCloud cloud in ActiveClouds)
        {
            cloud.Poisonify((Mathf.Abs(hitCloudNb - ActiveClouds.IndexOf(cloud))) * _delayBetweenClouds);
            Debug.Log(hitCloudNb);
        }
        ActiveClouds.Clear();
        bullet.Explode(bullet.Direction);
    }
    #endregion

    #region Eylau -> Nuage
    [Foldout("Eylau -> Nuage d'Aragon")]
    [SerializeField] private float _radius = 7.0f;
    [Foldout("Eylau -> Nuage d'Aragon")]
    [SerializeField] private LayerMask _enemiesMask;
    [Foldout("Eylau -> Nuage d'Aragon")]
    [SerializeField] private List<Health> _enemies;
    [Foldout("Eylau -> Nuage d'Aragon")]
    [SerializeField] private float _knockbackStrength = 10.0f;
    [Foldout("Eylau -> Nuage d'Aragon")]
    [SerializeField] AnimationCurve _curve;
    [Foldout("Eylau -> Nuage d'Aragon")]
    [SerializeField] PerlinShake.Params _wooshShake;

    public void EylauOnAragon(SynergyProjectile bullet)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, gameObject);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/EyleauOnAragon/Sound", 1f, ActiveClouds[(int)ActiveClouds.Count / 2].gameObject);
        PlayerManager.Instance.StartFlash(0.1f, 0.5f);

        Vector3 start = ActiveClouds[0].transform.position;
        Vector3 end = ActiveClouds[ActiveClouds.Count - 1].transform.position;
        Player.Instance.StartPerlinShake(_wooshShake, start + ((end - start) / 2));
        bullet.ForceSynergized();
        IEnumerator coroutine = WooshEm(start, end);
        StartCoroutine(coroutine);

        foreach (AragonCloud cloud in ActiveClouds)
            cloud.Swoosh();
        ActiveClouds.Clear();
        _enemies.Clear();
    }

    private IEnumerator WooshEm(Vector3 start, Vector3 end)
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Collider collider in Physics.OverlapCapsule(start, end, _radius, _enemiesMask))
        {
            Health health = collider.GetComponent<Hurtbox>().HealthComponent;
            if (!_enemies.Contains(health))
            {
                _enemies.Add(health);
                EnemyHealth enemy = health as EnemyHealth;

                Vector3 enemyPos = Player.Instance.transform.position;
                Vector3 center = end - ((end - start) / 2);
                Vector3 dir = (end - start).normalized;
                float distFromCenter = (enemyPos - center).magnitude;
                float maxDist = (start - center).magnitude + 2;
                float dot = Vector3.Dot(dir, (center - enemyPos).normalized) * _curve.Evaluate(Mathf.InverseLerp(0, maxDist, distFromCenter));

                Vector3 toEnemy = (enemyPos - center);
                toEnemy = new Vector3(toEnemy.x, 0, toEnemy.z).normalized;
                Vector3 finalDir = (toEnemy + (dir.normalized * dot));
                finalDir = finalDir.normalized;
                enemy.KnockbackSynergie(finalDir * _knockbackStrength);
                enemy.TakeDamage(1);
            }
        }
        yield return null;
    }

    void Update()
    {

    }
    #endregion

    #region Fugue -> Malade
    [Foldout("Fugue -> Malades")]
    public Pooler FugueMaladeShotsPooler;

    public void FugueOnMalade(Vector3 position)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, gameObject);
        PlayerManager.Instance.StartFlash(0.1f, 0.5f);

        // FugueMaladeShot shot = _fugueMaladeShotsPooler.Get() as FugueMaladeShot;
        // shot.Setup(Hospital, position);
        // foreach (EnemyHealth enemy in Hospital)
        //     enemy.RecoverFromSickness();
    }
    #endregion

    #region Eylau -> Malade
    [Foldout("Eylau -> Malades")]
    [SerializeField]
    Pooler _eylauMaladeVfxPooler;
    [Foldout("Eylau -> Malades")]
    [SerializeField] float _zapDamage = 2;
    [Foldout("Eylau -> Malades")]
    [SerializeField] PerlinShake.Params _zapShake;

    public void EylauOnMalade(Vector3 position, float damage)
    {
        PlayerManager.Instance.StartFlash(0.1f, 0.5f);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, gameObject);
        Player.Instance.StartPerlinShake(_zapShake, position);
        foreach (EnemyHealth enemy in Hospital)
        {
            VisualEffect arc = _eylauMaladeVfxPooler.Get().GetComponent<VisualEffect>();
            arc.transform.position = Vector3.zero;
            arc.SetVector3("Start_Pos", position);
            arc.SetVector3("End_Pos", enemy.transform.position);
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/EyleauOnMuse/Sound", 1f, gameObject);
            enemy.ZapSlow();
            enemy.TakeDamage(damage * _zapDamage);
            // enemy.RecoverFromSickness();
        }
    }
    #endregion

    #region Fugue -> Cimetière
    [Foldout("Fugue -> Cimetière")]
    [SerializeField] Pooler _blackHolePooler;

    public void FugueOnCimetiere(Vector3 position)
    {
        PlayerManager.Instance.StartFlash(0.1f, 1);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, gameObject);
        _eylauArea.StartBlackHoleDisappearance();
        Vector3 initialPos = new Vector3(_eylauArea.transform.position.x, _eylauArea.transform.position.y, _eylauArea.transform.position.z);
        SpawnBlackHole(initialPos);
    }

    private void SpawnBlackHole(Vector3 position)
    {
        BlackHole blackHole = _blackHolePooler.Get() as BlackHole;
        if (blackHole)
        {
            blackHole.transform.position = position;
            blackHole.Setup();
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/AragonOnEyleau/Sound", 1f, gameObject);
        }
    }
    #endregion

    #region Muse -> Cimetière
    [Foldout("Muse -> Cimetière")]
    [SerializeField] Pooler _explosionVfxPooler;
    [Foldout("Muse -> Cimetière")]
    [SerializeField] private float _explosionOffset = .02f;
    [Foldout("Muse -> Cimetière")]
    [SerializeField] PerlinShake.Params _boomShake;

    public void MuseOnCimetiere(Vector3 initialPos)
    {
        PlayerManager.Instance.StartFlash(0.1f, 1);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, gameObject);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/MuseMalade/Impact", 1, _eylauArea.gameObject);

        _eylauArea.GetComponent<EylauArea>().StartExplosionDisappearance();
        Vector3 endPos = initialPos + (_eylauArea.transform.position - initialPos) * 2;
        GameObject explosionPoint = new GameObject();
        explosionPoint.transform.position = initialPos;
        explosionPoint.transform.forward = (endPos - initialPos).normalized;
        Player.Instance.StartPerlinShake(_boomShake, _eylauArea.transform.position);

        int loops = 14;
        for (int i = 1; i < loops; i++)
        {
            float lerp = (float)i / (loops - 1);
            explosionPoint.transform.Rotate(Vector3.forward * 45);

            explosionPoint.transform.position = Vector3.Lerp(initialPos, endPos, lerp);
            float radius = 8 * Mathf.Abs(Mathf.Sin(Mathf.PI * lerp));
            SpawnAnExplosion(explosionPoint.transform.position + explosionPoint.transform.up * radius, lerp);
            SpawnAnExplosion(explosionPoint.transform.position + explosionPoint.transform.right * radius, lerp);
            SpawnAnExplosion(explosionPoint.transform.position - explosionPoint.transform.up * radius, lerp);
            SpawnAnExplosion(explosionPoint.transform.position - explosionPoint.transform.right * radius, lerp);
            SpawnAnExplosion(explosionPoint.transform.position, lerp);
        }
    }

    private void SpawnAnExplosion(Vector3 pos, float i)
    {
        MuseEylauExplosions exp = _explosionVfxPooler.Get().GetComponent<MuseEylauExplosions>();
        exp.Setup(pos + Vector3.down, i * _explosionOffset);
    }
    #endregion
}
