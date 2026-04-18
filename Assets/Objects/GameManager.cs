using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RobotCharacter robotCharacter;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI timeText;

    private float timer;

    private RobotCharacter.CurrentMode currentMode;

    private void Awake()
    {
        timeText.text = "";
    }

    void Start()
    {
        robotCharacter.ModeChange += ChangeMode;
        ChangeMode(RobotCharacter.CurrentMode.Looking);

        robotCharacter.StateChange += ChangeState;
    }

    private void OnDestroy()
    {
        robotCharacter.ModeChange -= ChangeMode;
        robotCharacter.StateChange -= ChangeState;
    }

    private void Update()
    {
        switch (currentMode)
        {
            case RobotCharacter.CurrentMode.Looking:
                break;
            case RobotCharacter.CurrentMode.Recording:
                timer += Time.deltaTime;
                timeText.text = timer.ToString("f3");
                break;
            case RobotCharacter.CurrentMode.Playing:
                timer -= Time.deltaTime;
                timeText.text = Mathf.Clamp(timer, 0, 100).ToString("f3");
                break;
            case RobotCharacter.CurrentMode.RealTimePlaying:
                break;
            default:
                break;
        }

    }

    private void ChangeMode(RobotCharacter.CurrentMode currentMode)
    {
        switch (currentMode)
        {
            case RobotCharacter.CurrentMode.Looking:
                modeText.text = "Press Start when you are ready to record";
                break;
            case RobotCharacter.CurrentMode.Recording:
                modeText.text = "Recording Inputs";
                break;
            case RobotCharacter.CurrentMode.Playing:
                modeText.text = "Playing Inputs";
                break;
            case RobotCharacter.CurrentMode.RealTimePlaying:
                break;
            default:
                break;
        }

        this.currentMode = currentMode;
    }

    private void ChangeState(RobotCharacter.CurrentState currentState)
    {
        switch (currentState)
        {
            case RobotCharacter.CurrentState.Alive:
                break;
            case RobotCharacter.CurrentState.Dead:
                Debug.Log("Death");
                LevelManager.Instance.LoadCurrentScene();
                break;
            case RobotCharacter.CurrentState.Win:
                Debug.Log("Win");
                LevelManager.Instance.LoadNextScene();
                break;
            default:
                break;
        }
    }



}
