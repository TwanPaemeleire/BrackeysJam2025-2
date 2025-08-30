using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks.MoonGoddess
{
    public class MoonProjectile : MonoBehaviour
    {
        [SerializeField] 
        private float _projectileSpeed;
        [SerializeField] 
        private float _projectileDamage;
        [SerializeField]
        private Animator _animator;

        private Vector3 _directionToPlayer;

        private void Awake()
        {
            _animator.SetTrigger("Fly");
        }

        private void Start()
        {
            var playerPos = FightSequenceManager.Instance.PlayerObject.transform.position;
            playerPos.y -= 1.0f;
            _directionToPlayer = (playerPos - transform.position).normalized;

            var angle = Mathf.Atan2(_directionToPlayer.y, _directionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90.0f);
        }

        private void Update()
        {
            transform.position += _projectileSpeed * Time.deltaTime * _directionToPlayer;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PlayerSword>(out PlayerSword sword))
            {
                if (!sword.IsParrying)
                {
                    collision.GetComponent<PlayerHealth>().TakeDamage(_projectileDamage);
                }
                else
                {
                    sword.OnSuccesfullParryExecuted();
                }

                _animator.SetTrigger("Hit");
            }

            if (collision.CompareTag("Ground"))
            {
                _animator.SetTrigger("Hit");
                _projectileSpeed = 0;
            }
        }

        private void OnHitEnded()
        {
            Destroy(gameObject);
        }
    }
}
