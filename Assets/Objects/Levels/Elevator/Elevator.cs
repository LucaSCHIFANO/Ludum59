using TMPro;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform targetPosition;
    [SerializeField] private Transform initialPosition;
    [SerializeField] private float travelTime;

    [SerializeField] private GameObject infoBox;
    [SerializeField] private TextMeshProUGUI textInfoBox;

    private Rigidbody2D rb;

    private float speed;
    private Transform currentTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Set up the text message
    /// </summary>
    private void Start()
    {
        initialPosition.position = transform.position;
        speed = Vector2.Distance(initialPosition.position, targetPosition.position) / travelTime;

        currentTarget = targetPosition;
        infoBox.transform.parent = currentTarget;

        GameManager.Instance.ResetTraps += Reset;
        GameManager.Instance.ShowInfos += ShowInfo;

        textInfoBox.text = $"Travel Time : {travelTime}\r\n" +
            $"Time to return to initial position : {travelTime*2}";
    }

    private void OnDestroy()
    {
        GameManager.Instance.ResetTraps -= Reset;
        GameManager.Instance.ShowInfos -= ShowInfo;
    }

    private void FixedUpdate()
    {
        if(Vector2.Distance(transform.position, currentTarget.position) < 0.01f)
        {
            if (currentTarget == initialPosition)
                currentTarget = targetPosition;
            else if (currentTarget == targetPosition)
                currentTarget = initialPosition;
        }

        Vector3 targetDirection = (currentTarget.position - transform.position).normalized;
        rb.MovePosition(transform.position + targetDirection * speed * Time.deltaTime);
    }

    /// <summary>
    /// Used to move the platform between 2 points
    /// </summary>
    /// <returns></returns>
    private void Reset()
    {
        var saveInitialPos = initialPosition.position;
        var saveTargetPos = targetPosition.position;

        transform.position = initialPosition.position;
        currentTarget = targetPosition;

        initialPosition.position = saveInitialPos;
        targetPosition.position = saveTargetPos;

        Vector3 targetDirection = (currentTarget.position - transform.position).normalized;
        rb.MovePosition(transform.position + targetDirection * speed * Time.deltaTime);
    }
    public void ShowInfo(bool show)
    {
        infoBox.SetActive(show);
    }
}
