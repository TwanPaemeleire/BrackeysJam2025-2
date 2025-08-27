using Assets.Scripts.SharedLogic;
using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks
{
    public class InPlaceHitScanAnimation : BaseGodAttack
    {
        [SerializeField] private string _animationTriggerName = "TestAttack";
        [SerializeField] private float _damage = 1.0f;
        [SerializeField] private HitScanObject _hitScanObject;
        public override void InitializeAttack()
        {
            RegisterAction(nameof(DoHitScan), DoHitScan);
            RegisterAction(nameof(AnimationFinished), AnimationFinished);
        }

        public override void StartAttack()
        {
            God.Animator.SetTrigger(_animationTriggerName);
        }

        public override void StopAttack()
        {
            
        }

        public void DoHitScan()
        {
            _hitScanObject.ExecuteHitScan(_damage);
            Debug.Log("HIT SCAN DONE");
        }

        public void AnimationFinished()
        {
            OnAttackFinished.Invoke();
        }
    }
}