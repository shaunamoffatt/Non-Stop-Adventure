using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Button3D : MonoBehaviour
{

    [SerializeField] public float scaleTime = 0.1f;

    private Vector3 startingScale;

    bool spinning = false;

    private readonly RuntimePlatform platform;

    [SerializeField] GameObject earth;

    [SerializeField] protected Vector3 m_to = new Vector3(0.0F, -45.0F, 0.0F);
    [SerializeField] protected float m_frequency = 1.0F;

    protected virtual void Rotate()
    {
        spinning = true;
        Quaternion to = Quaternion.Euler(this.m_to);

        float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * this.m_frequency));
        //spin the earth
        earth.transform.localRotation = Quaternion.Lerp(earth.transform.localRotation, to, lerp);
        spinning = false;
    }

    private void Start()
    {
        startingScale = transform.localScale;
        Debug.Log(" Starting Scale: " + transform.localScale);


    }

    void Update()
    {

        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    checkTouch(Input.GetTouch(0).position);
                }
                
            }
        }
        else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.OSXEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Rotate();
                checkTouch(Input.mousePosition);
            }
        }
    }

    private void checkTouch(Vector3 pos)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit))
            if (hit.transform != null && spinning == false)
            {
                
                Debug.Log("Hit " + hit.transform.gameObject.name + " Scale: " + transform.localScale);
                Debug.Log("Starting Corounting");

                Rotate();
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
}


