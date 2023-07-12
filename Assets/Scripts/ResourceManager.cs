using UnityEngine;

public struct ResourceClass
{
    private int food;
    private int wood;
    private int stone;
    private int tin;
    private int copper;
    
    public ResourceClass(int food, int wood, int stone, int tin, int copper)
    {
        this.food = food;
        this.wood = wood;
        this.stone = stone;
        this.tin = tin;
        this.copper = copper;
    }
    
    public void ChangeResources(ResourceClass changeAmount)
    {
        food += changeAmount.food;
        wood += changeAmount.wood;
        stone += changeAmount.stone;
        tin += changeAmount.tin;
        copper += changeAmount.copper;
    }
}

public class ResourceManager : MonoBehaviour
{
    public ResourceClass CurrentResources { get; } = new ResourceClass();
}
