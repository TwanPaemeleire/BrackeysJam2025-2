using System.Collections;
using UnityEngine;

namespace Assets.Scripts.General
{
    public class CameraToArenaMover : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _timeToReachTarget;
        public void MoveCameraToArena()
        {
            StopAllCoroutines();
            StartCoroutine(MoveToArena());
        }

        private IEnumerator MoveToArena()
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = _targetTransform.position;
            float elapsedTime = 0.0f;
            while(elapsedTime < _timeToReachTarget)
            {
                elapsedTime += Time.deltaTime;
                float newX = Mathf.SmoothStep(startPos.x, targetPos.x, elapsedTime / _timeToReachTarget);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                yield return null;
            }
        }
    }
}
