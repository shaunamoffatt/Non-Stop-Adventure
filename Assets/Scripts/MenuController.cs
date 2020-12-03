using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject earth;
    [SerializeField] float scaleTime = 0.1f;
    [SerializeField] float durationOfSpin = 1f;
    //Size/Scale of the buttons
    [SerializeField] private Vector3 startingScale = new Vector3(40, 40, 23);

    bool rotating = false;
    //Spin Speed of earth
    //public float speed = 0.1f;
    string arrow;
    private LEVEL currentLevel = LEVEL.FORREST;
    private readonly RuntimePlatform platform;

    //Set a const for Right arrow so that a chack can be done on it to see if its pressed;
    const string RIGHTARROW = "RightArrow";

    protected Vector3[] levelLocations = {
        new Vector3(0, 0, -45),//Level 1 location
        new Vector3(-45,-20,95),//LEvel 2 location
        new Vector3(160,0,0),//Level 3 location
        new Vector3(-20,80,0)//Level 4 location
    };

    enum LEVEL
    {
        FORREST,
        DESERT,
        SNOW,
        MULTIPLAYER
    }

    private void Start()
    {
        if (earth == null)
        {
            earth = GameObject.Find("earth");
        }
        //have the gameobject set to scale just in case one hasnt been set
        startingScale = transform.localScale;
    }

    void Update()
    {
        if (rotating == true)
            return;

        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    checkTouch(Input.GetTouch(0).position);
                    CheckCurrentLevelLocation();
                    RotateEarth(levelLocations[(int)currentLevel]);
                }

            }
        }
        else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.OSXEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                checkTouch(Input.mousePosition);
                CheckCurrentLevelLocation();
                RotateEarth(levelLocations[(int)currentLevel]);
            }
        }
    }

    void CheckCurrentLevelLocation()
    {
        Debug.Log("rotating true");
        nextLevel();
        rotating = true;

    }

    protected virtual void RotateEarth(Vector3 rotateTo)
    {
        if (rotating)
        {
            StartCoroutine(RotateOverTime(Quaternion.Euler(rotateTo), durationOfSpin));
        }
    }

    IEnumerator RotateOverTime(Quaternion finalRotation, float duration)
    {
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;
                earth.transform.rotation = Quaternion.Slerp(earth.transform.rotation, finalRotation, progress);
                yield return null;
            }
        }
        earth.transform.rotation = finalRotation;
        rotating = false;
    }

    private void checkTouch(Vector3 pos)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit))
            if (hit.transform != null )
            {
                Debug.Log("Hit " + hit.transform.gameObject.name + " Scale: " + transform.localScale);
                arrow = hit.transform.gameObject.name;
                StartCoroutine(ScaleUpAndDown(hit.transform, scaleTime));
            }
    }

    IEnumerator ScaleUpAndDown(Transform transform,  float duration)
    {
        Vector3 myScale = transform.localScale * 1.1f;
        for (float time = 0; time < duration * 2; time += Time.deltaTime)
        {
            float progress = Mathf.PingPong(time, duration) / duration;
            transform.localScale = Vector3.Lerp(startingScale, myScale, progress);
            yield return null;
        }
        //reset scale
        transform.localScale = startingScale; 
    }

    public void nextLevel()
    {
        switch (currentLevel)
        {
            case LEVEL.FORREST:
                currentLevel = (arrow == RIGHTARROW) ? LEVEL.DESERT : LEVEL.MULTIPLAYER;
                break;
            case LEVEL.DESERT:
                currentLevel = (arrow == RIGHTARROW) ? LEVEL.SNOW : LEVEL.FORREST;
                break;
            case LEVEL.SNOW:
                currentLevel = (arrow == RIGHTARROW) ? LEVEL.MULTIPLAYER : LEVEL.DESERT;
                break;
            case LEVEL.MULTIPLAYER:
                currentLevel =  (arrow == RIGHTARROW) ? LEVEL.FORREST : LEVEL.SNOW;
                break;
        }

        Debug.Log("Switched to Level : " + currentLevel);
    }
}