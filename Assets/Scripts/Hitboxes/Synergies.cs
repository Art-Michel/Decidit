using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class Synergies : LocalManager<Synergies>
{
    public enum Chants
    {
        ARAGON,
        MUSE,
        EYLAU
    }

    #region Declarations
    [Foldout("Nuage d'Aragon")]
    public List<AragonCloud> ActiveClouds;

    [Foldout("Eylau -> Nuage d'Aragon")]
    [SerializeField] Pooler _chargedEylauPooler;
    [Foldout("Muse -> Nuage d'Aragon")]
    [SerializeField] Pooler _acidicProjectilePooler;

    [Foldout("Malades")]
    public List<EnemyHealth> Hospital;
    [Foldout("Fugue -> Malades")]
    [SerializeField]
    Pooler _fugueMaladeShotsPooler;
    [Foldout("Eylau -> Malades")]
    [SerializeField]
    Pooler _eylauMaladeVfxPooler;
    [Foldout("Eylau -> Malades")]
    [SerializeField] float _zapDamage = 2;

    [Foldout("Cimetière")]
    [SerializeField] Transform _eylauArea;
    [Foldout("Muse -> Cimetière")]
    [SerializeField] Pooler _explosionVfxPooler;
    [Foldout("Fugue -> Cimetière")]
    [SerializeField] Pooler _blackHolePooler;
    #endregion

    public void Synergize(SynergyProjectile bullet, Transform collider)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, collider.gameObject);
        PlayerManager.Instance.StartSlowMo(0.0f, 0.1f);
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
                        FugueOnMalade(bullet.transform.position);
                        break;
                    case Chants.EYLAU:
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
                        MuseOnAragon(bullet);
                        break;
                    case Chants.MUSE:
                        //Nothing!
                        break;
                    case Chants.EYLAU:
                        MuseOnCimetiere(bullet.transform.position.y);
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
                        EylauOnMalade(bullet.transform.position);
                        break;
                    case Chants.EYLAU:
                        //Nothing!
                        break;
                }
                break;
        }
    }

    #region Muse -> Nuage
    public void MuseOnAragon(SynergyProjectile bullet)
    {
        Debug.Log("projectile transformé en projo acide");
        foreach (AragonCloud cloud in ActiveClouds)
            cloud.StartDisappearing();
        ActiveClouds.Clear();
    }
    #endregion

    #region Eylau -> Nuage
    public void EylauOnAragon(SynergyProjectile bullet)
    {
        SynergyProjectile shot = _chargedEylauPooler.Get().GetComponent<SynergyProjectile>();
        shot.Setup(bullet.transform.position, bullet.Direction);
        shot.ForceSynergized();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/CimetièreEyleau/MaxCharged", 1f, gameObject);
        foreach (AragonCloud cloud in ActiveClouds)
            cloud.StartDisappearing();
        ActiveClouds.Clear();
    }
    #endregion

    #region Fugue -> Malade
    public void FugueOnMalade(Vector3 position)
    {
        FugueMaladeShot shot = _fugueMaladeShotsPooler.Get() as FugueMaladeShot;
        shot.Setup(Hospital, position);
        foreach (EnemyHealth enemy in Hospital)
            enemy.RecoverFromSickness();
    }
    #endregion

    #region Eylau -> Malade
    public void EylauOnMalade(Vector3 position)
    {
        foreach (EnemyHealth enemy in Hospital)
        {
            enemy.TakeDamage(_zapDamage);
            VisualEffect arc = _eylauMaladeVfxPooler.Get().GetComponent<VisualEffect>();
            arc.transform.position = Vector3.zero;
            arc.SetVector3("Start_Pos", position);
            arc.SetVector3("End_Pos", enemy.transform.position);
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/EyleauOnMuse/Sound", 1f, gameObject);
            enemy.ZapSlow();
            enemy.RecoverFromSickness();
        }
    }
    #endregion

    #region Fugue -> Cimetière
    public void FugueOnCimetiere(Vector3 position)
    {
        Debug.Log("Trou noir, voir avec jt pour attirer les ennemis au centre du cimetière");
        Vector3 initialPos = new Vector3(_eylauArea.position.x, position.y, _eylauArea.position.z);
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
    public void MuseOnCimetiere(float y)
    {
        Vector3 initialPos = new Vector3(_eylauArea.position.x, y, _eylauArea.position.z);
        for (int i = 1; i < 10; i++)
        {
            Vector2 circle = (Random.insideUnitCircle).normalized * Random.Range(2.0f, 6.0f);
            Vector3 position = new Vector3(circle.x, 0.0f, circle.y);

            //twice to fill the cimetiere
            SpawnAnExplosion(initialPos + position, i);
            SpawnAnExplosion(initialPos - position, i);
        }
    }

    private void SpawnAnExplosion(Vector3 pos, int i)
    {

        MuseEylauExplosions exp = _explosionVfxPooler.Get().GetComponent<MuseEylauExplosions>();
        exp.Setup(pos + Vector3.up * i * 2, i / 30.0f);

        if (i == 1)
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/MuseOnEyleau/Sound", 1f, exp.gameObject);

        MuseEylauExplosions exp2 = _explosionVfxPooler.Get().GetComponent<MuseEylauExplosions>();
        exp2.Setup(pos + Vector3.down * i * 2, i / 30.0f);
    }
    #endregion
}
