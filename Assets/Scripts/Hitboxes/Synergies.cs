using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Synergies : LocalManager<Synergies>
{
    public enum Chants
    {
        ARAGON,
        MUSE,
        EYLAU
    }

    [Foldout("Cimetière")]
    [SerializeField] Transform _eylauArea;
    [Foldout("Muse -> Cimetière")]
    [SerializeField] Pooler _explosionVfx;
    [Foldout("Fugue -> Cimetière")]
    [SerializeField] Pooler _blackHole;

    public void Synergize(SynergyProjectile bullet, Transform collider)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, collider.gameObject);
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
                        FugueOnMalade();
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
                        MuseOnAragon();
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
                        EylauOnAragon();
                        break;
                    case Chants.MUSE:
                        EylauOnMalade();
                        break;
                    case Chants.EYLAU:
                        //Nothing!
                        break;
                }
                break;
        }
    }

    #region Muse -> Nuage
    public void MuseOnAragon()
    {
        Debug.Log("projectile transformé en projo acide");
    }
    #endregion

    #region Eylau -> Nuage
    public void EylauOnAragon()
    {
        Debug.Log("Projectile pas chargé devient chargé");
    }
    #endregion

    #region Fugue -> Malade
    public void FugueOnMalade()
    {
        Debug.Log("Balle rebondit entre ennemis marqués");
    }
    #endregion

    #region Eylau -> Malade
    public void EylauOnMalade()
    {
        Debug.Log("Zzt d'intensité proportionnel entre tous les ennemis marqués");
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
        BlackHole blackHole = _blackHole.Get() as BlackHole;
        if (blackHole)
        {
            blackHole.transform.position = position;
            blackHole.Setup();
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

        MuseEylauExplosions exp = _explosionVfx.Get().GetComponent<MuseEylauExplosions>();
        exp.Setup(pos + Vector3.up * i * 2, i / 30.0f);

        if (i == 1)
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/MuseOnEyleau/Sound", 1f, exp.gameObject);

        MuseEylauExplosions exp2 = _explosionVfx.Get().GetComponent<MuseEylauExplosions>();
        exp2.Setup(pos + Vector3.down * i * 2, i / 30.0f);
    }
    #endregion
}
