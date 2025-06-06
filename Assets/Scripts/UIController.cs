using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    // This is a static variable. It can be accessed from any other script, and there is only one. This is called a "singleton".
    public static UIController Instance;
     public GameObject restartButton;
     public GameObject gameOverText;

    [Tooltip("A reference to the player's Health component.")]
    public Health playerHealth;

    [Tooltip("A reference to the text component that displays the score.")]
    public TMP_Text scoreText;

    [Tooltip("The prefix to display before the score.")]
    public string scoreTextPrefix = "SCORE: ";

    [Tooltip("A reference to the text component that displays the player's health.")]
    public TMP_Text healthText;

    [Tooltip("The prefix to display before the health.")]
    public string healthTextPrefix = "HEALTH: ";

    [Tooltip("A reference to the win screen game object. We will enable this when we win.")]
    public GameObject winScreen;

    [Tooltip("A reference to the lose screen game object. We will enable this when we lose.")]
    public GameObject loseScreen;

    // The current score.
    private int currentScore;

    public GameObject gameOverPanel;
    

    private bool playerIsDead = false;
    private float restartTimer = 0f; // used for the 1 second delay

    // Awake is called before Start. We want to do this in case other scripts want to use UIController in their Start method.
    public void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        scoreText.text = scoreTextPrefix + currentScore;

        if (playerHealth == null)
    {
        playerHealth = FindObjectOfType<Health>();
    }
    }

    public void Update()
    {
        // If player object is missing and we haven't flagged them dead yet
        if (playerHealth == null && !playerIsDead)
        {
            playerIsDead = true;
            restartTimer = 1f; // Start the delay
            loseScreen.SetActive(true);
            healthText.text = healthTextPrefix + "0";
        }

        // Handle restart logic after delay
        if (playerIsDead)
        {
            if (restartTimer > 0f)
            {
                restartTimer -= Time.deltaTime; // countdown the delay
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
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

    public void ShowGameOverScreen()
{
    if (gameOverPanel != null)
    {
        gameOverPanel.SetActive(true);
    }
}
    public void RestartLevel()
    {
       // Reset score
       currentScore = 0;
       scoreText.text = scoreTextPrefix + currentScore;

      // Reset player death flag
      playerIsDead = false;
      restartTimer = 0f;

       playerHealth = FindObjectOfType<Health>(); // Reassign after reload
      // Reset player health
      if (playerHealth != null)
    {
          playerHealth.ResetHealth();  // Make sure this resets player health correctly
    }

    // Reload the current scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Reloads the currently active scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
    }
}
