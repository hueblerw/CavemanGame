

public class Days {

    // Constants

    // Variables
    public int temp;
    public double precip;
    public double rain;
    public double snowCover;
    public double surfaceWater;
    int trees;
    int shrubs;
    int scrubs;
    int grazing;
    int game;
    // Crops crops;


    // Constructors
    public Days()
    {
    }

    public string printWeatherInfo()
    {
        string weather = "Weather:\n" + temp + " deg-F\t";
        if(precip > 0.0)
        {
            if(temp > 32)
            {
                weather += "/ " + rain + " inches of rainfall";
            }
            else
            {
                weather += "/ " + precip + " inches of snowfall";
            }
        }
        if (snowCover > 0.0)
        {
            weather += "\nSnow cover: " + snowCover;
        }
        if (surfaceWater > 0.0)
        {
            weather += "\nRiver Level: " + surfaceWater;
        }
        return weather;
    }

}
