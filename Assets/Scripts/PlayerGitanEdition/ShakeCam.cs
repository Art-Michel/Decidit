using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeCam : MonoBehaviour
{
    static public ShakeCam instance;

    public static List<ShakeController> shakeCamhitStatic = new List<ShakeController>();
    public List<ShakeController> shakeCamhit = new List<ShakeController>();

    public static List<ShakeController> shakeCamparametersBlockPerfectStatic = new List<ShakeController>();
    public List<ShakeController> shakeCamParametersBlockPerfect = new List<ShakeController>();

    public static List<ShakeController> shakeCamParametersAttackNormalStatic = new List<ShakeController>();
    public List<ShakeController> shakeCamParametersAttackNormal = new List<ShakeController>();

    public static List<ShakeController> shakeCamParametersAttackPerfectStatic = new List<ShakeController>();
    public List<ShakeController> shakeCamParametersAttackPerfect = new List<ShakeController>();

    public static List<ShakeController> shakeCamParametersFailStatic = new List<ShakeController>();
    public List<ShakeController> shakeCamParametersFail = new List<ShakeController>();

    static CinemachineVirtualCamera cinemachineVirtualCamera;

    static CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannel;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannel = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        shakeCamhitStatic = shakeCamhit;

        shakeCamparametersBlockPerfectStatic = shakeCamParametersBlockPerfect;

        shakeCamParametersAttackNormalStatic = shakeCamParametersAttackNormal;

        shakeCamParametersAttackPerfectStatic = shakeCamParametersAttackPerfect;

        shakeCamParametersFailStatic = shakeCamParametersFail;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void ShakerCam(List<ShakeController> refList, Vector3 axeShake, float amplitude, float frequence, float duration)
    {
        cinemachineBasicMultiChannel.m_PivotOffset = axeShake;
        cinemachineBasicMultiChannel.m_AmplitudeGain = amplitude;
        cinemachineBasicMultiChannel.m_FrequencyGain = frequence;

        instance.LaunchCoroutine(refList, duration);
    }

    void LaunchCoroutine(List<ShakeController> refList, float duration)
    {
        StartCoroutine(ShakeCamDuration(refList, duration));
    }

    IEnumerator ShakeCamDuration(List<ShakeController> refList, float duration)
    {
        yield return new WaitForSeconds(duration);
        cinemachineBasicMultiChannel.m_PivotOffset = Vector3.zero;
        cinemachineBasicMultiChannel.m_AmplitudeGain = 0;
        cinemachineBasicMultiChannel.m_FrequencyGain = 0;
        yield break;
    }
}

[System.Serializable]
public class ShakeController
{
    public Vector3 axeShake;

    public float amplitude;
    public float frequence;
    public float duration;
}