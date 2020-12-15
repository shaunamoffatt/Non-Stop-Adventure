using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] GameObject bloodParticles;
    [SerializeField] float loadDelay = 0.75f;
    [SerializeField]public HealthBar healthbar;
    [SerializeField] int health = 10;
    private bool dead = false;
    private void Start()
    {
        healthbar.SetMaxHealth(health);
    }

    private void OnTriggerEnter(Collider collision)
    {

        //If youve killed the enemy first ignore it
        //Enemy layer 16 and Collectable layer 12 or the pyramids or rocks in the dessert level
        if (collision.gameObject.layer == 16 || collision.gameObject.layer == 12 || dead || collision.gameObject.tag == "collidableRocks")
            return;

        //SendMessage("GameObjectCollide");

        SoundManager.PlaySound(SoundManager.Sound.hurt);
        if (health > 0)
        {
            CauseDamage();
        }
        else
        {
            dead = true;
            Die();
        }
    }

    private void Die()
    {
        //Run Death Sound and make 
        SoundManager.PlaySound(SoundManager.Sound.bossDie);

        //Give the option to Restart Scene or Return to Main Menu
        PlayerManager.levelState = ENDSTATE.LOSE;
        LevelController.paused = true;
    }

    private void CauseDamage()
    {
        Debug.Log("RunDeathSequence");
        bloodParticles.SetActive(true);
        health--;
        healthbar.SetHealth(health);  
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
