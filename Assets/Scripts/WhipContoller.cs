using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipContoller : MonoBehaviour
{
    Animator anim;
    [SerializeField] GameObject whipParticles;
    [SerializeField] float waitTime = 1f;
    [SerializeField] GameObject whipTrailParticle;

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
        //TODO check if whipping done
        if (Input.GetButtonDown("Whip"))
        {
            Debug.Log("Whip presesed");
            anim.Play("whip");
            whipTrailParticle.SetActive(true);
            StartCoroutine(PlayWhipParticle());
            

        }
    }

    IEnumerator PlayWhipParticle()
    {
        yield return new WaitForSeconds(waitTime);
        whipParticles.SetActive(true);
    }
}
