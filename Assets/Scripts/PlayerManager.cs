﻿using System.Collections;
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
        player.GetComponent<InputControls>().enabled = true;
    }
}
