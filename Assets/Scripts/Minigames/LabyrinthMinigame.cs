using UnityEngine;
using TMPro;

public class LabyrinthMinigame : MonoBehaviour
{
    [SerializeField] float timer = 120f;
    [SerializeField] Transform startPosition;
    [SerializeField] Transform player;
    [SerializeField] Canvas timerCanvas;
    [SerializeField] TMP_Text timerText;

    public bool startingTimer;
    float currentTime;

    private void Start()
    {
        InitialSettings();
    }

    void InitialSettings()
    {
        currentTime = timer;
        startingTimer = false;

        timerCanvas.enabled = false;
        ShowTime(currentTime);
    }

    private void Update()
    {
        if (startingTimer == true)
        {
            currentTime -= Time.deltaTime;
            Debug.Log("Tiempo restante: " + currentTime);
            ShowTime(currentTime);

            if (currentTime <= 0f)
            {
                startingTimer = false;
                currentTime = timer;
                player.position = startPosition.position;
                timerText.text = "00:00";
                timerCanvas.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startingTimer = true;
            timerCanvas.enabled = true;
        }
    }

    void ShowTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);

        timerText.text = string.Format("{00:00}:{01:00}", minutes, seconds);
    }
}
