using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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

        private float _smashTimer = 4.0f;
        [SerializeField] 
        private float _smashCooldown = 4.0f;
        [SerializeField] 
        private float _hoverHeight = 5.0f;
        [SerializeField] 
        private float _smashSpeed = 20.0f;
        [SerializeField]
        private float _recoverTime = 3.0f;
        [SerializeField] 
        private float _returnSpeed = 5.0f;

        private float _recoveryTimer = 0.0f;


        [SerializeField] 
        private GameObject _projectilePrefab;

        [SerializeField]
        private Transform _shootSlotTransform;

        private enum BossState
        {
            EllipseMove,
            SmashPrepare,
            SmashAttack,
            Recover,
            Return
        }

        private BossState _state = BossState.EllipseMove;

        private Vector3 _returnPoint;

        public override void StartBossFight()
        {
            base.StartBossFight();

            _floatingAvatarObject.SetActive(true);

            Animator.SetTrigger("Spawn");

            StartCoroutine(nameof(ProcessMovement));
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

        private IEnumerator ProcessMovement()
        {
            while (true)
            {
                switch (_state)
                {
                    case BossState.EllipseMove:
                        DoEllipseMovement();
                        _smashTimer -= Time.deltaTime;

                        if (_smashTimer <= 0f)
                        {
                            _smashTimer = _smashCooldown;
                            StopCoroutine(nameof(StartShooting));
                            InitiateSmash();
                        }
                        break;

                    case BossState.SmashPrepare:
                        FollowPlayerHorizontally();
                        break;

                    case BossState.SmashAttack:
                        SmashDown();
                        break;

                    case BossState.Recover:
                        Recover();
                        break;

                    case BossState.Return:
                        ReturnToEllipse();
                        break;
                }

                yield return null;
            }
        }

        private void DoEllipseMovement()
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
        }

        private IEnumerator StartShooting()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 1.25f));
                ShootProjectile();
            }
        }

        private void ShootProjectile()
        {
            Instantiate(_projectilePrefab, _shootSlotTransform.position, Quaternion.identity);
        }

        private void InitiateSmash()
        {
            _state = BossState.SmashPrepare;
            Animator.SetTrigger("Charge");
            _returnPoint = transform.position;
        }

        private void FollowPlayerHorizontally()
        {
            var player = FightSequenceManager.Instance.PlayerObject.transform;

            var targetPos = new Vector3(player.position.x, player.position.y + _hoverHeight, 0);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _returnSpeed * Time.deltaTime);

            if (!(Vector3.Distance(transform.position, targetPos) < 0.1f))
            {
                return;
            }

            Animator.SetTrigger("Smash");
            _state = BossState.SmashAttack;
        }

        private void SmashDown()
        {
            transform.position += Vector3.down * _smashSpeed * Time.deltaTime;

            if (transform.position.y <= -2.0f)
            {
                _state = BossState.Recover;
                Animator.SetTrigger("Charge");
            }
        }

        private void Recover()
        {
            _recoveryTimer += Time.deltaTime;

            if (!(_recoveryTimer >= _recoverTime))
            {
                return;
            }

            _recoveryTimer = 0;
            _state = BossState.Return;
            Animator.SetTrigger("Move");
        }

        private void ReturnToEllipse()
        {
            transform.position = Vector3.MoveTowards(transform.position, _returnPoint, _returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _returnPoint) < 0.1f)
            {
                _state = BossState.EllipseMove;
                StartCoroutine(nameof(StartShooting));
            }
        }
    }
}