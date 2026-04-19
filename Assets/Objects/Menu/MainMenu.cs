using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsTab;
    [SerializeField] private GameObject levelSelectTab;
    [SerializeField] private GameObject tutoTab;

    [SerializeField] private GameObject quitButton;

    private bool canInteract = true;

    [SerializeField] private AudioMixerGroup masterAudioMixer;
    [SerializeField] private AudioMixerGroup musicAudioMixer;
    [SerializeField] private AudioMixerGroup sfxAudioMixer;

    [Header("Cursor")]
    [SerializeField] private GameObject cursor;
    [SerializeField] private List<Transform> cursorPositionList = new List<Transform>();

    private int buttonID = 0;
    private bool inSubMenu = false;

    public enum MenuType
    {
        Main = 0,
        LevelSelect = 1,
        Options = 2,
        Tuto = 3,
        Quit = 4,
    }

    private void Start()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.SetActive(false);
        }

        optionsTab.SetActive(false);
        levelSelectTab.SetActive(false);

        Invoke("SetButtonPos", 0.5f);
    }

    private void SetButtonPos()
    {
        cursor.transform.position = cursorPositionList[buttonID].position;
    }

    public void OpenMenu(int menuTypeID)
    {
        if (!canInteract)
            return; 

        buttonID = menuTypeID;
        cursor.transform.position = cursorPositionList[buttonID].position;

        switch ((MenuType)menuTypeID)
        {
            case MenuType.Main:
                canInteract = false;
                LevelManager.Instance.LoadScene(0, true);
                break;
            case MenuType.Options:
                inSubMenu = !inSubMenu;
                optionsTab.SetActive(inSubMenu);
                break;
            case MenuType.LevelSelect:
                inSubMenu = !inSubMenu;
                levelSelectTab.SetActive(inSubMenu);
                break;
            case MenuType.Tuto:
                inSubMenu = !inSubMenu;
                tutoTab.SetActive(inSubMenu);
                break;
            case MenuType.Quit:
                canInteract = false;
                LevelManager.Instance.QuitGame();
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

    private void Move(bool up)
    {
        if (up)
            buttonID--;
        else 
            buttonID++;

        if (buttonID < 0)
            buttonID = cursorPositionList.Count - 1;
        else if(buttonID > cursorPositionList.Count - 1)
            buttonID = 0;

        SetButtonPos();
    }

    public void UpInput(InputAction.CallbackContext context)
    {
        if (context.performed && !inSubMenu)
        {
            Move(true);
        }
    }

    public void DownInput(InputAction.CallbackContext context)
    {
        if (context.performed && !inSubMenu)
        {
            Move(false);
        }
    }

    public void ConfirmInput(InputAction.CallbackContext context)
    {
        if (context.performed && !inSubMenu)
        {
            OpenMenu(buttonID);
        }
    }

    public void CancelInput(InputAction.CallbackContext context)
    {
        if (context.performed && inSubMenu)
        {
            OpenMenu(buttonID);
        }
    }

}
