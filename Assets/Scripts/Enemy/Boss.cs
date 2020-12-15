using System.Collections;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    SoundManager.Sound soundDie;
    int bossHealth = 2;
    public TMP_Text bossHP;
    bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
        soundDie = SoundManager.Sound.bossDie;

        bossHP.text = "boss_hp: "+ bossHealth.ToString();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!hit)
        {
            //hit = true;
           
            //Play Enemy die sound for pain
            //TODO get more sounds
            SoundManager.PlaySound(soundDie, transform.position);

            bossHealth--;
            if (bossHealth <= 0)
            {
                StartCoroutine(HitBoss());
            }
            else
            {
                bossHP.text = bossHealth.ToString();
            }
        }
    }

    IEnumerator HitBoss()
    {
        //Play death particles
        //deathParticle.SetActive(true);

        //Update Player Score
        PlayerManager.playerScore += 35;

        if (bossHealth <= 0)
        {
            //Stop  Animations
            try
            {
                GetComponent<Animator>().Play("Death");
            }
            catch
            {
                Debug.LogError("No animator on the boss: need to add one");
            }
            //Wait 5 secs before destroying
            yield return new WaitForSecondsRealtime(1f);
            LevelController.paused = true;
            Destroy(gameObject);
        }
       // hit = false;
        yield return new WaitForSecondsRealtime(2f);
    }
}
