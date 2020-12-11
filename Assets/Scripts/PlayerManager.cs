using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField]public GameObject player;

    public void BeginPlay()
    {
        DeactivateAllParticle();
        player.GetComponent<InputControls>().enabled = true;
       
    }

    private void DeactivateAllParticle()
    {
        ParticleSystem[] childrenParticleSytems = player.GetComponentsInChildren<ParticleSystem>();
        Debug.Log("Number of Player Particles = " + childrenParticleSytems.Length);
        foreach (ParticleSystem childPS in childrenParticleSytems)
        {
            childPS.gameObject.SetActive(false);
        }
    }


}
