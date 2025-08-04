using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{

    private Player _player;
    
    void Start()
    {
        _player = GetComponent<Player>();
        _player.controllers.Character.Fire.performed += _ => Shoot();
    }


    private void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fire");
        print("Shoot!!!");
    }

}
