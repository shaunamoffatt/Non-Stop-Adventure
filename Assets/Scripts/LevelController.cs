using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    //KeepingTrack of the currentLevel starting on the forestScene
    public static LEVEL currentLevel = LEVEL.FOREST;
    public static bool paused = false;

    //Crossfade animationcontroller
    [SerializeField] Animator crossFadeImageAnimator;
    //Time to transition between scenes
    [SerializeField] float transitionTime = 1f;

    //Change the uv for every level so it seems to be different times of day
    [SerializeField]
    Renderer skyDomeRender;
    public enum LEVEL
    {
        FOREST = 1,
        DESERT = 2,
        SNOW = 3,
        MULTIPLAYER = 4,
        SETTINGS = 5

    }

    void Awake()
    {
        SoundManager.Initialize();
        InitializeSkyDoneUVOffset();
        //If we are back in the menu set the current level to be the forest where we usually start
        if(currentLevel == 0)
        {
            currentLevel = LEVEL.FOREST;
            Debug.Log("Set to be forrst level");
        }
    }

    private void InitializeSkyDoneUVOffset()
    {
        switch (currentLevel)
        {
            case LEVEL.FOREST:
                skyDomeRender.material.mainTextureOffset = new Vector3(-0.21f, 0, 0);
                break;
            case LEVEL.DESERT:
                skyDomeRender.material.mainTextureOffset = new Vector3(0, 0, 0);
                break;
            case LEVEL.SNOW:
                skyDomeRender.material.mainTextureOffset = new Vector3(0.25f, 0, 0);
                break;
            case LEVEL.MULTIPLAYER:
                skyDomeRender.material.mainTextureOffset = new Vector3(0.5f, 0, 0);
                break;
            case LEVEL.SETTINGS:
                skyDomeRender.material.mainTextureOffset = new Vector3(-0.74f, 0, 0);
                break;
            default:
                skyDomeRender.material.mainTextureOffset = new Vector3(-0.25f, 0, 0);
                break;
        }
    }

    //For loading Settings, Achievements and possibly other scenes
    public void LoadScene(int sceneIndex)
    {
        
        currentLevel = (LEVEL)sceneIndex;
        StartCoroutine(LoadSceneWithCrossFade(sceneIndex));
    }

    //For loading a level Scene: takes the current Level in view
    public void LoadCurrentScene()
    {
        StartCoroutine(LoadSceneWithCrossFade(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadScene()
    {
        StartCoroutine(LoadSceneWithCrossFade((int)currentLevel));
    }

    //https://www.youtube.com/watch?v=CE9VOZivb3I&ab_channel=Brackeys
    private IEnumerator LoadSceneWithCrossFade(int sceneIndex)
    {
        crossFadeImageAnimator.SetTrigger("Start");
        Debug.Log("HEY Loading levelIndex: " + sceneIndex);
        Time.timeScale = 1;
        paused = false;
        SceneManager.LoadScene(sceneIndex);
        yield return new WaitForSeconds(transitionTime);

        
    }
}

