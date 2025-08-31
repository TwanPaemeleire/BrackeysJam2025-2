using Assets.Scripts.General;
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

        [SerializeField] private AudioClip _shootAudioClip;
        [SerializeField] private AudioClip _groundHitAudioClip;
        [SerializeField] private float _shootAudioClipVolume = 0.05f;
        [SerializeField] private float _groundHitAudioClipVolume = 0.05f;

        private Rigidbody2D _rigidBody;
        private Vector2 _directionToPlayer;

        private void Awake()
        {
            _animator.SetTrigger("Fly");
        }

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();

            var sword = FightSequenceManager.Instance.PlayerObject.GetComponent<PlayerSword>();

            _directionToPlayer = (sword.PlayerCenterPoint.position - transform.position).normalized;

            var angle = Mathf.Atan2(_directionToPlayer.y, _directionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90.0f);

            SoundManager.Instance.PlaySFX(_shootAudioClip, _shootAudioClipVolume);
        }

        private void Update()
        {
            Vector2 newPosition = _rigidBody.position + _directionToPlayer * _projectileSpeed * Time.fixedDeltaTime;
            _rigidBody.MovePosition(newPosition);
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
                GetComponent<BoxCollider2D>().enabled = false;
                _animator.SetTrigger("Hit");
            }

            if (collision.CompareTag("Ground"))
            {
                GetComponent<BoxCollider2D>().enabled = false;
                SoundManager.Instance.PlaySFX(_groundHitAudioClip, _groundHitAudioClipVolume);
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
