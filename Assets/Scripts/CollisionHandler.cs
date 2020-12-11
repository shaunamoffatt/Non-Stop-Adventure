using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] GameObject bloodParticles;
    [SerializeField] float loadDelay = 0.75f;
    [SerializeField]public HealthBar healthbar;
    [SerializeField] int health = 10;

    private void Start()
    {
        healthbar.SetMaxHealth(health);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name.ToString());
        //SendMessage("GameObjectCollide");
   
        RunDeathSequence();

        //Invoke("ReloadScene", loadDelay);
    }



    private void OnCollisionEnter(Collision collision)
    {


    }

    private void RunDeathSequence()
    {
        Debug.Log("RunDeathSequence");
        bloodParticles.SetActive(true);
        healthbar.SetHealth(health--);  
    }


    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
