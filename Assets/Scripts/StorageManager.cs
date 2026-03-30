using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance { get; private set; }

    [Header("全部食材配置表")]
    [SerializeField] private List<Ingredient> mAllIngredients = new List<Ingredient>();

    // 运行时：ID -> 食材配置
    private Dictionary<int, Ingredient> mIngredientDataMap = new Dictionary<int, Ingredient>();

    // 运行时：ID -> 数量
    private Dictionary<int, int> mIngredient = new Dictionary<int, int>();

    private string mSavePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        mSavePath = Path.Combine(Application.persistentDataPath, "inventory.json");

        InitIngredientMap();
        LoadIngredientStorage();
    }

    private void InitIngredientMap()
    {
        mIngredientDataMap.Clear();

        for (int i = 0; i < mAllIngredients.Count; i++)
        {
            Ingredient data = mAllIngredients[i];
            if (data == null)
            {
                continue;
            }

            if (mIngredientDataMap.ContainsKey(data.ID))
            {
                Debug.LogWarning($"重复食材ID: {data.ID} Name={data.IngredientName}");
                continue;
            }

            mIngredientDataMap.Add(data.ID, data);
        }
    }

    public void AddIngredient(Ingredient ingredient, int count)
    {
        if (ingredient == null || count <= 0)
        {
            return;
        }

        AddIngredientByID(ingredient.ID, count);
    }

    public void AddIngredientByID(int ingredientID, int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (mIngredient.ContainsKey(ingredientID))
        {
            mIngredient[ingredientID] += count;
        }
        else
        {
            mIngredient.Add(ingredientID, count);
        }
    }

    public bool RemoveIngredient(Ingredient ingredient, int count)
    {
        if (ingredient == null || count <= 0)
        {
            return false;
        }

        return RemoveIngredientByID(ingredient.ID, count);
    }

    public bool RemoveIngredientByID(int ingredientID, int count)
    {
        if (count <= 0)
        {
            return false;
        }

        if (!mIngredient.ContainsKey(ingredientID))
        {
            return false;
        }

        if (mIngredient[ingredientID] < count)
        {
            return false;
        }

        mIngredient[ingredientID] -= count;

        if (mIngredient[ingredientID] <= 0)
        {
            mIngredient.Remove(ingredientID);
        }

        return true;
    }

    public int GetIngredientCount(Ingredient ingredient)
    {
        if (ingredient == null)
        {
            return 0;
        }

        return GetIngredientCountByID(ingredient.ID);
    }

    public int GetIngredientCountByID(int ingredientID)
    {
        if (mIngredient.TryGetValue(ingredientID, out int count))
        {
            return count;
        }

        return 0;
    }

    public bool HasEnough(Ingredient ingredient, int needCount)
    {
        if (ingredient == null || needCount <= 0)
        {
            return false;
        }

        return GetIngredientCount(ingredient) >= needCount;
    }

    public Ingredient GetIngredientDataByID(int ingredientID)
    { 
        if (mIngredientDataMap.TryGetValue(ingredientID, out Ingredient data))
        {
            return data;
        }

        return null;
    }

    public Dictionary<int, int> GetIngredientRaw()
    {
        return mIngredient;
    }

    public void SaveIngredientStorage()
    {
        IngredientStorageData saveData = new IngredientStorageData();

        foreach (var pair in mIngredient)
        {
            IngredientCountData item = new IngredientCountData();
            item.IngredientID = pair.Key;
            item.Count = pair.Value;
            saveData.Items.Add(item);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(mSavePath, json);

        Debug.Log($"库存已保存: {mSavePath}");
    }

    public void LoadIngredientStorage()
    {
        mIngredient.Clear();

        if (!File.Exists(mSavePath))
        {
            Debug.Log("库存存档不存在，使用空库存");
            return;
        }

        string json = File.ReadAllText(mSavePath);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("库存存档为空");
            return;
        }

        IngredientStorageData saveData = JsonUtility.FromJson<IngredientStorageData>(json);

        if (saveData == null || saveData.Items == null)
        {
            Debug.LogWarning("库存存档解析失败");
            return;
        }

        for (int i = 0; i < saveData.Items.Count; i++)
        {
            IngredientCountData item = saveData.Items[i];
            if (item == null)
            {
                continue;
            }

            mIngredient[item.IngredientID] = item.Count;
        }

        Debug.Log($"库存已读取: {mSavePath}");
    }

    public void GenerateIngredientJson()
    {
        IngredientStorageData saveData = new IngredientStorageData();

        for (int i = 0; i < mAllIngredients.Count; i++)
        {
            Ingredient ingredient = mAllIngredients[i];
            if (ingredient == null)
            {
                continue;
            }

            IngredientCountData item = new IngredientCountData();
            item.IngredientID = ingredient.ID;
            item.Count = 0;
            saveData.Items.Add(item);
        }

        string json = JsonUtility.ToJson(saveData, true);

        string dir = Path.GetDirectoryName(mSavePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(mSavePath, json);

        Debug.Log($"初始库存 JSON 生成成功: {mSavePath}");
    }

    public void ClearIngredient()
    {
        mIngredient.Clear();
    }

    private void OnApplicationQuit()
    {
        SaveIngredientStorage();
    }
}
