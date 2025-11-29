[System.Serializable]
public class ItemRequirement
{
    public MushroomType type;
    public int neededCount;

    public System.Func<MushroomProperties, bool> matchCondition;
}
