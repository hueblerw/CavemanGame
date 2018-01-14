
public class Beans : Crop
{

    public Beans(double regenerationRate) : base(regenerationRate)
    {
        cropName = "beans";
        minTemp = 66;
        maxTemp = 86;
        minWater = 4.9;
        maxWater = 9.9;
        growthPeriod = 39;
        humanFoodUnits = .32;
    }

}

