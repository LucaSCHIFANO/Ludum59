using System.Collections.Generic;
using UnityEngine;

public class ButtonOrder : MonoBehaviour
{
    private Door door;
    [SerializeField] private GameObject visual;
    [SerializeField] private List<Sprite> spriteList = new List<Sprite>();
    private SpriteRenderer sr;

    private bool isActivated;

    private void Awake()
    {
        //sr = GetComponent<SpriteRenderer>();    
    }

    public void SetDoor(Door door, int id)
    {
        this.door = door;
        /*if(id < 0 || id >= spriteList.Count)
            sr.sprite = null;
        else sr.sprite = spriteList[id];*/
    }

    public void Activate(bool activate)
    {
        isActivated = activate;
        visual.SetActive(!activate);
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
