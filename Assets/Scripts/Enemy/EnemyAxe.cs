using System;
using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
   [SerializeField] public Rigidbody rb;
   [SerializeField] private Transform axeVisual;
   [SerializeField] private GameObject impactFx;
   
   private float _flySpeed;
   private Transform _player;
   private float _rotationSpeed;
   private float _timer = 1;
   private Vector3 _direction;

   public void AxeSetUp(float flySpeed, Transform player,float timer)
   {
      _rotationSpeed = 1600;
      _flySpeed = flySpeed;
      _player = player;
      _timer = timer;
   }

   private void Update()
   {
      axeVisual.Rotate(Vector3.right * _rotationSpeed * Time.deltaTime);

      _timer -= Time.deltaTime;

      if (_timer > 0)
      {
         _direction = _player.position + Vector3.up - transform.position;
      }
      rb.linearVelocity = _direction.normalized * _flySpeed;
      transform.forward = rb.linearVelocity;
   }

   private void OnTriggerEnter(Collider other)
   {
      Bullet bullet = other.GetComponent<Bullet>();
      Player player = other.GetComponent<Player>();

      if (bullet != null || player != null)
      {
         GameObject newFx = ObjectPool.instance.GetObject(impactFx);
         newFx.transform.position = transform.position;
         ObjectPool.instance.ReturnObject(gameObject);
         ObjectPool.instance.ReturnObject(newFx, 1f);
      }
   }
}
