using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class DecisionBossButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Vector3 _maxScale = new Vector3(1.2f, 1.2f, 1.2f);
        [SerializeField] private float _timeToReachTargetScale = 0.5f;
        [SerializeField] private string _bossName; // Replace with enum in future

        private Coroutine _scaleCoroutine;

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartScaleCoroutine(_maxScale);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartScaleCoroutine(Vector3.one);
        }

        void StartScaleCoroutine(Vector3 targetScale)
        {
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }
            _scaleCoroutine = StartCoroutine(ChangeButtonScale(targetScale));
        }

        private IEnumerator ChangeButtonScale(Vector3 targetScale)
        {
            Vector3 initialScale = transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < _timeToReachTargetScale)
            {
                float newScale = Mathf.SmoothStep(initialScale.x, targetScale.x, elapsedTime / _timeToReachTargetScale);
                transform.localScale = new Vector3(newScale, newScale, 1.0f);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            transform.localScale = targetScale;
        }
    }
}