using Assets.Scripts.General;
using Assets.Scripts.GodFights;
using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks.SunGod
{
    public class ThrowScytheAttack : BaseGodAttack
    {
        [SerializeField] private GameObject _scythePrefab;
        [SerializeField] private Transform _scytheSpawnPoint;
        [SerializeField] private AudioClip _scytheThrowAudioClip;
        [SerializeField] private float _scytheThrowAudioVolume;
        private GameObject _scythe;

        public override void InitializeAttack()
        {
            RegisterAction(nameof(ThrowScythe), ThrowScythe);
        }

        private void ThrowScythe()
        {
            _scythe = Instantiate(_scythePrefab, _scytheSpawnPoint);
            ThrownScythe thrownScythe = _scythe.GetComponent<ThrownScythe>();
            thrownScythe.PositionToArriveAt = _scytheSpawnPoint.position;
            thrownScythe.OnScytheReachedEndPoint.AddListener(OnScytheReachedEndPoint);
            thrownScythe.DirectionMultiplier = God.CurrentDirectionMultiplier;
        }

        public override void StartAttack()
        {
            SoundManager.Instance.PlaySFX(_scytheThrowAudioClip, _scytheThrowAudioVolume);
            God.Animator.SetTrigger("ThrowScythe");
        }

        private void OnScytheReachedEndPoint()
        {
            Destroy(_scythe);
            _scythe = null;
            OnAttackFinished.Invoke();
        }

        public override void StopAttack()
        {
            if (_scythe)
            {
                Destroy(_scythe); 
            }
        }
    }
}