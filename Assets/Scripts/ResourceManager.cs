using System.Reflection;
using UnityEngine;

[System.Serializable]
public class ResourceClass
{
    public int food;
    public int wood;
    public int stone;
    public int tin;
    public int copper;
    
    public ResourceClass(int food, int wood, int stone, int tin, int copper)
    {
        this.food = food;
        this.wood = wood;
        this.stone = stone;
        this.tin = tin;
        this.copper = copper;
    }
}

public class ResourceManager : MonoBehaviour
{
    public ResourceClass CurrentResources { get; } = new ResourceClass(
            100,
            50,
            0,
            0,
            0
        );

    public static ResourceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void ChangeResources(ResourceClass changeAmount)
    {
        CurrentResources.food += changeAmount.food;
        CurrentResources.wood += changeAmount.wood;
        CurrentResources.stone += changeAmount.stone;
        CurrentResources.tin += changeAmount.tin;
        CurrentResources.copper += changeAmount.copper;
    }
    
    public bool HasEnoughResources(ResourceClass costResources)
    {
        for (int i = 0; i < typeof(ResourceClass).GetProperties().Length; i++)
        {
            PropertyInfo resourceProperty = typeof(ResourceClass).GetProperties()[i];
            int requiredAmount = (int)resourceProperty.GetValue(costResources);
            int currentAmount = (int)resourceProperty.GetValue(CurrentResources);

            if (requiredAmount > currentAmount)
                return false;
        }

        return true;
    }
}
