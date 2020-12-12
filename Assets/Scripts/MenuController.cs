using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject earth;
    [SerializeField] GameObject skyDome;

    [SerializeField] private float scaleTime = 0.1f;
    [SerializeField] private float durationOfSpin = 1f;

    //keeping track of the current level we might pick
    private LEVEL currentLevel = LEVEL.FOREST;

    //Size/Scale of the buttons
    [SerializeField] private Vector3 startingScale = new Vector3(1, 1, 1);
    //rotating puts a lock on the arrow buttons
    private bool rotating = false;
    //TODO fix this
    private const string RIGHTBUTTON = "RightButton";
    bool arrowRight;
    //Crossfade animationcontroller
    [SerializeField]Animator anim;
    //Time to transition between scenes
    [SerializeField] float transitionTime = 1f;

    protected Vector3[] levelLocations = {
        new Vector3(-200, 192, -272),//Level 1 Forest location
        new Vector3(-140,70,-140),//Level 2 Desert location
        new Vector3(-33,-20,95),//LEvel 2 location
        new Vector3(160,0,0)//Level 4 multi location
    };

    Renderer skyDomeRender;

    private enum LEVEL
    {
        FOREST = 1,
        DESERT = 2,
        SNOW = 3,
        MULTIPLAYER = 4
    }

    private void Start()
    {
        
        if (earth == null)
            earth = GameObject.Find("earth");
        if (skyDome == null)
            skyDome = GameObject.Find("SkyDome");

        skyDomeRender = skyDome.GetComponent<Renderer>();
        //game starts at level 1 : Forest
        earth.transform.rotation = Quaternion.Euler(levelLocations[0]);
        //have the gameobject set to scale just in case one hasnt been set
        startingScale = transform.localScale;
    }

    public void ChangeLevelSelect(Button b)
    {
        if (rotating == false)
        {
            rotating = true;
            string arrowName = b.gameObject.name;
            nextLevel(arrowName == RIGHTBUTTON);
            StartCoroutine(ScaleUpAndDown(b));
            StartCoroutine(RotateEarthandSky());
        }
    }

    //https://docs.unity3d.com/ScriptReference/Material-mainTextureOffset.html?_ga=2.58027117.652387074.1606933424-562702184.1602072928
    IEnumerator RotateEarthandSky()
    {
        float offset = (arrowRight) ? 0.25f : -0.25f;
        Vector3 startingUV = skyDomeRender.material.mainTextureOffset;
        Vector3 endingUV = startingUV + new Vector3(offset, 0, 0);
        Quaternion finalRotation = Quaternion.Euler(levelLocations[(int)currentLevel-1]);//minus 1 as the LEVEL starts at 1

        if (durationOfSpin > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + durationOfSpin;

            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / durationOfSpin;
                //spin the earth
                earth.transform.rotation = Quaternion.Slerp(earth.transform.rotation, finalRotation, progress);
                //spin the skydome UVS
                skyDomeRender.material.mainTextureOffset = Vector2.Lerp(startingUV, endingUV, progress);
                yield return null;
            }
        }
        earth.transform.rotation = finalRotation;
        skyDomeRender.material.mainTextureOffset = endingUV;
     
        rotating = false;
    }

    public void ScaleButton(Button b)
    {
        StartCoroutine(ScaleUpAndDown(b));
    }

    IEnumerator ScaleUpAndDown(Button b)
    {
        Vector3 myScale = b.transform.localScale * 1.1f;
        for (float time = 0; time < scaleTime * 2; time += Time.deltaTime)
        {
            float progress = Mathf.PingPong(time, scaleTime) / scaleTime;
            b.transform.localScale = Vector3.Lerp(startingScale, myScale, progress);
            yield return null;
        }
        //reset scale and unlock button
        b.transform.localScale = startingScale;
    }

    private void nextLevel(bool arrowGoingRight)
    {
        arrowRight = arrowGoingRight;
        switch (currentLevel)
        {
            case LEVEL.FOREST:
                currentLevel = (arrowRight) ? LEVEL.DESERT : LEVEL.MULTIPLAYER;
                break;
            case LEVEL.DESERT:
                currentLevel = (arrowRight) ? LEVEL.SNOW : LEVEL.FOREST;
                break;
            case LEVEL.SNOW:
                currentLevel = (arrowRight) ? LEVEL.MULTIPLAYER : LEVEL.DESERT;
                break;
            case LEVEL.MULTIPLAYER:
                currentLevel =  (arrowRight) ? LEVEL.FOREST : LEVEL.SNOW;
                break;
        }

        Debug.Log("Switched to Level : " + currentLevel);
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
        anim.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }
}