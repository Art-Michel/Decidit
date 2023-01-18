using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance = null;
    [SerializeField] int _gameIndex;
    [SerializeField] int _optionIndex;
    //[SerializeField] List<Sprite> sprites;

    [SerializeField] GameObject firstSelected;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        SetSelectedGameObjectToSettings();
    }
    #region Butons fonctions
    //toutes les fonctions pour les boutons 
    public void Quit()
    {
        //fait quitter le jeu
        Application.Quit();
    }

    public void LoadGame()
    {
        //fait entrer le joueur dans la scene de jeu
        SceneManager.LoadScene(_gameIndex);
    }

    public void Options()
    {
        //fait entrer le joueur dans les options
        SceneManager.LoadScene(_optionIndex);
    }

    public void SetSelectedGameObjectToSettings()
    {
        //Clear
        //EventSystem.current.SetSelectedGameObject(null);

        if (EventSystem.current == null || EventSystem.current == sprites)
        {
            Debug.Log("vbksdbgjbkjnkgnklsmnkgln,lnkdglkbndklnbsknbknkl,ngs");
            //Reassign
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    #endregion
}
