using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] GameObject bloodParticles;
    [SerializeField] float loadDelay = 0.75f;

    private const string terrainName = "ForestTerrain";
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name.ToString());
        //SendMessage("GameObjectCollide");
        if (collision.gameObject.name == terrainName)
        {
                return;
        }
        RunDeathSequence();

        Invoke("ReloadScene", loadDelay);
    }

    private void OnCollisionEnter(Collision collision)
    {


    }

    private void RunDeathSequence()
    {
        bloodParticles.SetActive(true);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
