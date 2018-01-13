
public class Berries : Crop
{

    public Berries(double regenerationRate) : base(regenerationRate)
    {
        cropName = "berries";
        minTemp = 43;
        maxTemp = 79;
        minWater = 12.3;
        maxWater = 24.7;
        growthPeriod = 39;
        humanFoodUnits = .37;
    }

}
