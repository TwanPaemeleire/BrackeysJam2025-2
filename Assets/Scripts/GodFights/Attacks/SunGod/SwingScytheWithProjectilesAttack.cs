using Assets.Scripts.GodFights;
using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks.SunGod
{
    public class SwingScytheWithProjectilesAttack : BaseGodAttack
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _attackDuration;
        [SerializeField] private Transform _shootPoint;
        public override void InitializeAttack()
        {
            RegisterAction(nameof(ShootProjectile), ShootProjectile);
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
            var projectileObj = Instantiate(_projectilePrefab, _shootPoint);
            SunGodProjectile projectile = projectileObj.GetComponent<SunGodProjectile>();
            var directionToPlayer = (FightSequenceManager.Instance.PlayerObject.transform.position - _shootPoint.transform.position).normalized;
            projectile.Initialize(directionToPlayer, transform);
        }
    }
}