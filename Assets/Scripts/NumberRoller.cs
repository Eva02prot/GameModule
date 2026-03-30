using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class NumberRoller : MonoBehaviour
{
    [Header("依次对应数字 0~9 的 Sprite")]
    [SerializeField] private List<Sprite> numberSprites = new List<Sprite>(10);

    [SerializeField] private SpriteRenderer spriteRenderer;

    private int currentNumber;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// 显示指定数字（0~9）。
    /// </summary>
    public void SetNumber(int number)
    {
        if (!TryGetSprite(number, out Sprite sprite))
        {
            return;
        }

        currentNumber = number;
        ApplySprite(sprite);
    }

    /// <summary>
    /// 切换到下一个数字（9 后回到 0）。
    /// </summary>
    public void SwitchNumbers()
    {
        int next = (currentNumber + 1) % 10;
        SetNumber(next);
    }

    /// <summary>
    /// 随机显示一个数字（0~9）。
    /// </summary>
    public void SetRandomNumber()
    {
        int randomNumber = Random.Range(0, 10);
        SetNumber(randomNumber);
    }

    private bool TryGetSprite(int number, out Sprite sprite)
    {
        sprite = null;

        if (number < 0 || number > 9)
        {
            Debug.LogWarning($"NumberRoller: number {number} 超出范围（0~9）。", this);
            return false;
        }

        if (numberSprites == null || numberSprites.Count <= number || numberSprites[number] == null)
        {
            Debug.LogWarning($"NumberRoller: numberSprites[{number}] 未配置。", this);
            return false;
        }

        sprite = numberSprites[number];
        return true;
    }

    private void ApplySprite(Sprite sprite)
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("NumberRoller: 缺少 SpriteRenderer。", this);
            return;
        }

        spriteRenderer.sprite = sprite;
    }
}
