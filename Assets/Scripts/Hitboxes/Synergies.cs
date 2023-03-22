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

    public void Synergize(SynergyProjectile bullet, Transform collider)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 2f, collider.gameObject);

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
                        FugueOnCimetiere();
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

    public void MuseOnAragon()
    {
        Debug.Log("projectile transformé en projo acide");
    }

    public void EylauOnAragon()
    {
        Debug.Log("Projectile pas chargé devient chargé");
    }

    public void FugueOnMalade()
    {
        Debug.Log("Balle rebondit entre ennemis marqués");
    }

    public void EylauOnMalade()
    {
        Debug.Log("Zzt d'intensité proportionnel entre tous les ennemis marqués");
    }

    public void FugueOnCimetiere()
    {
        Debug.Log("Trou noir");
    }

    public void MuseOnCimetiere(float y)
    {
        Debug.Log("Plein de petites explosions (== une grosse) qui stun");
        Vector3 initialPos = new Vector3(_eylauArea.position.x, y, _eylauArea.position.z);
        for (int i = 1; i < 20; i++)
        {
            SpawnAnExplosion(i, initialPos, Vector3.up);
            SpawnAnExplosion(i, initialPos, Vector3.down);
        }
    }

    private void SpawnAnExplosion(int i, Vector3 initialPos, Vector3 direction)
    {
        MuseEylauExplosions exp = _explosionVfx.Get().GetComponent<MuseEylauExplosions>();
        Vector3 pos = initialPos + direction * i + Random.insideUnitSphere * 6;
        exp.Setup(pos, i / 40.0f);
    }
}
