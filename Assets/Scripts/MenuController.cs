using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private LEVEL currentLevel = LEVEL.FOREST;
    private readonly RuntimePlatform platform;

    protected Vector3[] levelLocations = {
        new Vector3(-200, 192, -272),//Level 1 Forest location
        new Vector3(-140,70,-140),//Level 2 Desert location
        new Vector3(-33,-20,95),//LEvel 2 location
        new Vector3(160,0,0)//Level 4 multi location
    };


    private enum LEVEL
    {
        FOREST,
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
        //game starts at level 1 : Forest
        earth.transform.rotation = Quaternion.Euler(levelLocations[0]);
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
                   // checkTouch(Input.GetTouch(0).position);
                   // ChangeLevel();
                   // RotateEarth(levelLocations[(int)currentLevel]);
                }

            }
        }
        else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.OSXEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //checkTouch(Input.mousePosition);
               // ChangeLevel();
              //  RotateEarth(levelLocations[(int)currentLevel]);
            }
        }
    }

    public void ChangeLevel(bool arrowRight)
    {
        nextLevel(arrowRight);
        rotating = true;
        RotateEarth(levelLocations[(int)currentLevel]);
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

    public void ScaleButton(Transform transform)
    {
        StartCoroutine(ScaleUpAndDown(transform, scaleTime));
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

    private void nextLevel(bool arrowRight)
    {
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

    public void LoadScene() {

        SceneManager.LoadScene((int)currentLevel);
    }
}