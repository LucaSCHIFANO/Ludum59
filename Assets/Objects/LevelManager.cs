using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    static LevelManager instance;
    public static LevelManager Instance
    {
        get => instance;
    }

    private int levelID;

    [SerializeField] private SceneAsset mainMenu;
    [SerializeField] private List<SceneAsset> listScene = new List<SceneAsset>();

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetLevelId()
    {
       return levelID; 
    }
    
    public void LoadNextScene()
    {
        levelID++;
        LoadScene(levelID);
    }

    public void LoadCurrentScene()
    {
        LoadScene(levelID);
    }

    public void LoadScene(int id)
    {
        levelID = id;
        if (levelID < listScene.Count && levelID >= 0)
            SceneManager.LoadScene(listScene[levelID].name);
        else
            SceneManager.LoadScene(mainMenu.name);
    }
}
