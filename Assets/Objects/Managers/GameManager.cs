using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RobotCharacter robotCharacter;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI timeText;

    private float timer;

    private RobotCharacter.CurrentMode currentMode;
    private RobotCharacter.CurrentState currentState;

    [Header("Sounds")]
    [SerializeField] private SOSound beat;
    [SerializeField] private SOSound victory;
    [SerializeField] private SOSound death;

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
                InvokeRepeating("PlayBeat", 0, 1);
                break;
            case RobotCharacter.CurrentMode.Playing:
                modeText.text = "Playing Inputs";
                CancelInvoke("PlayBeat");
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
                if (this.currentState == RobotCharacter.CurrentState.Win)
                    return;
                this.currentState = RobotCharacter.CurrentState.Dead;
                SoundManager.Instance.Play(death);
                LevelManager.Instance.LoadCurrentScene();
                break;
            case RobotCharacter.CurrentState.Win:
                if (this.currentState == RobotCharacter.CurrentState.Dead)
                    return;
                this.currentState = RobotCharacter.CurrentState.Win;
                SoundManager.Instance.Play(victory);
                LevelManager.Instance.LoadNextScene();
                break;
            default:
                break;
        }
    }

    private void PlayBeat()
    {
        SoundManager.Instance.Play(beat);
    }

}
