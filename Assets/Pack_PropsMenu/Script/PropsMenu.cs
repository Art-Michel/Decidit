using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [Header("Rotation Objet")]
    [SerializeField] float rotationY;
    [SerializeField] float rotSpeedY;
    [SerializeField] float rotationX;
    [SerializeField] float rotSpeedX;
    [SerializeField] bool canRotateY;

    [SerializeField] List<GameObject> buttonList = new List<GameObject>();

    void Start()
    {
        Time.timeScale = 1;

        m_EventSystem = EventSystem.current;
        StartCoroutine("SelectFirstButton");

        loreText = transform.Find("Lore").GetComponent<Text>();
        parentButton = transform.parent;
        fond = transform.Find("Fond").GetComponent<Image>();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
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

                    if(canRotateY)
                    {
                        rotationX += rotSpeedX * Time.deltaTime * Input.GetAxis("Mouse Y");
                        float yRot = cloneProps.transform.GetChild(0).transform.localRotation.eulerAngles.y;
                        cloneProps.transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, yRot, rotationX);
                    }
                }
            }
        }
    }

    void PrintUI()
    {
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
            CameraPropsMenu.instance.ResetCam();

            if (spawnProps.childCount > 0)
            {
                for (int i = 0; i < spawnProps.childCount; i++)
                {
                    Destroy(spawnProps.GetChild(i).gameObject);
                }
            }
            cloneProps = Instantiate(prefabProps);
            loreText.enabled = true;
            fond.enabled = true;
            cloneProps.transform.parent = spawnProps;
            ResetRotation();
            cloneProps.transform.localPosition = Vector3.zero;
            cloneProps.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void ResetRotation()
    {
        rotationY = 0;
        rotationX = 0;
        cloneProps.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}