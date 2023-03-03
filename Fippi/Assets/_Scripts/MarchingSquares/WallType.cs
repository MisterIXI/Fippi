using System;
using System.Linq;
public enum WallType
{
    None = 0,
    Wall = 1,
    PermanentWall = 2,
    RessourceA = 3,
    RessourceB = 4
}

public static class WallTypeExtensions
{
    public static int WallCount = Enum.GetNames(typeof(WallType)).Length - 1;
    public static int SpecialWallCount = Enum.GetNames(typeof(WallType)).Length - 2;
    public static WallType LastType = (WallType)Enum.GetValues(typeof(WallType)).Cast<int>().Max();
    // public static int GetWallCount(this int value)
    // {
    //     int walltype_count = Enum.GetNames(typeof(WallType)).Length;
    //     // substract NONE
    //     walltype_count--;
    //     return walltype_count;
    // }
}