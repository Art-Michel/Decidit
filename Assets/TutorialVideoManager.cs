using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideoManager : LocalManager<TutorialVideoManager>
{
    [SerializeField] private VideoClip[] _fugueVideos;
    [SerializeField] private VideoClip[] _museVideos;
    [SerializeField] private VideoClip[] _eylauVideos;
    [SerializeField] private VideoPlayer[] _videoPlayers;

    public void StartClips(Altar.Chants chant)
    {
        VideoClip clip0 = null;
        VideoClip clip1 = null;
        switch (chant)
        {
            case Altar.Chants.Fugue:
                clip0 = _fugueVideos[0];
                clip1 = _fugueVideos[1];
                break;
            case Altar.Chants.Muse:
                clip0 = _museVideos[0];
                clip1 = _museVideos[1];
                break;
            case Altar.Chants.Cimetiere:
                clip0 = _eylauVideos[0];
                clip1 = _eylauVideos[1];
                break;
        }
        _videoPlayers[0].clip = clip0;
        _videoPlayers[0].enabled = true;
        _videoPlayers[1].clip = clip1;
        _videoPlayers[1].enabled = true;
    }

    public void StopClips()
    {
        _videoPlayers[0].enabled = false;
        _videoPlayers[1].enabled = false;
    }
}
