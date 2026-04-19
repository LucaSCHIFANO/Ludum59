using TMPro;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] int levelID;

    public void Start()
    {
        name = $"Level {levelID + 1}";
        text.text = $"Level {levelID + 1}";
    }

    public void LoadLevel()
    {
        LevelManager.Instance.LoadScene(levelID, true);
    }
}
