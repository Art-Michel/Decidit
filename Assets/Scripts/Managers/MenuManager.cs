using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance = null;
    [SerializeField] int _gameIndex;
    [SerializeField] int _optionIndex;

    private void Awake()
    {
        if(Instance != null)
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
    #endregion
}
