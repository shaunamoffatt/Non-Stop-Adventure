using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Collectable : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 2f;
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 1f;

    // Position
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    private bool start = false;
    private static int count = 0;
    void Start()
    {
        // Store the starting position & rotation of the object
        posOffset = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager.collectableCount++;
        SoundManager.PlaySound(SoundManager.Sound.collect);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            // Rotate around Y
            transform.Rotate(new Vector3(0, rotateSpeed, 0), Space.World);

            // Bobble up/down
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
            transform.position = tempPos;
        }
        else
        {
            // Wait a few milliseconds so they start moving at different times
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(Random.Range(0,3.5f));
        start = true;
    }
}
