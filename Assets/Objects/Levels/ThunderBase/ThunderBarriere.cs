using System.Collections;
using TMPro;
using UnityEngine;

public class ThunderBarriere : MonoBehaviour
{
    [SerializeField] private Animator animBase;
    [SerializeField] private GameObject thunder;

    [SerializeField] private GameObject infoBox;
    [SerializeField] private TextMeshProUGUI textInfoBox;

    [SerializeField] private float timeBeforeActivation;
    [SerializeField] private float timeBetweenActivation;
    [SerializeField] private float timeActive;

    private void Start()
    {
        GameManager.Instance.ResetTraps += Reset;
        GameManager.Instance.ShowInfos += ShowInfo;
        Reset();

        textInfoBox.text = $"Time Before Activation : {timeBeforeActivation}\r\n" +
            $"Active Time : {timeActive} \r\n" +
            $"Time Between Activations : {timeBetweenActivation}";
    }

    private void OnDestroy()
    {
        GameManager.Instance.ResetTraps -= Reset;
        GameManager.Instance.ShowInfos -= ShowInfo;
    }

    private void Reset()
    {
        StopAllCoroutines();
        thunder.SetActive(false);
        Invoke("StartThunder", timeBeforeActivation);
    }

    private void StartThunder()
    {
        StartCoroutine(Thunder());
    }

    IEnumerator Thunder()
    {
        thunder.SetActive(true);
        animBase.Play("ThunderBaseAnim_BaseActive");
        yield return new WaitForSeconds(timeActive);
        animBase.Play("ThunderBaseAnim_Base");
        thunder.SetActive(false);
        yield return new WaitForSeconds(timeBetweenActivation);
        StartCoroutine(Thunder());
    }

    public void ShowInfo(bool show)
    {
        infoBox.SetActive(show);
    }
}
