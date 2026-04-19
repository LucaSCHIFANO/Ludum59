using TMPro;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] int levelID;

    public void Start()
    {
        name = $"Level {levelID + 1}";
    }

    public void LoadLevel()
    {
        LevelManager.Instance.LoadScene(levelID, true);
    }
}
