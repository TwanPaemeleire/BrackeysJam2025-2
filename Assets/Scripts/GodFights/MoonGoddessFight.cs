using System.Collections;
using UnityEngine;

namespace Assets.Scripts.GodFights
{
    public class MoonGoddessFight : BaseGodFight
    {
        [SerializeField]
        private GameObject _floatingAvatarObject;

        [SerializeField] 
        private Transform _centerPoint;
        [SerializeField] 
        private float _radiusX = 6.0f;      
        [SerializeField] 
        private float _radiusY = 2.0f;      
        [SerializeField] 
        private float _speed = 0.5f;

        private float _angle;

        public override void StartBossFight()
        {
            base.StartBossFight();

            _floatingAvatarObject.SetActive(true);

            Animator.SetTrigger("Spawn");

            StartCoroutine(nameof(StartEllipseMovement));
            StartCoroutine(nameof(StartShooting));
        }

        public override void RestartBossFight()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDeathInternal()
        {
            _floatingAvatarObject.SetActive(false);
            OnDeath.Invoke();
        }

        public void OnSpawnFinished()
        {
            Animator.SetTrigger("Move");    
        }

        private IEnumerator StartEllipseMovement()
        {
            while (true)
            {
                _angle += Time.deltaTime * _speed;

                var x = Mathf.Cos(_angle) * _radiusX;
                var y = Mathf.Sin(_angle) * _radiusY;

                var pos = _floatingAvatarObject.transform.position;
                var newPos = _centerPoint.position + new Vector3(x, y, 0);

                var dir = newPos - pos;

                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    if (dir.x > 0)
                    {
                        transform.localScale = new Vector3(1, 1, 1);
                    }
                    else if (dir.x < 0)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                }

                transform.position = newPos;

                yield return null;
            }
        }

        private IEnumerator StartShooting()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(2f, 5f));
                ShootProjectile();
            }
        }

        private void ShootProjectile()
        {

        }
    }
}