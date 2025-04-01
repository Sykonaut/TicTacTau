using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data for a single level that defines the board layout (3x3) and board rules (row of 3, col of 3, diag of 3)
/// </summary>
[CreateAssetMenu(fileName = "LevelTemplate", menuName = "ScriptableObjects/LevelTemplate", order = 1)]
public class LevelTemplate : ScriptableObject
{
    #region Definitions

    // TODO: These should really be using AND and OR logic; by default it is always OR

    public enum LevelWinType
    {
        Row,
        Column,
        Diagonal,
        Square
    }

    [System.Serializable]
    public class LevelWinRules
    { 
        public LevelWinType WinType;    // Ex. Row
        public int WinTileAmount;       // Ex. 3
    }

    #endregion

    #region Variables

    [SerializeField]
    private int rowCount;

    [SerializeField]
    private int columnCount;

    [SerializeField]
    private LevelWinRules[] levelWinRules;

    [SerializeField]
    private string displayName;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private string description;

    #endregion

    #region Definitions


    public int RowCount => rowCount;

    public int ColumnCount => columnCount;

    public IEnumerable<LevelWinRules> WinRules => levelWinRules;

    public string DisplayName => displayName;

    public Sprite Icon => icon;

    public string Description => description;

    #endregion
}
