using UnityEngine;

public class PickUpWeapon: Interactable
{
    private PlayerWeaponController _weaponController;
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
        UpdateGameObject();
    }

    public void SetUpPickUpWeapon(Weapon weapon, Transform transform)
    {
        _oldWeapon = true;
        this.weapon = weapon;
        weaponData = weapon.weaponData;

        this.transform.position = transform.position + new Vector3(0 ,.75f, 0);
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (_weaponController == null)
        {
            _weaponController = other.GetComponent<PlayerWeaponController>();
        }
        
    }
    [ContextMenu("Update Item Model")]
    public void UpdateGameObject()
    {
        gameObject.name = "PickupWeapon - " + weaponData.weaponType.ToString();
        UpdateItemModel();
    }

    public void UpdateItemModel()
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
