using Assets.Scripts.GodFights;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks.SunGod
{
    public class SwingScytheWithProjectilesAttack : BaseGodAttack
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _attackDuration;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private Transform _centerPosition;

        private Transform _playerCenterTransform;
        public override void InitializeAttack()
        {
            RegisterAction(nameof(ShootProjectile), ShootProjectile);
            _playerCenterTransform = FightSequenceManager.Instance.PlayerObject.GetComponent<PlayerSword>().PlayerCenterPoint;
        }

        public override void StartAttack()
        {
            God.GetComponent<Animator>().SetTrigger("SpinScythe");
        }

        public override void StopAttack()
        {
            
        }

        public void ShootProjectile()
        {
            var projectileObj = Instantiate(_projectilePrefab);
            projectileObj.transform.position = _shootPoint.position;
            SunGodProjectile projectile = projectileObj.GetComponent<SunGodProjectile>();
            var directionToPlayer = (_playerCenterTransform.position - _shootPoint.transform.position).normalized;
            projectile.Initialize(directionToPlayer, _centerPosition);
        }
    }
}