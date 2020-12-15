using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject earth;
    [SerializeField] GameObject skyDome;

    //keeping track of the current level we might pick
    private LevelController.LEVEL currentLevel = LevelController.LEVEL.FOREST;

    [SerializeField] private float durationOfSpin = 1f;

    //rotating puts a lock on the arrow buttons
    private bool rotating = false;
    //TODO fix this
    private const string RIGHTBUTTON = "RightButton";
    bool arrowRight;

    Renderer skyDomeRender;

    protected Vector3[] levelLocations = {
        new Vector3(-200, 192, -272),//Level 1 Forest location
        new Vector3(-140,70,-140),//Level 2 Desert location
        new Vector3(-33,-20,95),//LEvel 2 location
        new Vector3(160,0,0)//Level 4 multi location
    };

    private void Start()
    {
        if (earth == null)
            earth = GameObject.Find("earth");
        if (skyDome == null)
            skyDome = GameObject.Find("SkyDome");

        skyDomeRender = skyDome.GetComponent<Renderer>();
        //Set the sky to 
        skyDomeRender.material.mainTextureOffset = new Vector3(-0.25f, 0, 0);
        //game starts at level 1 : Forest
        earth.transform.rotation = Quaternion.Euler(levelLocations[0]);
    }

    public void ChangeLevelSelect(Button b)
    {
        if (rotating == false)
        {
            rotating = true;
            string arrowName = b.gameObject.name;
            nextLevel(arrowName == RIGHTBUTTON);
            //StartCoroutine(ScaleUpAndDown(b));
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

    private void nextLevel(bool arrowGoingRight)
    {
        arrowRight = arrowGoingRight;
        switch (currentLevel)
        {
            case LevelController.LEVEL.FOREST:
                currentLevel = (arrowRight) ? LevelController.LEVEL.DESERT : LevelController.LEVEL.MULTIPLAYER;
                break;
            case LevelController.LEVEL.DESERT:
                currentLevel = (arrowRight) ? LevelController.LEVEL.SNOW : LevelController.LEVEL.FOREST;
                break;
            case LevelController.LEVEL.SNOW:
                currentLevel = (arrowRight) ? LevelController.LEVEL.MULTIPLAYER : LevelController.LEVEL.DESERT;
                break;
            case LevelController.LEVEL.MULTIPLAYER:
                currentLevel =  (arrowRight) ? LevelController.LEVEL.FOREST : LevelController.LEVEL.SNOW;
                break;
        }
        LevelController.currentLevel = currentLevel;
        Debug.Log("Switched to Level : " + currentLevel);
    }
}