using UnityEngine;

namespace Assets.Scripts.GodFights
{
    public class MoonGoddessFight : BaseGodFight
    {
        [SerializeField]
        private GameObject _floatingAvatarObject;

        public override void StartBossFight()
        {
            base.StartBossFight();

            _floatingAvatarObject.SetActive(true);

            Animator.SetTrigger("Spawn");
        }

        public override void RestartBossFight()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDeathInternal()
        {
            OnDeath.Invoke();
        }

        public void OnSpawnFinished()
        {
            Animator.SetTrigger("Move");    
        }
    }
}