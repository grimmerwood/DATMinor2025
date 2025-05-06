using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    // This is a static variable. It can be accessed from any other script, and there is only one. This is called a "singleton".
    public static UIController Instance { get; private set; }

    [Tooltip("A reference to the player's Health component.")]
    public Health playerHealth;

    [Tooltip("A reference to the text component that displays the score.")]
    public TMP_Text scoreText;

    [Tooltip("The prefix to display before the score.")]
    public string scoreTextPrefix = "SCORE: ";

    [Tooltip("A reference to the text component that displays the player's health.")]
    public TMP_Text healthText;

    [Tooltip("The prefix to display before the health.")]
    public string healthTextPrefix = "Health: ";

    [Tooltip("A reference to the win screen game object. We will enable this when we win.")]
    public GameObject winScreen;

    [Tooltip("A reference to the lose screen game object. We will enable this when we lose.")]
    public GameObject loseScreen;

    // The current score.
    private int currentScore;

    private bool playerIsDead = false;
    private float restartTimer = 0f; // used for the 1 second delay

    private bool restartTriggeredFromSerial = false;

    void OnEnable()
    {
        ReceiveFromSerial.SerialDataReceived += OnSerialInput;
     }

    void OnDisable()
    {
         ReceiveFromSerial.SerialDataReceived -= OnSerialInput;
     }
     void OnSerialInput(int value)
     {
         if (playerIsDead)
         {
             Debug.Log("Restart triggered from Arduino.");
             restartTriggeredFromSerial = true;
         }
     }
    // Awake is called before Start. We want to do this in case other scripts want to use UIController in their Start method.
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        scoreText.text = scoreTextPrefix + currentScore;
    }

    public void Update()
    {
       
        // Handle restart logic after delay
        
        if (playerIsDead)
        {
            if (restartTimer > 0f)
            {
                restartTimer -= Time.deltaTime; // countdown the delay
            }
            else if (Input.GetKeyDown(KeyCode.Space) || restartTriggeredFromSerial)
            {
                restartTriggeredFromSerial = false; // Reset the flag
                RestartLevel();
            }
        }
    
        

        // Update health if player is still alive
        //if (playerHealth != null)
        {
            // healthText.text = healthTextPrefix + playerHealth.GetComponent<Health>().GetCurrentHitPoints();
        }
    }

    // Start is called before the first frame update


    public void PlayerDied()
    {
    playerIsDead = true;
    restartTimer = 1f;
    loseScreen.SetActive(true);
    healthText.text = healthTextPrefix + "0";
    }


    public void ChangeScore(int scoreChange)
    {
        currentScore += scoreChange;
        scoreText.text = scoreTextPrefix + currentScore;
    }

    public void UpdatePlayerHealth(float current, float max)
    {
        if (healthText != null)
        {
            healthText.text = healthTextPrefix + current.ToString("0");
        }
    }

    public void RestartLevel()
    {
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Reloads the currently active scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
    }
}
