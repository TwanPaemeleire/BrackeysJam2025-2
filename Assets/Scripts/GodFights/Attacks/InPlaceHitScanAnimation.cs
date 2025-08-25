using Assets.Scripts.SharedLogic;
using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks
{
    public class InPlaceHitScanAnimation : BaseGodAttack
    {
        [SerializeField] private string _animationTriggerName = "TestAttack";
        [SerializeField] private float _damage = 1.0f;
        [SerializeField] private Animator _animator;
        [SerializeField] private HitScanObject _hitScanObject;
        public override void InitializeAttack()
        {
            RegisterAction(nameof(DoHitScan), DoHitScan);
        }

        public override void StartAttack()
        {
            _animator.SetTrigger(_animationTriggerName);
        }

        public override void StopAttack()
        {
            throw new System.NotImplementedException();
        }

        public void DoHitScan()
        {
            _hitScanObject.ExecuteHitScan(_damage);
            Debug.Log("HIT SCAN DONE");
        }
    }
}