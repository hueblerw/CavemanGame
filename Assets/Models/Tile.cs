using System;
using System.Collections.Generic;

public class Tile {

    // Constants
    private const double RIVER_FLOWRATE_MULTIPLIER = 4.0;
    private const double RIVER_STEEPNESS_MULTIPLIER = 2.5;

    // Variables
    public Days[] dayArray;
    //public double habitatPercents;
    public int highTemp;
    public int lowTemp;
    public double variance;
    public TempEquation tempEQ;
    public double springMidpt;
    // public Array humidity;
    public double elevation;
    public double oceanPercent;
    public double hillPercent;
    public string flowDirection;
    public double soilAbsorption;
    public List<string> upstreamDirections;
    public double[] randomTreeDistributor;
    public Habitat habitat;
    public double surfaceStone;
    public double minableStone;


	public Tile()
    {
        // Tile constructor
        dayArray = new Days[Date.DAYS_PER_YEAR];
        for (int day = 0; day < Date.DAYS_PER_YEAR; day++)
        {
            dayArray[day] = new Days();
        }
        // randomTreeDistributor = new double[TerrainView.MAX_TREES_PER_TILE];
        upstreamDirections = new List<string>();
        // humidity = new Array[MainController.NUM_OF_HUMIDITY_FILES];
    }


    public void CreateTempEQ()
    {
        tempEQ = new TempEquation(highTemp, lowTemp, (float)springMidpt, (float)variance);
    }


    public double HabitatYearInfo(out double surfaceAvg, out int hotDays, out int coldDays)
    {
        double rainSum = 0.0;
        hotDays = 0;
        coldDays = 0;
        surfaceAvg = 0.0;
        for (int day = 0; day < Date.DAYS_PER_YEAR; day++)
        {
            rainSum += dayArray[day].precip;
            surfaceAvg += dayArray[day].surfaceWater;
            if (dayArray[day].temp > 70)
            {
                hotDays++;
            }
            else if (dayArray[day].temp < 32)
            {
                coldDays++;
            }
        }

        surfaceAvg = surfaceAvg / Date.DAYS_PER_YEAR;
        return rainSum;
    }


    public string printRiverDirections()
    {
        string output = " - D: " + flowDirection + " // U: ";
        for (int i = 0; i < upstreamDirections.Count; i++)
        {
            output += upstreamDirections[i] + ", ";
        }
        return output;
    }

    public double getFlowRate(double riverVolume)
    {
        double steepness = hillPercent * RIVER_STEEPNESS_MULTIPLIER;
        double flowrate = Math.Round(riverVolume * steepness * (Math.Sqrt(riverVolume / 600.0) / (RIVER_FLOWRATE_MULTIPLIER * World.WIDTH_HEIGHT_RATIO)), 4);
        // double flowrate = Math.Round((((this.hillPercent * 2 + randy.NextDouble() * 2 + 4) / (1.0 - this.oceanPercent)) / 100.0), 4);
        return flowrate;
    }

}
