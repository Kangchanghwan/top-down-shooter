using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce;

    private Rigidbody _rigidbody;
    private TrailRenderer _trailRenderer;
    private MeshRenderer _meshRenderer;
    private BoxCollider _boxCollider;

    [SerializeField] private GameObject bulletImpactFx;

    private Vector3 _startPosition;
    private float _flyDistance;
    private bool _bulletDisabled;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        FadeTrailNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (_trailRenderer.time < 0)
        {
            ObjectPool.instance.ReturnObject(gameObject);
        }
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(
                _startPosition, transform.position) > _flyDistance &&
            _bulletDisabled == false)
        {
            _boxCollider.enabled = false;
            _meshRenderer.enabled = false;
            _bulletDisabled = true;
        }
    }

    private void FadeTrailNeeded()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance - 1.5f)
        {
            _trailRenderer.time -= 2f * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        CreateImpactFx(other);
        ObjectPool.instance.ReturnObject(gameObject);
        
        Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
        EnemyShield enemyShield = other.gameObject.GetComponentInParent<EnemyShield>();

        if (enemyShield != null)
        {
            enemyShield.ReduceDurability();
            return;
        }

        if (enemy != null)
        {
            Vector3 force = _rigidbody.linearVelocity.normalized * impactForce;
            Rigidbody hitRb = other.collider.attachedRigidbody;
            
            enemy.GetHit();
            enemy.DeathImpact(force, other.contacts[0].point, hitRb);
        }

    }

    public void BulletSetup(float flyDistance, float impactForce)
    {
        this.impactForce = impactForce;

        _bulletDisabled = false;
        _boxCollider.enabled = true;
        _meshRenderer.enabled = true;

        _trailRenderer.time = .25f;
        _startPosition = transform.position;
        _flyDistance = flyDistance + .5f; // 이 메직넘버는 UpdateAimVisuals()함수에서 레이저 팁의 길이를 더했습니다.
    }


    private void CreateImpactFx(Collision other)
    {
        if (other.contacts.Length > 0)
        {
            ContactPoint contact = other.contacts[0];

            GameObject newImpactFx = ObjectPool.instance.GetObject(bulletImpactFx);
            newImpactFx.transform.position = contact.point;
            newImpactFx.transform.rotation = Quaternion.LookRotation(contact.normal);

            ObjectPool.instance.ReturnObject(newImpactFx, 1f);
        }
    }
}
