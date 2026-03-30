using System;
using System.Collections.Generic;

[Serializable]
public class IngredientCountData
{
    public int IngredientID;
    public int Count;
}

[Serializable]
public class IngredientStorageData
{
    public List<IngredientCountData> Items = new List<IngredientCountData>();
}
