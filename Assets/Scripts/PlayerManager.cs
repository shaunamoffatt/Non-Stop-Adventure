using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] public GameObject player;
    //Handle Collectable counts
    [SerializeField] TMP_Text countDisplay;
    public static int collectableCount = 0;
    private static int previousCount = 0;

    //For Singleton
    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
        SoundManager.Initialize();
    }

    private void Start()
    {
         countDisplay.text = collectableCount.ToString();
    }

    private void FixedUpdate()
    {
        if (previousCount != collectableCount)
        {
            countDisplay.text = collectableCount.ToString();
            previousCount++;
        }
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
