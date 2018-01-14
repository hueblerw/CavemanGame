
public class Corn : Crop {

    public Corn(double regenerationRate) : base(regenerationRate)
    {
        cropName = "corn";
        minTemp = 68;
        maxTemp = 88;
        minWater = 14.8;
        maxWater = 22.2;
        growthPeriod = 28;
        humanFoodUnits = 1.08;
    }

}
