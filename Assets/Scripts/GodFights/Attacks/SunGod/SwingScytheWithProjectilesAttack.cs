using Assets.Scripts.GodFights;
using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks.SunGod
{
    public class SwingScytheWithProjectilesAttack : BaseGodAttack
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _attackDuration;
        [SerializeField] private float _minShootDelay;
        [SerializeField] private float _maxShootDelay;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private Transform _centerPosition;

        private bool _canShoot = false;

        private Transform _playerCenterTransform;
        public override void InitializeAttack()
        {
            RegisterAction(nameof(ShootProjectile), ShootProjectile);
            _playerCenterTransform = FightSequenceManager.Instance.PlayerObject.GetComponent<PlayerSword>().PlayerCenterPoint;
        }

        public override void StartAttack()
        {
            God.GetComponent<Animator>().SetTrigger("SpinScythe");
            StartCoroutine(ShootingCoroutine());
            Invoke(nameof(FinishAttack), _attackDuration);
        }

        public override void StopAttack()
        {
            StopAllCoroutines();
        }

        public void ShootProjectile()
        {
            if (!_canShoot) return;
            _canShoot = false;
            var projectileObj = Instantiate(_projectilePrefab);
            projectileObj.transform.position = _shootPoint.position;
            SunGodProjectile projectile = projectileObj.GetComponent<SunGodProjectile>();
            var directionToPlayer = (_playerCenterTransform.position - _shootPoint.transform.position).normalized;
            projectile.Initialize(directionToPlayer, _centerPosition);
        }

        private IEnumerator ShootingCoroutine()
        {
            float randomDelay = Random.Range(_minShootDelay, _maxShootDelay);
            while(true)
            {
                yield return new WaitForSeconds(randomDelay);
                _canShoot = true;
                randomDelay = Random.Range(_minShootDelay, _maxShootDelay); ;
            }
        }

        private void FinishAttack()
        {
            StopAllCoroutines();
            OnAttackFinished.Invoke();
        }
    }
}