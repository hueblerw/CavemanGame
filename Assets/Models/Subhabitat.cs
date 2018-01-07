using System.Collections.Generic;

public class Subhabitat
{

    private int index;
    private double percentage;
    private double usage;

    private static Dictionary<int, string> indexToStringMap;
    private static Dictionary<string, int> stringToIndexMap;

    public Subhabitat(int index)
    {
        this.index = index;
    }

    public double getPercentage()
    {
        return percentage;
    }

    public double getUsage()
    {
        return usage;
    }

    public void setPercentage(double percentage)
    {
        this.percentage = percentage;
    }

    public static int StringToIndex(string name)
    {
        if (stringToIndexMap == null)
        {
            initializeIndexMap();
        }

        return stringToIndexMap[name];
    }


    public static string IndexToString(int index)
    {
        if (indexToStringMap == null)
        {
            initializeIndexMap();
        }

        return indexToStringMap[index];
    }

    private static void initializeIndexMap()
    {
        indexToStringMap = new Dictionary<int, string>();
        stringToIndexMap = new Dictionary<string, int>();
        addToBothDictionaries(0, "glacier");
        addToBothDictionaries(1, "dry tundra");
        addToBothDictionaries(2, "tundra");
        addToBothDictionaries(3, "boreal");
        addToBothDictionaries(4, "artic marsh");
        addToBothDictionaries(5, "desert");
        addToBothDictionaries(6, "plains");
        addToBothDictionaries(7, "forest");
        addToBothDictionaries(8, "swamp");
        addToBothDictionaries(9, "hot desert");
        addToBothDictionaries(10, "savannah");
        addToBothDictionaries(11, "monsoon forest");
        addToBothDictionaries(12, "rainforest");
        addToBothDictionaries(13, "ocean");
    }

    private static void addToBothDictionaries(int index, string habitat)
    {
        indexToStringMap.Add(index, habitat);
        stringToIndexMap.Add(habitat, index);
    }

}