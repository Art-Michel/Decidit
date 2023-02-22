using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundManager : LocalManager<SoundManager>
{
    [SerializeField] StudioListener AudioListener;

    protected void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySound(string PathLink)
    {
        FMODUnity.RuntimeManager.PlayOneShot(PathLink);
    }
}
