using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
   public static ObjectPool instance;

   [SerializeField] private int poolSize = 10;

   private Dictionary<GameObject, Queue<GameObject>> _poolDictionary = 
      new Dictionary<GameObject, Queue<GameObject>>();

   [Header("To Initialize")] [SerializeField]
   private GameObject weaponPickUp;
   [SerializeField]
   private GameObject ammoPickup;

   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
   }

   public void Start()
   {
      InitializeNewPool(weaponPickUp);
      InitializeNewPool(ammoPickup);
   }

   public GameObject GetObject(GameObject prefab)
   {

      if (_poolDictionary.ContainsKey(prefab) == false)
      {
         InitializeNewPool(prefab);  
      }

      if (_poolDictionary[prefab].Count == 0)
      {
         CreateNewObject(prefab);
      }
      
      var objectToGet = _poolDictionary[prefab].Dequeue();
      objectToGet.SetActive(true);
      objectToGet.transform.parent = null;
      
      return objectToGet;
   }

   public void ReturnObject(GameObject objectToReturn, float delay = .001f)
   {
      StartCoroutine(DelayReturn(objectToReturn, delay));
   }

   private IEnumerator DelayReturn(GameObject objectToReturn, float delay)
   {
      yield return new WaitForSeconds(delay);
      
      ReturnToPool(objectToReturn);
   }
   
   private void ReturnToPool(GameObject objectToReturn)
   {
      var pooledObject = objectToReturn.GetComponent<PooledObject>();
      GameObject originPrefab = pooledObject.originalPrefab;

      objectToReturn.SetActive(false);
      objectToReturn.transform.parent = transform;
      _poolDictionary[originPrefab].Enqueue(objectToReturn);
   }

   private void InitializeNewPool(GameObject prefab)
   {
      _poolDictionary[prefab] = new Queue<GameObject>();
      
      for (int i = 0; i < poolSize; i++)
      {
         CreateNewObject(prefab);
      }
   }

   private void CreateNewObject(GameObject prefab)
   {
      GameObject newObject = Instantiate(prefab, transform);
      newObject.AddComponent<PooledObject>().originalPrefab = prefab;
      newObject.SetActive(false);
      _poolDictionary[prefab].Enqueue(newObject);
   }
}
