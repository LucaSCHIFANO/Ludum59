using System.Collections.Generic;
using UnityEngine;

public class ButtonOrder : MonoBehaviour
{
    private Door door;

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private SpriteRenderer srNumber;
    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;


    [SerializeField] private List<Sprite> spriteList = new List<Sprite>();

    private bool isActivated;

    public void SetDoor(Door door, int id)
    {
        this.door = door;
        if(id < 0 || id >= spriteList.Count)
            srNumber.sprite = null;
        else srNumber.sprite = spriteList[id];
    }

    public void Activate(bool activate)
    {
        isActivated = activate;
        sr.sprite = activate ? on : off;
        if (activate == true)
            door?.ButtonActivated(this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isActivated)
        {
            Activate(true);
        }
    }
}
