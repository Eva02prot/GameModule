using UnityEngine;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

public class IngredientContainer : MonoBehaviour
{
    public int IngredientID = -1;
    public StorageManager Storage = null;

    public int ColumnCount = 4;
    public float PaddingX = 0.2f;
    public float PaddingY = 0.2f;
    public bool CenterAlign = false;


    private Ingredient ingredient = null;

    void Start()
    {
        var count = Storage.GetIngredientCountByID(IngredientID);
        ingredient = Storage.GetIngredientDataByID(IngredientID);

        Layout(count);
    }


    public void Layout(int count)
    {
        if (ingredient == null) return;


        for (int i = 0; i < count; i++)
        {
            GameObject foodInstance = new GameObject();
            var instance = foodInstance.AddComponent<IngrediientInstance>();
            instance.SetIngredient(ingredient);

            foodInstance.name = ingredient.name;
            foodInstance.transform.SetParent(this.transform);

            SpriteRenderer firstRenderer = instance.GetRenderer();
            if (firstRenderer == null || firstRenderer.sprite == null) return;

            Vector2 spriteSize = firstRenderer.bounds.size;
            float cellWidth = spriteSize.x + PaddingX;
            float cellHeight = spriteSize.y + PaddingY;

            float startX = 0f;
            if (CenterAlign)
            {
                int currentRowCount = Mathf.Min(ColumnCount, count);
                float totalWidth = (currentRowCount - 1) * cellWidth;
                startX = -totalWidth * 0.5f;
            }

            int row = i / ColumnCount;
            int col = i % ColumnCount;

            float x = startX + col * cellWidth;
            float y = -row * cellHeight;

            foodInstance.transform.localPosition = new Vector3(x, y, 0f);
        }
    }

    void Update()
    {
        
    }
}
