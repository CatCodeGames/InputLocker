using UnityEngine;
using System.Collections;

public sealed class InteractionObject : MonoBehaviour
{
    private Coroutine _coroutine;

    public void Play()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Effect());
    }

    private IEnumerator Effect()
    {
        Vector3 start = Vector3.one;
        Vector3 end = Vector3.one * 1.25f;

        float duration = .5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }

        t = duration - t;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(end, start, t / duration);
            yield return null;
        }

        transform.localScale = start;
    }
}
