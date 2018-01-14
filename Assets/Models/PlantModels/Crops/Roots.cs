
public class Roots : Crop
{

    public Roots(double regenerationRate) : base(regenerationRate)
    {
        cropName = "roots";
        minTemp = 60;
        maxTemp = 90;
        minWater = 12.3;
        maxWater = 24.7;
        growthPeriod = 35;
        humanFoodUnits = .43;
    }

}
