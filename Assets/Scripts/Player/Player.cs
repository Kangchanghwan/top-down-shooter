using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControllers controllers;
    public PlayerAim aim { get; private set; }
    public PlayerMovement movement { get; private set; }
    public PlayerWeaponController weapon { get; private set; }

    private void Awake()
    {
        controllers = new PlayerControllers();
        aim = GetComponent<PlayerAim>();
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<PlayerWeaponController>();
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
