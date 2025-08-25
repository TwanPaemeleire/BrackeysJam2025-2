using Assets.Scripts.GodFights;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.SharedLogic
{
    public class HitScanObject : MonoBehaviour
    {
        [SerializeField] private float _range = 0.2f;
        [SerializeField] private LayerMask _layerMask;
        public void ExecuteHitScan(float damage)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _range, _layerMask);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                }
                else if (hit.TryGetComponent<GodHealth>(out GodHealth godHealth))
                {
                    godHealth.TakeDamage(damage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}