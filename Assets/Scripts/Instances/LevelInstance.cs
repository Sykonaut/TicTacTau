using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This is ugly :)
public class LevelInstance 
{
    public LevelTemplate levelTemplate;

    public TileInstance[,] tileGrid;

    public LevelInstance() { }

    public void CreateLevelBoard()
    {
        tileGrid = new TileInstance[levelTemplate.RowCount, levelTemplate.ColumnCount];

        // Create base tile instances
        for (int row = 0; row < levelTemplate.RowCount; row++)
        {
            for (int col = 0; col < levelTemplate.ColumnCount; col++)
            {
                tileGrid[row, col] = new TileInstance(row, col);
            }
        }
    }

    public void ClearLevelBoard()
    {
        tileGrid = null;
    }

    private TileInstance GetTile(int row, int col)
    {
        if (row < 0 || row >= levelTemplate.RowCount)
            return null;

        if (col < 0 || col >= levelTemplate.ColumnCount)
            return null;

        return tileGrid[row, col];
    }

    // Trying to grab all the tiles in a row starting from a current tile and up to a given amount
    // NOTE: Only need to check "forward" with row and column checks; Diagonal checks need to go 2 directions
    public bool CheckPlayerTilesInRow(TileInstance startTile, int desiredTileCount)
    {
        for (int column = startTile.Column; column < startTile.Column + desiredTileCount; column++)
        {
            // Make sure the tile is valid, if not then return false
            var tile = GetTile(startTile.Row, column);
            if (tile == null)
            {
                return false;
            }

            // Make sure the tile is the same player as the player we're checking
            if (tile.Player != startTile.Player)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckPlayerTilesInColumn(TileInstance startTile, int desiredTileCount)
    {
        for (int row = startTile.Row; row < startTile.Row + desiredTileCount; row++)
        {
            // Make sure the tile is valid, if not then return false
            var tile = GetTile(row, startTile.Column);
            if (tile == null)
            {
                return false;
            }

            // Make sure the tile is the same player as the startTile's player
            if (tile.Player != startTile.Player)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckPlayerTilesInDiagonal(TileInstance startTile, int desiredTileCount)
    {
        if (CheckPlayerTilesInDiagonalBR(startTile, desiredTileCount))
            return true;

        if (CheckPlayerTilesInDiagonalBL(startTile, desiredTileCount))
            return true;

        return false;
    }

    private bool CheckPlayerTilesInDiagonalBR(TileInstance startTile, int desiredTileCount)
    {
        // Test "bottom right" diagonal; increasing row and increasing column
        int row = startTile.Row;
        int column = startTile.Column;
        for (int tileCount = 0; tileCount < desiredTileCount; tileCount++)
        {
            // Make sure the tile is valid, if not then return false
            var tile = GetTile(row, column);
            if (tile == null)
            {
                return false;
            }

            // Make sure the tile is the same player as the startTile's player
            if (tile.Player != startTile.Player)
            {
                return false;
            }

            row++;
            column++;
        }

        return true;
    }

    private bool CheckPlayerTilesInDiagonalBL(TileInstance startTile, int desiredTileCount)
    {
        // Test "bottom left" diagonal; increasing row and decreasing column
        int row = startTile.Row;
        int column = startTile.Column;
        for (int tileCount = 0; tileCount < desiredTileCount; tileCount++)
        {
            // Make sure the tile is valid, if not then return false
            var tile = GetTile(row, column);
            if (tile == null)
            {
                return false;
            }

            // Make sure the tile is the same player as the startTile's player
            if (tile.Player != startTile.Player)
            {
                return false;
            }

            row++;
            column--;
        }

        return true;
    }

    public bool CheckPlayerTilesInSquare(TileInstance startTile, int desiredTileCount)
    {
        // The desired count means a square of that size (must be > 1); Ex. 2 = 2x2, 3 = 3x3, 4 = 4x4
        // So just do a double for loop against the desired count
        for (int row = startTile.Row; row < startTile.Row + desiredTileCount; row++)
        {
            for (int col = startTile.Column; col < startTile.Column + desiredTileCount; col++)
            {
                // Make sure the tile is valid, if not then return false
                var tile = GetTile(row, col);
                if (tile == null)
                {
                    return false;
                }

                // Make sure the tile is the same player as the startTile's player
                if (tile.Player != startTile.Player)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
