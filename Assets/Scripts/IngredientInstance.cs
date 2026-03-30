using UnityEngine;

public class IngrediientInstance : MonoBehaviour
{
    public Ingredient IngredData;

    private SpriteRenderer mRenderer;

    void Awake()
    {
        mRenderer = this.gameObject.AddComponent<SpriteRenderer>();
    }

    void Start()
    {
        ShowSprite();
    }

    public void SetIngredient(Ingredient data) 
    {
        if(data == null) return;
        IngredData = data;
        ShowSprite();
    }

    public void ShowSprite() 
    {
        if (IngredData != null)
        {
            mRenderer.sprite = IngredData.Icon;
        }
    }

    public SpriteRenderer GetRenderer()
    {
        return mRenderer;
    }
}
