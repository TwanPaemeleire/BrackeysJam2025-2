using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GodFights.Attacks.SunGod
{
    public class ThrownScythe : MonoBehaviour
    {
        [SerializeField] private float _scytheSpeed;
        [SerializeField] private float _scytheDamage;
        [SerializeField] private float _maxTravelDistance;
        private bool _isReturning = false;
        private Vector3 _positionToArriveAt;
        private Vector3 _startPosition;
        private float _directionMultiplier = 1.0f;

        public Vector3 PositionToArriveAt { set { _positionToArriveAt = value; } }
        public float DirectionMultiplier { set { _directionMultiplier = value; } }

        public UnityEvent OnScytheReachedEndPoint = new UnityEvent();

        private void Start()
        {
            _startPosition = transform.position;
            StartCoroutine(DoScytheThrow());
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PlayerSword>(out PlayerSword sword))
            {
                if (!sword.IsParrying)
                {
                    collision.GetComponent<PlayerHealth>().TakeDamage(_scytheDamage);
                }
                else
                {
                    sword.OnSuccesfullParryExecuted();
                    if(!_isReturning)
                    {
                        StopAllCoroutines();
                        StartCoroutine(ReturnToOwner());
                        _isReturning=true;
                    }
                }
            }
        }

        private IEnumerator DoScytheThrow()
        {
            while (Vector3.Distance(transform.position, _startPosition) < _maxTravelDistance)
            {
                Vector3 newPos = transform.position;
                newPos.x += _scytheSpeed * _directionMultiplier * Time.deltaTime;
                transform.position = newPos;
                yield return null;
            }
            yield return ReturnToOwner();
        }

        private IEnumerator ReturnToOwner()
        {
            _isReturning = true;
            while (Vector3.Distance(transform.position, _positionToArriveAt) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _positionToArriveAt, _scytheSpeed * Time.deltaTime);
                yield return null;
            }
            OnScytheReachedEndPoint.Invoke();
        }
    }
}