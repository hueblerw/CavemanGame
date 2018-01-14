
public class Grape : Crop
{

    public Grape(double regenerationRate) : base(regenerationRate)
    {
        cropName = "grape";
        minTemp = 71;
        maxTemp = 91;
        minWater = 7.4;
        maxWater = 12.3;
        growthPeriod = 51;
        humanFoodUnits = 1.03;
    }

}
