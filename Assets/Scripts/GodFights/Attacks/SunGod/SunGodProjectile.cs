using Assets.Scripts.GodFights;
using Assets.Scripts.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks.SunGod
{
    public class SunGodProjectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 2.0f;
        [SerializeField] private float _damage = 1.0f;
        [SerializeField] private float _speedIfParried = 4.0f;
        [SerializeField] private float _damageIfParried = 2.0f;

        private bool _hasBeenParried = false;
        private Vector3 _directionToGod;

        private Vector3 _shootDirection;
        public Vector3 ShootDirection { set { _shootDirection = value; } }
        private Transform _godPointToAimAtIfParried;
        public Transform GodPointToAimAtIfParried { set { _godPointToAimAtIfParried = value; } }

        public void Initialize(Vector3 shootDirection, Transform godPointToAimAt)
        {
            _shootDirection = shootDirection;
            _godPointToAimAtIfParried = godPointToAimAt;
            float angle = Mathf.Atan2(_shootDirection.y, _shootDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void Update()
        {
            if (!_hasBeenParried)
            {
                transform.position += _shootDirection * _speed * Time.deltaTime;
            }
            else
            {
                transform.position += _directionToGod * _speed * Time.deltaTime;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hasBeenParried)
            {
                if (collision.TryGetComponent<GodHealth>(out GodHealth health))
                {
                    health.TakeDamage(_damage);
                    Destroy(gameObject);
                }
            }

            if (collision.TryGetComponent<PlayerSword>(out PlayerSword sword))
            {
                if (!sword.IsParrying)
                {
                    if (collision.GetComponent<PlayerHealth>().TakeDamage(_damage))
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    sword.OnSuccesfullParryExecuted();
                    _hasBeenParried = true;
                    _directionToGod = (_godPointToAimAtIfParried.position - transform.position).normalized;
                    float angle = Mathf.Atan2(_directionToGod.y, _directionToGod.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }
            // Check for ground collision here after work in game scene is done and pushed, on ground collision, destroy
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}