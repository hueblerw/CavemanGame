
public class Rice : Crop
{

    public Rice(double regenerationRate) : base(regenerationRate)
    {
        cropName = "rice";
        minTemp = 67;
        maxTemp = 90;
        minWater = 24.7;
        maxWater = 32.1;
        growthPeriod = 58;
        humanFoodUnits = 2.30;
    }

}
