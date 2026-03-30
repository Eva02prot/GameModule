using System;
using UnityEngine;

public enum IngredientStatType
{
    Flavor,
    Nutrition,
    Energy,
    Aroma,
    Property
}

[Serializable]
public class IngredientStat
{
    public IngredientStatType StatType;
    public float Value;
}