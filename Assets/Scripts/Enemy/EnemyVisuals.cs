using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyWeaponModelType
{
   OneHand, Throw
}

public class EnemyVisuals : MonoBehaviour
{

   [Header("Weapon Model")] 
   [SerializeField] private EnemyWeaponModel[] weaponModels;
   [SerializeField] private EnemyWeaponModelType weaponType;
   public GameObject currentWeaponModel { get; private set; }

   [Header("Color")]
   [SerializeField] private Texture[] colorTextures;
   [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

   private void Start()
   {
      weaponModels = GetComponentsInChildren<EnemyWeaponModel>(true);
      InvokeRepeating(nameof(SetupLook), 0, 1.5f);
   }

   public void SetUpWeaponType(EnemyWeaponModelType weaponType)
      => this.weaponType = weaponType;
   
   public void SetupLook()
   {
      SetupRandomColor();
      SetupRandomWeapon();
   }

   private void SetupRandomWeapon()
   {
      foreach (var model in weaponModels)
      {
         model.gameObject.SetActive(false);
      }

      List<EnemyWeaponModel> filteredWeaponModels = new List<EnemyWeaponModel>();
      foreach (var model in weaponModels)
      {
         if (model.weaponType == weaponType)
         {
            filteredWeaponModels.Add(model);
         }  
      }

      int randomIndex = Random.Range(0, filteredWeaponModels.Count);
      
      currentWeaponModel = filteredWeaponModels[randomIndex].gameObject;
      currentWeaponModel.SetActive(true);
   }
   
   private void SetupRandomColor()
   {
      int randomIndex = Random.Range(0, colorTextures.Length);

      Material newMAt = new Material(skinnedMeshRenderer.material);

      newMAt.mainTexture = colorTextures[randomIndex];
      skinnedMeshRenderer.material = newMAt;
   }
}
