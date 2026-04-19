using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Sprite IconOn;
    [SerializeField] private Sprite IconOff;

    [SerializeField] private GameObject colliderClosed;
    [SerializeField] private GameObject colliderOpen;

    [SerializeField] private List<SpriteRenderer> listLight = new List<SpriteRenderer>();
    [SerializeField] private List<ButtonOrder> listButtons = new List<ButtonOrder>();
    [SerializeField] private Animator anim;
    private int buttonID;
    private bool isOpen;

    [SerializeField] private SOSound openDoor;

    void Start()
    {
        for (int i = 0; i < listLight.Count; i++)
        {
            listLight[i].sprite = IconOff;
            listButtons[i].SetDoor(this, i);
        }

        GameManager.Instance.ResetTraps += Reset;
        colliderOpen.SetActive(false);
        colliderClosed.SetActive(true);
    }

    public void ButtonActivated(ButtonOrder button)
    {
        if (isOpen)
            return;

        if (button == listButtons[buttonID])
        {
            listLight[buttonID].sprite = IconOn;
            buttonID++;
            if(buttonID == listLight.Count)
            {
                Open();    
            }
        }
        else
        {
            Reset();
        }
    }

    private void Open()
    {
        isOpen = true;
        anim.Play("DoorButtonOpening");

        for (int i = 0; i < listLight.Count; i++)
        {
            listLight[i].sprite = IconOn;
        }

        colliderOpen.SetActive(true);
        colliderClosed.SetActive(false);

        SoundManager.Instance.Play(openDoor);
    }

    private void Reset()
    {
        buttonID = 0;
        for (int i = 0; i < listButtons.Count; i++)
        {
            listButtons[i].Activate(false);
            listLight[i].sprite = IconOff;
        }

    }
}
