using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILife : MonoBehaviour
{
    public int hpMax;
    public int hp;

    [SerializeField] Material material;
    [SerializeField] Color baseColor, hitColor;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;

        hp = hpMax;
        baseColor = material.color;
    }

    public void ApplyDamage(int damage)
    {
        hp -= damage;
        StartCoroutine("HitFeedBack");
    }

    IEnumerator HitFeedBack()
    {
        material.color = hitColor;
        yield return new WaitForSeconds(0.5f);
        material.color = baseColor;
        yield break;
    }
}