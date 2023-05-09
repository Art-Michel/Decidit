using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;

public class PropsMenu : MonoBehaviour
{
    [Header("Objet")]
    [SerializeField] GameObject prefabProps;
    [SerializeField] Transform spawnProps;
    [SerializeField] GameObject cloneProps;

    [Header("UI")]
    [SerializeField] bool isActive;
    [SerializeField] Image fond;
    Text loreText;
    EventSystem m_EventSystem;
    static Transform parentButton;

    CinemachineVirtualCamera cineCam;
    CinemachinePOV cineCamPOV;
    Camera cam;
    Quaternion baseRotCam;

    [Header("Move Objet")]
    [SerializeField] float rotationY, rotSpeedY;

    [SerializeField] List<GameObject> buttonList = new List<GameObject>();

    void Start()
    {
        cineCam = GameObject.Find("CM vcam1").GetComponent< CinemachineVirtualCamera>();
        cineCamPOV = cineCam.GetCinemachineComponent<CinemachinePOV>();

        cam = Camera.main;
        baseRotCam = Quaternion.Euler(cineCam.transform.rotation.eulerAngles.x, cineCam.transform.rotation.eulerAngles.y, cineCam.transform.rotation.eulerAngles.z);

        m_EventSystem = EventSystem.current;
        StartCoroutine("SelectFirstButton");

        loreText = transform.Find("Lore").GetComponent<Text>();
        parentButton = transform.parent;
        fond = transform.Find("Fond").GetComponent<Image>();

        Cursor.lockState = CursorLockMode.Confined;
    }

    IEnumerator SelectFirstButton()
    {
        yield return new WaitForEndOfFrame();
        if (m_EventSystem.currentSelectedGameObject == this.gameObject)
        {
            parentButton.GetChild(0).transform.GetComponent<Button>().Select();
            InstanciateProps();
        }
        yield break;
    }

    void Update()
    {
        RotationObj();
        PrintUI();
        Zoom();
    }

    void RotationObj()
    {
        if (spawnProps.childCount > 0)
        {
            if (spawnProps.GetChild(0).gameObject == cloneProps)
            {
                if (Input.GetButton("Fire1"))
                {
                    rotationY -= rotSpeedY * Time.deltaTime * Input.GetAxis("Mouse X") /*Input.GetAxis("RightJoystickX")*/;
                    cloneProps.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
                }
            }
        }
    }

    void Zoom()
    {
        //cineCam.transform.LookAt(cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y)));

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

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            cineCam.transform.position += cam.transform.TransformDirection(Vector3.forward);
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            cineCam.transform.position += cam.transform.TransformDirection(Vector3.back);
        }
        else
        {
            //cineCam.transform.rotation = baseRotCam;
        }
    }

    void PrintUI()
    {
        //Debug.Log(m_EventSystem.currentSelectedGameObject);

        if (m_EventSystem.currentSelectedGameObject == this.gameObject)
        {
            if(!isActive)
            {
                transform.Find("Unselected").gameObject.SetActive(false);
                transform.Find("Selected").gameObject.SetActive(true);
                loreText.enabled = true;
                fond.enabled = true;
                isActive = true;
            }
        }
        else if(m_EventSystem.currentSelectedGameObject != null)
        {
            if(isActive)
            {
                transform.Find("Unselected").gameObject.SetActive(true);
                transform.Find("Selected").gameObject.SetActive(false);
                loreText.enabled = false;
                fond.enabled = false;
                isActive = false;
            }
        }
    }

    public void InstanciateProps()
    {
        if (cloneProps == null)
        {
            if (spawnProps.childCount > 0)
            {
                for (int i = 0; i < spawnProps.childCount; i++)
                {
                    Destroy(spawnProps.GetChild(i).gameObject);
                }
                rotationY = 0;
            }
            cloneProps = Instantiate(prefabProps);
            loreText.enabled = true;
            fond.enabled = true;
            cloneProps.transform.parent = spawnProps;
            cloneProps.transform.localPosition = Vector3.zero;
            cloneProps.transform.localRotation = Quaternion.Euler(0, 0, 0);
            cloneProps.transform.localScale = new Vector3(1, 1, 1);
        }

       // Debug.Log(gameObject);
    }
}