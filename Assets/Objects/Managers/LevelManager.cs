using System.Collections;
using System.Collections.Generic;
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
    private bool canReset;

    [Header("Scenes")]
    [SerializeField] private LevelList listScene;
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
        StartCoroutine(SpawnAnimation());
    }

    /// <summary>
    /// Manages start transition animation
    /// </summary>
    IEnumerator SpawnAnimation()
    {
        canReset = false;
        FadeOut();
        yield return new WaitForSeconds(0.6f);
        canReset = true;
        leftTransition.SetActive(false);
        rightTransition.SetActive(false);
    }

    public int GetLevelId()
    {
        return levelID;
    }

    public void LoadNextScene(bool skipWait = false)
    {
        levelID++;
        LoadScene(levelID, skipWait);
    }

    public void LoadCurrentScene(bool skipWait = false)
    {
        LoadScene(levelID, skipWait);
    }

    public void LoadScene(int id, bool skipWait = false)
    {
        if (!canReset)
            return;

        StartCoroutine(LoadSceneAnimation(id, skipWait));
    }

    public void LoadMainMenu(bool skipWait = false)
    {
        if (!canReset)
            return;
        StartCoroutine(LoadSceneAnimation(-1, skipWait));
    }

    /// <summary>
    /// Manages transition animation between 2 scenes
    /// </summary>
    IEnumerator LoadSceneAnimation(int id, bool skipWait = false)
    {
        canReset = false;

        if (!skipWait)
            yield return new WaitForSeconds(1f);

        FadeIn();
        yield return new WaitForSeconds(0.6f);
        TrueLoadScene(id);
        yield return new WaitForSeconds(0.2f);
        FadeOut();
        yield return new WaitForSeconds(0.6f);
        leftTransition.SetActive(false);
        rightTransition.SetActive(false);
        canReset = true;
    }

    /// <summary>
    /// Function that actually load the scene
    /// </summary>
    /// <param name="id"></param>
    private void TrueLoadScene(int id)
    {
        levelID = id;
        if (levelID < listScene.listScene.Count && levelID >= 0)
            SceneManager.LoadScene(listScene.listScene[levelID]);
        else
            SceneManager.LoadScene(listScene.mainMenu);
    }

    public void QuitGame()
    {
        StartCoroutine(LoadSceneAnimation());
    }

    IEnumerator LoadSceneAnimation()
    {
        FadeIn();
        yield return new WaitForSeconds(0.6f);
        Application.Quit();
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
