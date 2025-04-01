using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance
{
    public int Row;
    public int Column;

    public PlayerInstance Player;   // Current player occupying

    public TileInstance(int row, int column)
    {
        Row = row;
        Column = column;
        Player = null;
    }
}
