using UnityEngine;

public class Bullet : MonoBehaviour
{
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
            ObjectPool.instance.ReturnBullet(gameObject);
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
        ObjectPool.instance.ReturnBullet(gameObject);
    }
   
    public void BulletSetup(float flyDistance)
    {
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

            GameObject newImpactFx =
                Instantiate(bulletImpactFx, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(newImpactFx, 1f);
        }
    }
}
