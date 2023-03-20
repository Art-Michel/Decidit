using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Synergies : LocalManager<Synergies>
{
    [Foldout("FugueOnMuse")]
    [SerializeField] float mario;

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
