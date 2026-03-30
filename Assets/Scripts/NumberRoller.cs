using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class NumberRoller : MonoBehaviour
{
    [Header("Digits")]
    [Tooltip("按 0~9 的顺序放入数字图片。")]
    public List<Sprite> NumberSprites = new List<Sprite>(10);

    [Header("Roll")]
    [Min(0.01f)]
    [SerializeField] private float stepDuration = 0.08f;
    [SerializeField] private AnimationCurve rollEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool useUnscaledTime = false;

    [Header("State")]
    [Range(0, 9)]
    [SerializeField] private int currentDigit = 0;

    private SpriteRenderer currentRenderer;
    private SpriteRenderer nextRenderer;
    private float stepHeight;
    private Coroutine rollingRoutine;

    public int CurrentDigit => currentDigit;

    private void Awake()
    {
        EnsureRenderers();
        CacheStepHeight();
        ApplyDigitInstant(currentDigit);
    }

    private void OnValidate()
    {
        currentDigit = Mathf.Clamp(currentDigit, 0, 9);
        if (stepDuration < 0.01f)
        {
            stepDuration = 0.01f;
        }
    }

    /// <summary>
    /// 设置当前数字（0~9）。
    /// </summary>
    public void SetDigit(int targetDigit, bool animate = true)
    {
        targetDigit = Mathf.Clamp(targetDigit, 0, 9);

        if (!animate || !gameObject.activeInHierarchy)
        {
            if (rollingRoutine != null)
            {
                StopCoroutine(rollingRoutine);
                rollingRoutine = null;
            }

            ApplyDigitInstant(targetDigit);
            return;
        }

        if (rollingRoutine != null)
        {
            StopCoroutine(rollingRoutine);
        }

        rollingRoutine = StartCoroutine(RollToDigit(targetDigit));
    }

    /// <summary>
    /// 兼容旧接口：随机切换到一个数字。
    /// </summary>
    public void SwitchNumbers()
    {
        SetDigit(Random.Range(0, 10), true);
    }

    private IEnumerator RollToDigit(int targetDigit)
    {
        EnsureRenderers();
        CacheStepHeight();

        int steps = (targetDigit - currentDigit + 10) % 10;
        if (steps == 0)
        {
            yield break;
        }

        for (int i = 0; i < steps; i++)
        {
            int nextDigitValue = (currentDigit + 1) % 10;
            nextRenderer.sprite = GetSprite(nextDigitValue);

            currentRenderer.transform.localPosition = Vector3.zero;
            nextRenderer.transform.localPosition = new Vector3(0f, -stepHeight, 0f);

            float elapsed = 0f;
            while (elapsed < stepDuration)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / stepDuration);
                float eased = rollEase.Evaluate(t);

                float y = Mathf.Lerp(0f, stepHeight, eased);
                currentRenderer.transform.localPosition = new Vector3(0f, y, 0f);
                nextRenderer.transform.localPosition = new Vector3(0f, y - stepHeight, 0f);
                yield return null;
            }

            currentDigit = nextDigitValue;

            SpriteRenderer temp = currentRenderer;
            currentRenderer = nextRenderer;
            nextRenderer = temp;

            nextRenderer.transform.localPosition = Vector3.zero;
            nextRenderer.sprite = null;
        }

        currentRenderer.transform.localPosition = Vector3.zero;
        rollingRoutine = null;
    }

    private void ApplyDigitInstant(int digit)
    {
        EnsureRenderers();
        currentDigit = Mathf.Clamp(digit, 0, 9);

        currentRenderer.transform.localPosition = Vector3.zero;
        currentRenderer.sprite = GetSprite(currentDigit);

        nextRenderer.transform.localPosition = Vector3.zero;
        nextRenderer.sprite = null;
    }

    private Sprite GetSprite(int digit)
    {
        if (NumberSprites == null || NumberSprites.Count == 0)
        {
            return null;
        }

        int index = Mathf.Clamp(digit, 0, NumberSprites.Count - 1);
        return NumberSprites[index];
    }

    private void EnsureRenderers()
    {
        if (currentRenderer == null)
        {
            currentRenderer = GetComponent<SpriteRenderer>();
        }

        if (nextRenderer != null)
        {
            return;
        }

        Transform child = transform.Find("__NextDigit");
        if (child == null)
        {
            GameObject nextGo = new GameObject("__NextDigit");
            nextGo.transform.SetParent(transform, false);
            child = nextGo.transform;
        }

        nextRenderer = child.GetComponent<SpriteRenderer>();
        if (nextRenderer == null)
        {
            nextRenderer = child.gameObject.AddComponent<SpriteRenderer>();
        }

        nextRenderer.sortingLayerID = currentRenderer.sortingLayerID;
        nextRenderer.sortingOrder = currentRenderer.sortingOrder;
    }

    private void CacheStepHeight()
    {
        Sprite sprite = GetSprite(currentDigit);
        if (sprite != null)
        {
            stepHeight = sprite.bounds.size.y;
            if (stepHeight < 0.01f)
            {
                stepHeight = 1f;
            }
        }
        else
        {
            stepHeight = 1f;
        }
    }
}
