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
    [SerializeField] Vector2 zoomMinMax;
    float currenZoomRation;

    void Awake()
    {
        instance = this;
        cineCam = GetComponent<CinemachineVirtualCamera>();
        cineCamPOV = cineCam.GetCinemachineComponent<CinemachinePOV>();
        baseRotCam = new Vector2(cineCamPOV.m_HorizontalAxis.Value, cineCamPOV.m_VerticalAxis.Value);
        currenZoomRation = zoomMinMax.y;
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
            if (cineCam.m_Lens.FieldOfView > zoomMinMax.x)
                currenZoomRation -= Time.deltaTime * speedZoom;
            else
                currenZoomRation = zoomMinMax.x;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cineCam.m_Lens.FieldOfView < zoomMinMax.y)
                currenZoomRation += Time.deltaTime * speedZoom;
            else
                currenZoomRation = zoomMinMax.y;
        }
        currenZoomRation = Mathf.Clamp(currenZoomRation, zoomMinMax.x, zoomMinMax.y);
        cineCam.m_Lens.FieldOfView = currenZoomRation;
    }

    public void ResetCam()
    {
        Debug.Log("ResetCam");
        cineCamPOV.m_HorizontalAxis.Value = baseRotCam.x;
        cineCamPOV.m_VerticalAxis.Value = baseRotCam.y;
        currenZoomRation = zoomMinMax.y;
    }       
}