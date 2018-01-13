using UnityEngine;
using System.Collections.Generic;

public class Support {

    public static float[] CellsAroundVertex(int x, int z, int WorldX, int WorldZ, float[,] array)
    {
        float[] cells;
        List<float> cellList = Support.getAround(x, z, WorldX, WorldZ, array);
        cells = cellList.ToArray();
        return cells;
    }

    public static float[] CellsAllAround(int x, int z, int WorldX, int WorldZ, float[,] array)
    {
        float[] cells;
        List<float> cellList = Support.getAroundCenter(true, x, z, WorldX, WorldZ, array); ;
        cells = cellList.ToArray();
        return cells;
    }

    public static List<string> getDirectionAsStringBelow(bool diagonals, int x, int z, int WorldX, int WorldZ, float testValue, float[,] array)
    {
        List<string> cellList = new List<string>();
        // Add strings representing the directions if legal
        if (x > 0 && array[x - 1, z] < testValue)
        {
            cellList.Add("left");
        }
        if (x < WorldX - 1 && array[x + 1, z] < testValue)
        {
            cellList.Add("right");
        }
        if (z > 0 && array[x, z - 1] < testValue)
        {
            cellList.Add("up");
        }
        if (z < WorldZ - 1 && array[x, z + 1] < testValue)
        {
            cellList.Add("down");

        }
        // Add the diagonals if required and legal
        if (diagonals)
        {
            if (x < WorldX - 1 && z < WorldZ - 1 && array[x + 1, z + 1] < testValue)
            {
                cellList.Add("lower right");
            }
            if (x > 0 && z > 0 && array[x - 1, z - 1] < testValue)
            {
                cellList.Add("upper left");
            }
            if (x > 0 && z < WorldZ - 1 && array[x - 1, z + 1] < testValue)
            {
                cellList.Add("lower left");
            }
            if (x < WorldX - 1 && z > 0 && array[x + 1, z - 1] < testValue)
            {
                cellList.Add("upper right");
            }
        }

        return cellList;
    }

    // Private methods
    private static List<float> getAround(int x, int z, int WorldX, int WorldZ, float[,] array)
    {
        List<float> cellList = new List<float>();
        // Add the four caridnal values if legal
        if (x < WorldX && z < WorldZ)
        {
            cellList.Add(array[x, z]);
        }
        if (x > 0 && z > 0)
        {
            cellList.Add(array[x - 1, z - 1]);
        }
        if (x > 0 && z < WorldZ)
        {
            cellList.Add(array[x - 1, z]);
        }
        if (x < WorldX && z > 0)
        {
            cellList.Add(array[x, z - 1]);
        }

        return cellList;
    }

    private static List<float> getAroundCenter(bool diagonals, int x, int z, int WorldX, int WorldZ, float[,] array)
    {
        List<float> cellList = new List<float>();
        // Add the four caridnal values if legal
        if (x > 0)
        {
            cellList.Add(array[x - 1, z]);
        }
        if (x < WorldX - 1)
        {
            cellList.Add(array[x + 1, z]);
        }
        if (z > 0)
        {
            cellList.Add(array[x, z - 1]);
        }
        if (z < WorldZ - 1)
        {
            cellList.Add(array[x, z + 1]);
        }
        // Add the diagonals if required and legal
        if (diagonals)
        {
            if (x < WorldX - 1 && z < WorldZ - 1)
            {
                cellList.Add(array[x + 1, z + 1]);
            }
            if (x > 0 && z > 0)
            {
                cellList.Add(array[x - 1, z - 1]);
            }
            if (x > 0 && z < WorldZ - 1)
            {
                cellList.Add(array[x - 1, z + 1]);
            }
            if (x < WorldX - 1 && z > 0)
            {
                cellList.Add(array[x + 1, z - 1]);
            }
        }

        return cellList;
    }


    public static Vector2[] GetCoordinatesAround(int x, int z, int X, int Z)
    {
        List<Vector2> coor = new List<Vector2>();
        if (x > 0)
        {
            if (z > 0)
            {
                coor.Add(new Vector2(x - 1, z - 1));
            }
            if (z < Z - 1)
            {
                coor.Add(new Vector2(x - 1, z + 1));
            }
            coor.Add(new Vector2(x - 1, z));
        }
        if (x < X - 1)
        {
            if (z > 0)
            {
                coor.Add(new Vector2(x + 1, z - 1));
            }
            if (z < Z - 1)
            {
                coor.Add(new Vector2(x + 1, z + 1));
            }
            coor.Add(new Vector2(x + 1, z));
        }
        if (z > 0)
        {
            coor.Add(new Vector2(x, z - 1));
        }
        if (z < Z - 1)
        {
            coor.Add(new Vector2(x, z + 1));
        }

        return coor.ToArray();
    }


    public static List<Vector3> GetCardinalCoordinatesAround(int x, int z, int X, int Z)
    {
        List<Vector3> coor = new List<Vector3>();
        if (x > 0)
        {
            // Left - 0
            coor.Add(new Vector3(x - 1, z, 0));
        }
        if (x < X - 1)
        {
            // Right - 1
            coor.Add(new Vector3(x + 1, z, 1));
        }
        if (z > 0)
        {
            // Down - 2
            coor.Add(new Vector3(x, z - 1, 2));
        }
        if (z < Z - 1)
        {
            // Up - 3
            coor.Add(new Vector3(x, z + 1, 3));
        }

        return coor;
    }


    public static Vector2[] getCoordinatesAroundVertex(int x, int z, int X, int Z)
    {
        List<Vector2> coorList = new List<Vector2>();
        // Add the four caridnal values if legal
        if (x < X && z < Z)
        {
            coorList.Add(new Vector2(x, z));
        }
        if (x > 0 && z > 0)
        {
            coorList.Add(new Vector2(x - 1, z - 1));
        }
        if (x > 0 && z < Z)
        {
            coorList.Add(new Vector2(x - 1, z));
        }
        if (x < X && z > 0)
        {
            coorList.Add(new Vector2(x, z - 1));
        }

        return coorList.ToArray();
    }
    

    public static double ChooseMinOf(double A, double B)
    {
        if (A > B)
        {
            return B;
        }
        else
        {
            return A;
        }
    }

    // Averages two doubles
    public static double Average(double a, double b)
    {
        return (a + b) / 2.0;
    }

}
