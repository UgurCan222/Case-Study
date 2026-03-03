using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/*
Dotween kullanýmýna izin verilmediði için lerp tabanlý bir custom tween sistemi yazdým.
Bunu ayrý scriptte yapýyorum ki yumuþak geçiþleri merkezi bir yerden yönetebileyim ve
ayný interpolasyon kodunu farklý scriptlerde tekrar etmeyeyim.
*/
public class CustomTween : MonoBehaviour
{
    private static CustomTween _instance;

    // SCALE TWEEN
    public Coroutine ScaleTo(Transform target, Vector3 targetScale, float dur, Action onComplete = null)
    {
        if (target == null) return null;
        return StartCoroutine(DoScale(target, targetScale, dur, onComplete));
    }

    private IEnumerator DoScale(Transform tr, Vector3 goal, float d, Action done)
    {
        Vector3 start = tr.localScale;
        float timer = 0f;

        if (d <= 0) { tr.localScale = goal; done?.Invoke(); yield break; }

        while (timer < d)
        {
            if (tr == null) yield break;
            timer += Time.deltaTime;
            float step = Mathf.Clamp01(timer / d);
            float curve = 1f - Mathf.Pow(1f - step, 3f);
            tr.localScale = Vector3.Lerp(start, goal, curve);
            yield return null;
        }

        if (tr != null) tr.localScale = goal;
        done?.Invoke();
    }

    // ZIPLAYARAK HAREKET TWEENÝ
    public Coroutine JumpTo(Transform target, Vector3 targetPos, float jumpHeight, float dur, Action onComplete = null)
    {
        if (target == null) return null;
        return StartCoroutine(DoJump(target, targetPos, jumpHeight, dur, onComplete));
    }

    private IEnumerator DoJump(Transform tr, Vector3 goal, float height, float d, Action done)
    {
        Vector3 start = tr.position;
        float timer = 0f;

        if (d <= 0) { tr.position = goal; done?.Invoke(); yield break; }

        while (timer < d)
        {
            if (tr == null) yield break;
            timer += Time.deltaTime;
            float t = timer / d;
            Vector3 currentPos = Vector3.Lerp(start, goal, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * height;
            tr.position = currentPos;
            yield return null;
        }

        if (tr != null) tr.position = goal;
        done?.Invoke();
    }

    // DÜZ HAREKETTWEENÝ
    public Coroutine MoveTo(Transform target, Vector3 targetPos, float dur, Action onComplete = null)
    {
        if (target == null) return null;
        return StartCoroutine(DoMove(target, targetPos, dur, onComplete));
    }

    private IEnumerator DoMove(Transform tr, Vector3 goal, float d, Action done)
    {
        Vector3 start = tr.position;
        float timer = 0f;

        if (d <= 0) { tr.position = goal; done?.Invoke(); yield break; }

        while (timer < d)
        {
            if (tr == null) yield break;
            timer += Time.deltaTime;
            float step = Mathf.Clamp01(timer / d);
            tr.position = Vector3.Lerp(start, goal, step);
            yield return null;
        }

        if (tr != null) tr.position = goal;
        done?.Invoke();
    }

    // ROTATE TWEENÝ
    public Coroutine RotateFromTo(Transform target, Vector3 startRot, Vector3 endRot, float dur, Action onComplete = null)
    {
        if (target == null) return null;
        return StartCoroutine(DoRotateFromTo(target, startRot, endRot, dur, onComplete));
    }

    private IEnumerator DoRotateFromTo(Transform tr, Vector3 start, Vector3 goal, float d, Action done)
    {
        float timer = 0f;

        if (d <= 0) { tr.eulerAngles = goal; done?.Invoke(); yield break; }

        while (timer < d)
        {
            if (tr == null) yield break;
            timer += Time.deltaTime;
            float step = Mathf.Clamp01(timer / d);
            tr.rotation = Quaternion.Lerp(Quaternion.Euler(start), Quaternion.Euler(goal), step);
            yield return null;
        }

        if (tr != null) tr.eulerAngles = goal;
        done?.Invoke();
    }

    // FADE TWEENÝ
    public Coroutine FadeTo(Image target, float targetAlpha, float dur, Action onComplete = null)
    {
        if (target == null) return null;
        return StartCoroutine(DoFade(target, targetAlpha, dur, onComplete));
    }

    private IEnumerator DoFade(Image img, float goalAlpha, float d, Action done)
    {
        Color startColor = img.color;
        float startAlpha = startColor.a;
        float timer = 0f;

        if (d <= 0) { img.color = new Color(startColor.r, startColor.g, startColor.b, goalAlpha); done?.Invoke(); yield break; }

        while (timer < d)
        {
            if (img == null) yield break;
            timer += Time.deltaTime;
            float step = Mathf.Clamp01(timer / d);
            float currentAlpha = Mathf.Lerp(startAlpha, goalAlpha, step);
            img.color = new Color(startColor.r, startColor.g, startColor.b, currentAlpha);
            yield return null;
        }

        if (img != null) img.color = new Color(startColor.r, startColor.g, startColor.b, goalAlpha);
        done?.Invoke();
    }

    // COUNT TWEENÝ
    public Coroutine CountTo(int startValue, int endValue, float dur, Action<int> onUpdate, Action onComplete = null)
    {
        return StartCoroutine(DoCount(startValue, endValue, dur, onUpdate, onComplete));
    }

    private IEnumerator DoCount(int start, int goal, float d, Action<int> onUpdate, Action done)
    {
        float timer = 0f;

        if (d <= 0) { onUpdate?.Invoke(goal); done?.Invoke(); yield break; }

        while (timer < d)
        {
            timer += Time.deltaTime;
            float step = Mathf.Clamp01(timer / d);
            float curve = step * (2f - step);
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(start, goal, curve));
            onUpdate?.Invoke(currentValue);
            yield return null;
        }

        onUpdate?.Invoke(goal);
        done?.Invoke();
    }

    // COLOR TWEENÝ
    public Coroutine ColorTo(Color startColor, Color endColor, float dur, Action<Color> onUpdate, Action onComplete = null)
    {
        return StartCoroutine(DoColor(startColor, endColor, dur, onUpdate, onComplete));
    }

    private IEnumerator DoColor(Color start, Color goal, float d, Action<Color> onUpdate, Action done)
    {
        float timer = 0f;

        if (d <= 0) { onUpdate?.Invoke(goal); done?.Invoke(); yield break; }

        while (timer < d)
        {
            timer += Time.deltaTime;
            float step = Mathf.Clamp01(timer / d);
            onUpdate?.Invoke(Color.Lerp(start, goal, step));
            yield return null;
        }

        onUpdate?.Invoke(goal);
        done?.Invoke();
    }

    public void StopAll()
    {
        StopAllCoroutines();
        Debug.Log("Tweens stopped");
    }
}