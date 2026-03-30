using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Scriptable Objects/Ingredient")]
public class Ingredient : ScriptableObject
{
    public int ID;
    public string IngredientName;
    public Sprite Icon;

    [TextArea]
    public string Description;

    [Header("Basic Property")]
    public List<IngredientStat> BaseStats = new List<IngredientStat>();

    public float GetStatValue(IngredientStatType statType)
    {
        for (int i = 0; i < BaseStats.Count; i++)
        {
            if (BaseStats[i].StatType == statType)
            {
                return BaseStats[i].Value;
            }
        }

        return 0f;
    }
}
