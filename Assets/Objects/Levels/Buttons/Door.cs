using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private List<GameObject> listLight = new List<GameObject>();
    [SerializeField] private List<ButtonOrder> listButtons = new List<ButtonOrder>();
    private int buttonID;

    [SerializeField] private GameObject door;
    private bool isOpen;

    void Start()
    {
        for (int i = 0; i < listLight.Count; i++)
        {
            listLight[i].SetActive(false);
            listButtons[i].SetDoor(this, i);
        }

        GameManager.Instance.ResetTraps += Reset;
    }

    public void ButtonActivated(ButtonOrder button)
    {
        if (isOpen)
            return;

        if (button == listButtons[buttonID])
        {
            listLight[buttonID].SetActive(true);
            buttonID++;
            if(buttonID == listLight.Count)
            {
                isOpen = true;
                door.SetActive(false);
                for (int i = 0; i < listLight.Count; i++)
                {
                    listLight[i].SetActive(false);
                }
            }
        }
        else
        {
            Reset();
        }
    }

    private void Reset()
    {
        buttonID = 0;
        for (int i = 0; i < listButtons.Count; i++)
        {
            listButtons[i].Activate(false);
            listLight[i].SetActive(false);
        }

    }
}
