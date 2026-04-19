using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject OptionsTab;
    private bool isOptionsOpen = false;

    [SerializeField] private GameObject LevelSelectTab;
    private bool isLevelSelectOpen = false;

    [SerializeField] private GameObject quitButton;

    private bool canInteract = true;

    [SerializeField] private AudioMixerGroup masterAudioMixer;
    [SerializeField] private AudioMixerGroup musicAudioMixer;
    [SerializeField] private AudioMixerGroup sfxAudioMixer;

    public enum MenuType
    {
        Main = 0,
        Options = 1,
        Quit = 2,
        LevelSelect = 3,
    }

    private void Start()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.SetActive(false);
        }

        OptionsTab.SetActive(false);
        LevelSelectTab.SetActive(false);
    }

    public void OpenMenu(int menuTypeID)
    {
        if (!canInteract)
            return; 

        switch ((MenuType)menuTypeID)
        {
            case MenuType.Main:
                canInteract = false;
                LevelManager.Instance.LoadScene(0, true);
                break;
            case MenuType.Options:
                OptionsTab.SetActive(!isOptionsOpen);
                isOptionsOpen = !isOptionsOpen;
                break;
            case MenuType.Quit:
                canInteract = false;
                LevelManager.Instance.QuitGame();
                break;
            case MenuType.LevelSelect:
                LevelSelectTab.SetActive(!isLevelSelectOpen);
                isLevelSelectOpen = !isLevelSelectOpen;
                break;
            default:
                break;
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterAudioMixer.audioMixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20);
    }
    public void SetMusicVolume(float volume)
    {
        musicAudioMixer.audioMixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20);
    }
    public void SetSFXVolume(float volume)
    {
        sfxAudioMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log(volume) * 20);
    }

}
