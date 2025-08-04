using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControllers controllers;

    private void Awake()
    {
        controllers = new PlayerControllers();
    }
    
    private void OnEnable()
    {
        controllers.Enable();
    }

    private void OnDisable()
    {
        controllers.Disable();
    }

}
