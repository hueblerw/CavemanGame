using System;

public class Crop : Plant {

	public Crop(double regenerationRate) : base(regenerationRate){
    }

    // Constants
    public const int NUM_OF_CROPS = 12;
    public const int LAST_X_DAYS = 5;

    // Variables
    protected int maxTemp;
    protected int minTemp;
    protected double minWater;
    protected double maxWater;
    protected int growthPeriod;
    protected double humanFoodUnits;
    protected string cropName;

    protected double rainSum;
    protected double percentGrowable;


    // NOTE ***************
    // So far the crops can't grow early in the year because for that they need access to information from the previous year.
    // Implementation of that will be a bit tricky so I am saving it for later.
    // Also, these represent the number of new crops that grew today.  A scavenger would have access to the last x days worth of crops.
    // Calculate how much of a crop is present upon request
    public double ReturnCurrentCropArray(int day, double percentage, Days[] days)
    {
        double currentCrop = 0.0;
        percentGrowable = 1.0;
        // For each of the crops
        // If he crop can grow in the region return the crops store the crops returned value in the current crop array for today.
        if (DayTempAllowCrop(day, days) && DayRainAllowCrop(day, days))
        {
            // Debug.Log("Crops Allowed!");
            double cropMultiplier = (1.0 / ((80 - growthPeriod) * 100.0)) * 400.0 * percentage;
            // Calculate the crop quality
            currentCrop = cropQuality(day, days) * cropMultiplier * humanFoodUnits * percentGrowable;
            // Debug.Log(x + ", " + z + " / " + cropQuality(day, temps) + " / " + percentGrowable + " / " + humanFoodUnits);
        }

        return currentCrop;
    }


    // Determine if starting on today the previous days have a suitable temperature range.
    private bool DayTempAllowCrop(int day, Days[] days)
    {
        // Can grow if the temperature is within +/- 10 degrees of the ideal temperature range
        if (day - growthPeriod > 0)
        {
            int startGrowthDay = day - growthPeriod;
            for (int d = day; d > startGrowthDay; d--)
            {
                if (days[d].temp < minTemp - 10 || days[d].temp > maxTemp + 10)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }


    // Return the growing crops Quality
    private double cropQuality(int day, Days[] days)
    {
        int goodDays = 0;
        int startGrowthDay = day - growthPeriod;
        // temperature multiplier is % of days that are within the ideal temperature range.
        for (int d = day; d > startGrowthDay; d--)
        {
            if (days[d].temp >= minTemp || days[d].temp <= maxTemp)
            {
                goodDays++;
            }
        }
        // rain multiplier is between 50% and 125%  based on how close to ideal the rainfall level was.
        double maxDist = (maxWater - minWater) / 2.0;
        double idealRain = (maxWater + minWater) / 2.0;
        double rainMultiplier = 1.25 - ((Math.Abs(rainSum - idealRain) / maxDist) * .75);
        // return the two modifiers used together.
        return (goodDays / growthPeriod) * rainMultiplier * 100.0;
    }


    // Determine if starting on today the previous days have a suitable rainfall sum.
    private bool DayRainAllowCrop(int day, Days[] days)
    {
        // can grow ONLY if the rainfall is within the ideal rainfall range
        double sum = 0;
        double surfaceSum = 0;
        if (day - growthPeriod > 0)
        {
            int startGrowthDay = day - growthPeriod;
            // Sum the rainfall in the crops growing period
            for (int d = day; d > startGrowthDay; d--)
            {
                sum += days[d].rain;
                surfaceSum += days[d].surfaceWater * Habitat.RIVERWATERINGCONSTANT;
            }
            // If that sum is in the acceptable range set the rainSum variable and return true, else return false.
            // Ideally if any value in the range of values from sum to sum + surfacewaterSum is between minWater and maxWater
            // Return true and rainSum set and percentGrowable set.
            // Else return false
            if ((sum > minWater && sum < maxWater) || (sum < minWater && (sum + surfaceSum) > minWater))
            {
                // Set the rainSum with the midpoint of the trapezoid of possible return values.
                rainSum = Support.Average(Math.Min(sum + surfaceSum, maxWater), Math.Max(sum, minWater));
                // Set the percent Growable as the percent of the tile that actually is in the acceptable water range.
                if (surfaceSum == 0)
                {
                    percentGrowable = 1.0;
                }
                else
                {
                    percentGrowable = (Math.Min(sum + surfaceSum, maxWater) - Math.Max(sum, minWater)) / surfaceSum;
                }
                // return true because stuff grows here
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public string getName()
    {
        return cropName;
    }

}
