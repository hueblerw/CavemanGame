

public class Potato : Crop {

    public Potato(double regenerationRate) : base(regenerationRate)
    {
        cropName = "potato";
        minTemp = 58;
        maxTemp = 80;
        minWater = 12.3;
        maxWater = 19.7;
        growthPeriod = 39;
        humanFoodUnits = 1.34;
    }

}
