using System.Collections;
using UnityEngine;

public class WhipContoller : MonoBehaviour
{
    Animator anim;
    [SerializeField] GameObject whipParticles;
    [SerializeField] float waitTime = 1f;
    [SerializeField] GameObject whipTrailParticle;
    bool whipping = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
        if(whipParticles == null || whipTrailParticle == null)
        {
            Debug.LogError("Error: Missing some Whip Particle GameObject Reference");
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name.ToString());
        if(collision.gameObject.tag == "Enemy")
            SendMessage("DieOnWhipCollide",null, SendMessageOptions.DontRequireReceiver);
       
    }


    private void Update()
    {
        //Whip if Button is pressed and check if whipping done
        if (Input.GetButtonDown("Whip") && !whipping)
        {
            whipping = true;
            Debug.Log("Whip presesed");
            anim.Play("whip");
            whipTrailParticle.SetActive(true);
            StartCoroutine(PlayWhipParticle());
        }
    }

    IEnumerator PlayWhipParticle()
    {
        SoundManager.PlaySound(SoundManager.Sound.whip);
        yield return new WaitForSeconds(waitTime);
        whipParticles.SetActive(true);
        yield return new WaitForSeconds(1f);
        whipping = false;
    }
}
