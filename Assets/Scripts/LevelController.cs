using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    //KeepingTrack of the currentLevel starting on the forestScene
    public static LEVEL currentLevel = LEVEL.FOREST;

    //Crossfade animationcontroller
    [SerializeField] Animator crossFadeImageAnimator;
    //Time to transition between scenes
    [SerializeField] float transitionTime = 1f;

    [SerializeField]
    Renderer skyDomeRender;
    public enum LEVEL
    {
        FOREST = 1,
        DESERT = 2,
        SNOW = 3,
        MULTIPLAYER = 4
    }

    void Awake()
    {
        SoundManager.Initialize();
        InitializeSkyDoneUVOffset();
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
            default:
                skyDomeRender.material.mainTextureOffset = new Vector3(-0.25f, 0, 0);
                break;
        }
    }

    //For loading Settings, Achievements and possibly other scenes
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneWithCrossFade(sceneIndex));
    }

    //For loading a level Scene: takes the current Level in view
    public void LoadScene()
    {
        StartCoroutine(LoadSceneWithCrossFade((int)currentLevel));
    }

    //https://www.youtube.com/watch?v=CE9VOZivb3I&ab_channel=Brackeys
    private IEnumerator LoadSceneWithCrossFade(int sceneIndex)
    {
        crossFadeImageAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }
}
