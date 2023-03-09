using UnityEngine;
using TMPro;

public enum ENDSTATE
{
    WIN,
    LOSE,
    TIMEOUT
}

public class PlayerManager : MonoBehaviour
{
    //Keep track of the endstate of the leve
    public static ENDSTATE levelState = ENDSTATE.WIN;

    //UI for resetting or going back to menu
    public GameObject EndLevelUI;
    private TMP_Text endText;

    [SerializeField] public GameObject player;
    //Handle Collectable counts
    [SerializeField] TMP_Text countDisplay;
    public static int collectableCount = 0;
    private static int previousCount = 0;
    //Handle Score
    [SerializeField] TMP_Text scoreDisplay;
    public static int playerScore;
    private static int previousScore = 0;

    //For Singleton
    public static PlayerManager instance;

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        //get the tmptext fromt eh ui
        endText = EndLevelUI.GetComponentInChildren<TMP_Text>();
        countDisplay.text = collectableCount.ToString();
        scoreDisplay.text = playerScore.ToString();
    }
    private void SetEndLevelText()
    {
        switch (levelState)
        {
            case ENDSTATE.WIN:
                endText.text = "YOU WIN!\n boop";
                break;
            case ENDSTATE.LOSE:
                endText.text = "YOU LOSE";
                break;
            case ENDSTATE.TIMEOUT:
                endText.text = "OUT OF TIME!";
                break;
        }
    }

    private void Update()
    {
        if (previousCount != collectableCount)
        {
            countDisplay.text = collectableCount.ToString();
            previousCount++;
        }
        if (previousCount != previousScore)
        {
            scoreDisplay.text = playerScore.ToString();
            previousCount = playerScore;
        }
        if (LevelController.paused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    void PauseGame()
    {
        SetEndLevelText();
        EndLevelUI.SetActive(true);
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        EndLevelUI.SetActive(false);
        Time.timeScale = 1;
    }

    //Used in the StartCountDown class to stop the players input and particles
    public void BeginPlay()
    {
        DeactivateAllParticle();
        player.GetComponent<InputControls>().enabled = true;
    }

    private void DeactivateAllParticle()
    {
        ParticleSystem[] childrenParticleSytems = player.GetComponentsInChildren<ParticleSystem>();
        Debug.Log("Number of Player Particles = " + childrenParticleSytems.Length);
        foreach (ParticleSystem childPS in childrenParticleSytems)
        {
            childPS.gameObject.SetActive(false);
        }
    }
}