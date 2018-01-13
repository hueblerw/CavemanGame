
public class Nuts : Crop
{

    public Nuts(double regenerationRate) : base(regenerationRate)
    {
        cropName = "nuts";
        minTemp = 52;
        maxTemp = 73;
        minWater = 19.7;
        maxWater = 34.5;
        growthPeriod = 69;
        humanFoodUnits = 1.0; // ?????????????????
    }

}
