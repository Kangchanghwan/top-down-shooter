using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum EnemyWeaponModelType
{
   OneHand, Throw, Unarmed
}

public class EnemyVisuals : MonoBehaviour
{

   [FormerlySerializedAs("corrupGameObjects")] [Header("Corruption Visuals")] [SerializeField]
   private GameObject[] corruptionGameObjects;
   [SerializeField] private int corruptionAmount;
   
   
   [Header("Weapon Model")] 
   [SerializeField] private EnemyWeaponModel[] weaponModels;
   [SerializeField] private EnemyWeaponModelType weaponType;
   public GameObject currentWeaponModel { get; private set; }

   [Header("Color")]
   [SerializeField] private Texture[] colorTextures;
   [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

   private void Awake()
   {
      weaponModels = GetComponentsInChildren<EnemyWeaponModel>(true);
      CollectCorruptionCrystals();
      
   }

   private void Start()
   {
  
   }

   private void CollectCorruptionCrystals()
   {
      EnemyCorruptionCrystal[] crystalComponents = GetComponentsInChildren<EnemyCorruptionCrystal>(true);
      corruptionGameObjects = new GameObject[crystalComponents.Length];
      for (var i = 0; i < crystalComponents.Length; i++)
      {
         corruptionGameObjects[i] = crystalComponents[i].gameObject;
      }
   }

   public void SetUpWeaponType(EnemyWeaponModelType weaponType)
      => this.weaponType = weaponType;
   
   public void SetupLook()
   {
      SetupRandomColor();
      SetupRandomWeapon();
      SetupRandomCorruption();
   }

   private void SetupRandomCorruption()
   {
      List<int> avalibleIndexs = new List<int>();
      
      for (int i = 0; i < corruptionGameObjects.Length; i++)
      {
         avalibleIndexs.Add(i);
         corruptionGameObjects[i].SetActive(false);
      }

      for (int i = 0; i < corruptionAmount; i++)
      {
         if (avalibleIndexs.Count == 0)
         {
            break;
         }
         
         int randomIndex = Random.Range(0, avalibleIndexs.Count);
         corruptionGameObjects[randomIndex].SetActive(true);
         avalibleIndexs.RemoveAt(randomIndex);
      }
      
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

      OverrideAnimatorControllerIfCan();
   }

   private void OverrideAnimatorControllerIfCan()
   {
      AnimatorOverrideController overrideController =
         currentWeaponModel.GetComponent<EnemyWeaponModel>().overrideController;

      if (overrideController != null)
      {
         GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
      }
   }

   private void SetupRandomColor()
   {
      int randomIndex = Random.Range(0, colorTextures.Length);

      Material newMAt = new Material(skinnedMeshRenderer.material);

      newMAt.mainTexture = colorTextures[randomIndex];
      skinnedMeshRenderer.material = newMAt;
   }
}
