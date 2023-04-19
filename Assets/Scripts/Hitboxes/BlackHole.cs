using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State.AIBull;
using State.AICAC;
using State.FlyAI;
using State.WallAI;

public class BlackHole : PooledObject
{
    [SerializeField] private float _activeTime = 2.0f;
    // [SerializeField] private float _lifeSpan = 1.0f;
    [SerializeField] private float _radius = 12.0f;
    [SerializeField] private float _force = 10.0f;
    [SerializeField] private ParticleSystem[] _vfxs;
    [SerializeField] private LayerMask _layerMask;

    private List<GlobalRefAICAC> _vorasesInBH;
    private List<GlobalRefBullAI> _shrednossesInBH;
    private List<GlobalRefFlyAI> _vorisesInBH;
    private List<GlobalRefWallAI> _menasesInBH;

    private float _activeT;
    private bool _isActive;

    public void Setup()
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/AragonOnEyleau/Sound", 1f, gameObject);
        foreach (ParticleSystem vfx in _vfxs)
        {
            vfx.Play();
        }

        _isActive = true;
        _activeT = _activeTime;

        CheckCollisions();
    }

    private void Awake()
    {
        _vorasesInBH = new List<GlobalRefAICAC>();
        _shrednossesInBH = new List<GlobalRefBullAI>();
        _vorisesInBH = new List<GlobalRefFlyAI>();
        _menasesInBH = new List<GlobalRefWallAI>();
    }

    void Update()
    {
        if (_isActive)
        {
            _activeT -= Time.deltaTime;
            if (_activeT <= 0.0f)
                Disable();
        }
        else
        {
            bool b = true;
            foreach (ParticleSystem vfx in _vfxs)
            {
                b = b && vfx.particleCount <= 0;
            }
            if (b)
                Pooler.Return(this);
        }
    }

    private void CheckCollisions()
    {
        Collider[] Enemies = Physics.OverlapSphere(transform.position, _radius, _layerMask);
        foreach (Collider other in Enemies)
        {

            if (other.CompareTag("Ennemi"))
            {
                Debug.Log(other.transform.name + " entered");
                SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/CimetièreEyleau/IAInOut", 1f, gameObject);
                if (other.TryGetComponent<GlobalRefAICAC>(out GlobalRefAICAC voras))
                {
                    if (!_vorasesInBH.Contains(voras))
                    {
                        voras.isInSynergyAttraction = true;
                        _vorasesInBH.Add(voras);
                    }
                }
                else if (other.TryGetComponent<GlobalRefBullAI>(out GlobalRefBullAI shrednoss))
                {
                    if (!_shrednossesInBH.Contains(shrednoss))
                    {
                        shrednoss.isInSynergyAttraction = true;
                        _shrednossesInBH.Add(shrednoss);
                    }
                }
                else if (other.TryGetComponent<GlobalRefFlyAI>(out GlobalRefFlyAI voris))
                {
                    if (!_vorisesInBH.Contains(voris))
                    {
                        voris.isInSynergyAttraction = true;
                        _vorisesInBH.Add(voris);
                    }
                }
                // else if (other.TryGetComponent<GlobalRefWallAI>(out GlobalRefWallAI wallAi))
                // {
                // if (!_aiWallInArea.Contains(cacAi))
                // {
                //     wallAi.isInSynergyAttraction = true;
                //     _aiWallInArea.Add(wallAi);
                // }
                // }

                else
                    Debug.Log("exited enemy but couldnt find ai script");
            }
        }
    }

    private void Disable()
    {
        _isActive = false;
        foreach (ParticleSystem vfx in _vfxs)
            vfx.Stop();

        foreach (GlobalRefAICAC voras in _vorasesInBH)
            voras.isInSynergyAttraction = false;
        foreach (GlobalRefBullAI shrednoss in _shrednossesInBH)
            shrednoss.isInSynergyAttraction = false;
        foreach (GlobalRefFlyAI voris in _vorisesInBH)
            voris.isInSynergyAttraction = false;
        // foreach (GlobalRefWallAI aiWall in _menasesInBH)
        // aiWall.isInSynergyAttraction = false;

        _vorasesInBH.Clear();
        _shrednossesInBH.Clear();
        _vorisesInBH.Clear();
        _menasesInBH.Clear();
    }

}
