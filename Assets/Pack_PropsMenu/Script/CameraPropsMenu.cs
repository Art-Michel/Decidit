using UnityEngine;
using Cinemachine;

public class CameraPropsMenu : MonoBehaviour
{
    static public CameraPropsMenu instance;
    CinemachineVirtualCamera cineCam;
    CinemachinePOV cineCamPOV;
    Camera cam;
    public Vector2 baseRotCam;

    [Header("Zoom Parameter")]
    [SerializeField] float speedZoom;
    [SerializeField] Vector2 zoomMinMax_Base;
    [SerializeField] Vector2 zoomMinMax_Max;
    [SerializeField] Vector2 zoomMinMax_Current;
    float currenZoomRation;
    public bool zoomMax;

    void Awake()
    {
        instance = this;
        cineCam = GetComponent<CinemachineVirtualCamera>();
        cineCamPOV = cineCam.GetCinemachineComponent<CinemachinePOV>();
        baseRotCam = new Vector2(cineCamPOV.m_HorizontalAxis.Value, cineCamPOV.m_VerticalAxis.Value);
        currenZoomRation = zoomMinMax_Current.y;
    }

    void Update()
    {
        Zoom();
        RotationCam();
    }

    void RotationCam()
    {
        if (Input.GetButton("Fire2"))
        {
            cineCamPOV.m_VerticalAxis.m_MaxSpeed = 100;
            cineCamPOV.m_HorizontalAxis.m_MaxSpeed = 100;
        }
        else
        {
            cineCamPOV.m_VerticalAxis.m_MaxSpeed = 0;
            cineCamPOV.m_HorizontalAxis.m_MaxSpeed = 0;
        }
    }

    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (cineCam.m_Lens.FieldOfView > zoomMinMax_Current.x)
                currenZoomRation -= Time.deltaTime * speedZoom;
            else
                currenZoomRation = zoomMinMax_Current.x;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cineCam.m_Lens.FieldOfView < zoomMinMax_Current.y)
                currenZoomRation += Time.deltaTime * speedZoom;
            else
                currenZoomRation = zoomMinMax_Current.y;
        }
        currenZoomRation = Mathf.Clamp(currenZoomRation, zoomMinMax_Current.x, zoomMinMax_Current.y);
        cineCam.m_Lens.FieldOfView = currenZoomRation;
    }

    public void ResetCam()
    {
        Debug.Log("ResetCam");
        if (zoomMax)
            zoomMinMax_Current = zoomMinMax_Max;
        else
            zoomMinMax_Current = zoomMinMax_Base;

        cineCamPOV.m_HorizontalAxis.Value = baseRotCam.x;
        cineCamPOV.m_VerticalAxis.Value = baseRotCam.y;
        currenZoomRation = zoomMinMax_Current.y;
    }       
}