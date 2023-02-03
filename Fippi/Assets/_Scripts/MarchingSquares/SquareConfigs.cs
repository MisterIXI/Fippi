using System;
using System.Collections.Generic;

static class SquareConfigs
{
    public static Dictionary<byte, int[]> Triangles = new Dictionary<byte, int[]>(){
        // floor
        {0b0000, new int[] {}},
        // ceiling
        {0b1111, new int[] {0, 5, 7, 0, 7, 2}},
        // outer corners
        {0b0111, new int[] {3,5,7,1,3,7,2,1,7}},
        {0b1011, new int[] {1,0,5,4,1,5,7,4,5}},
        {0b1101, new int[] {4,2,0,6,4,0,5,6,0}},
        {0b1110, new int[] {6,7,2,3,6,2,0,3,2}},
        // inner corners
        {0b1000, new int[] {0,3,1}},
        {0b0100, new int[] {1,4,2}},
        {0b0010, new int[] {7,4,6}},
        {0b0001, new int[] {5,6,3}},
        // walls
        {0b1100, new int[] {0,3,4,2,0,4}},
        {0b0110, new int[] {2,1,6,6,7,2}},
        {0b0011, new int[] {3,5,7,7,4,3}},
        {0b1001, new int[] {5,6,0,0,6,1}},
        // connecting walls
        {0b1010, new int[] {0,3,1,1,3,6,1,6,4,4,6,7}},
        {0b0101, new int[] {3,5,6,1,6,4,1,3,6,1,4,2}}
    };
}