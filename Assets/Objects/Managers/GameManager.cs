using TMPro;
using UnityEngine;
using static RobotCharacter;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RobotCharacter robotCharacter;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject infoText;
    [SerializeField] private bool showInfoText;

    private float timer;

    private RobotCharacter.CurrentMode currentMode;
    private RobotCharacter.CurrentState currentState;

    [Header("Sounds")]
    [SerializeField] private SOSound beat;
    [SerializeField] private SOSound victory;
    [SerializeField] private SOSound death;

    public delegate void ResetAllTraps();
    public event ResetAllTraps ResetTraps;

    public delegate void ShowAllInfos(bool show);
    public event ShowAllInfos ShowInfos;

    static GameManager instance;

    private int call = 0;
    public static GameManager Instance
    {
        get => instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        timeText.text = "";
        infoText.SetActive(showInfoText);
    }

    /// <summary>
    /// Subscribes to delegated
    /// </summary>

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

    /// <summary>
    /// Start or stop the timer based on the player mode
    /// </summary>
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
                if(call > 1)
                timer -= Time.deltaTime;
                timeText.text = Mathf.Clamp(timer, 0, 100).ToString("f3");
                break;
            case RobotCharacter.CurrentMode.RealTimePlaying:
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Updates the text based on the player mode
    /// </summary>
    /// <param name="currentMode"></param>
    private void ChangeMode(RobotCharacter.CurrentMode currentMode)
    {
        switch (currentMode)
        {
            case RobotCharacter.CurrentMode.Looking:
                modeText.text = "Press Start/Enter when \n\ryou are ready to record";
                break;
            case RobotCharacter.CurrentMode.Recording:
                modeText.text = "Recording Inputs... \n\rPress Start/Enter again to stop recording";
                ResetTraps?.Invoke();
                InvokeRepeating("PlayBeat", 0, 1);
                break;
            case RobotCharacter.CurrentMode.Playing:
                modeText.text = "Playing Inputs...";
                if (call != 0)
                    ResetTraps?.Invoke();
                call++;
                CancelInvoke("PlayBeat");
                break;
            case RobotCharacter.CurrentMode.RealTimePlaying:
                break;
            default:
                break;
        }

        this.currentMode = currentMode;
    }

    /// <summary>
    /// Play sound or load scene based on player state
    /// </summary>
    /// <param name="currentState"></param>
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

    public void ShowInfo(bool show)
    {
        ShowInfos?.Invoke(show);
    }

    private void PlayBeat()
    {
        SoundManager.Instance.Play(beat);
    }

}
