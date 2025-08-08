using UnityEngine;

public class PickUpWeapon: Interactable
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private BackUpWeaponModel[] models;
    [SerializeField] private Weapon weapon;

    private bool _oldWeapon;
    private void Start()
    {
        
        if (_oldWeapon == false)
        {
            weapon = new Weapon(weaponData);
        }
        SetUpGameObject();
    }

    public void SetUpPickUpWeapon(Weapon weapon, Transform transform)
    {
        _oldWeapon = true;
        this.weapon = weapon;
        weaponData = weapon.weaponData;

        this.transform.position = transform.position + new Vector3(0 ,.75f, 0);
    }
   
    [ContextMenu("Update Item Model")]
    public void SetUpGameObject()
    {
        gameObject.name = "PickupWeapon - " + weaponData.weaponType.ToString();
        SetUpWeaponModel();
    }

    private void SetUpWeaponModel()
    {
        foreach (var model in models)
        {
            model.gameObject.SetActive(false);
            if (model.weaponType == weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }
    
    public override void Interaction()
    {
        _weaponController.PickupWeapon(weapon);
        ObjectPool.instance.ReturnObject(gameObject);
    }

   
}
