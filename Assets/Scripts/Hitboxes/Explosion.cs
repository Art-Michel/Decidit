using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Hitbox
{
    [SerializeField] private float _lifeSpan = 1f;
    private Projectile _parentProjectile;
    private float _lifeT;

    protected override void Awake()
    {
        base.Awake();
        _parentProjectile = transform.parent.GetComponent<Projectile>();
    }

    void Start()
    {
        Reset();
    }

    void OnEnable()
    {
        Reset();
    }

    private void Reset()
    {
        _lifeT = _lifeSpan;
        ClearBlacklist();
    }

    protected override void Update()
    {
        base.Update();
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
        {
            gameObject.SetActive(false);
            _parentProjectile.Disappear();
        }
    }
}
