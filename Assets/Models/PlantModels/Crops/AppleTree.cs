
public class AppleTree : Crop
{

    public AppleTree(double regenerationRate) : base(regenerationRate)
    {
        cropName = "apple";
        minTemp = 48;
        maxTemp = 70;
        minWater = 12.3;
        maxWater = 19.7;
        growthPeriod = 39;
        humanFoodUnits = 1.29;
    }

}
