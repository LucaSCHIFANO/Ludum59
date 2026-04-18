using System.Collections;
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

    [Header("Transition")]
    [SerializeField] private GameObject leftTransition;
    [SerializeField] private GameObject rightTransition;
    [SerializeField] private Animator anim;


    [Header("Scenes")]
    [SerializeField] private SceneAsset mainMenu;
    [SerializeField] private List<SceneAsset> listScene = new List<SceneAsset>();
    private int levelID;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        FadeOut();
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
        StartCoroutine(LoadSceneAnimation(id));
    }

    IEnumerator LoadSceneAnimation(int id)
    {
        FadeIn();
        yield return new WaitForSeconds(0.6f);
        TrueLoadScene(levelID);
        yield return new WaitForSeconds(0.2f);
        FadeOut();
    }

    private void TrueLoadScene(int id)
    {
        levelID = id;
        if (levelID < listScene.Count && levelID >= 0)
            SceneManager.LoadScene(listScene[levelID].name);
        else
            SceneManager.LoadScene(mainMenu.name);
    }

    private void FadeIn()
    {
        leftTransition.SetActive(true);
        rightTransition.SetActive(true);
        anim.Play("FadeIn");
    }

    private void FadeOut()
    {
        leftTransition.SetActive(true);
        rightTransition.SetActive(true);
        anim.Play("FadeOut");
    }
}
