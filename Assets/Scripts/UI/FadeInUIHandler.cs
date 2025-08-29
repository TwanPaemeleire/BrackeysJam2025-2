using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class FadeInUIHandler : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeInTime;
    public void StartFadingIn()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Fade(0.0f, 1.0f));
    }

    public void StartFadingOut()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(1.0f, 0.0f));
    }

    private IEnumerator Fade(float start, float end)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < _fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.SmoothStep(start, end, elapsedTime / _fadeInTime);
            yield return null;
        }
        if(end < 0.1f) gameObject.SetActive(false);
    }
}