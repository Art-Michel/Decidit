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

    [Foldout("FugueOnMuse")]
    [SerializeField] float mario;

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
                        MuseOnCimetiere();
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

    public void MuseOnCimetiere()
    {
        Debug.Log("Plein de petites explosions (== une grosse) qui stun");
    }
}
