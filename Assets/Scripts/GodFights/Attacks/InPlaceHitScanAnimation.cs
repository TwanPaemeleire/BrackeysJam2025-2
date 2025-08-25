using UnityEngine;

namespace Assets.Scripts.GodFights.Attacks
{
    public class InPlaceHitScanAnimation : BaseGodAttack
    {
        [SerializeField] private string _animationTriggerName = "TestAttack";
        [SerializeField] private Animator _animator;
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
            Debug.Log("HIT SCAN DONE");
        }

    }
}