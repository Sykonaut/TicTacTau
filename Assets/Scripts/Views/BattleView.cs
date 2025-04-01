using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{

    #region Variables

    //--- Serialized ---

    [Header("Player References")]

    [SerializeField]
    private BattlePlayerElement player1Element;

    [SerializeField]
    private BattlePlayerElement player2Element;

    [Header("Level References")]

    [SerializeField]
    private TMP_Text levelNameText;

    [SerializeField]
    private TMP_Text levelDescriptionText;

    [SerializeField]
    private GridLayoutGroup boardGrid;

    [SerializeField]
    private BattleTileEntry boardTileEntryPrefab;


    //--- Non-Serialized ---

    private LevelInstance level;
    private PlayerInstance player1;
    private PlayerInstance player2;

    private Action onExitButtonCallback;
    private Action<TileInstance> onTileClickedCallback;

    private List<BattleTileEntry> battleTileEntries = new List<BattleTileEntry>();

    #endregion

    #region Methods

    public void Initialize(LevelInstance level,
        PlayerInstance player1,
        PlayerInstance player2,
        Action<TileInstance> OnTileClickedCallback,
        Action OnExitButtonCallback)
    {
        // Cache values
        this.level = level;
        this.player1 = player1;
        this.player2 = player2;

        onExitButtonCallback = OnExitButtonCallback;
        onTileClickedCallback = OnTileClickedCallback;

        InitializePlayerElements();
        InitializeLevelElements();
    }

    private void InitializePlayerElements()
    {
        player1Element.Initialize(player1);
        player2Element.Initialize(player2);
    }

    private void InitializeLevelElements()
    {
        levelNameText.text = level.levelTemplate.DisplayName;
        levelDescriptionText.text = level.levelTemplate.Description;

        // Set the board grid
        boardGrid.constraintCount = level.levelTemplate.ColumnCount;

        // Create the board grid
        foreach (var tile in level.tileGrid)
        {
            var tileEntry = Instantiate(boardTileEntryPrefab, boardGrid.transform);

            tileEntry.Initialize(tile, OnTileEntryClicked);

            battleTileEntries.Add(tileEntry);
        }
    }

    private void RefreshTileEntries()
    {
        foreach (var tileEntry in battleTileEntries)
        {
            tileEntry.Refresh();
        }
    }

    public void OnBattleStateChanged(GameManager.BattleState newState)
    {
        Debug.Log("BattleView -> OnBattleStateChanged = " + newState);

        RefreshTileEntries();

        // Update the player turns
        player1Element.SetPlayerTurn(newState == GameManager.BattleState.Player1Turn);
        player2Element.SetPlayerTurn(newState == GameManager.BattleState.Player2Turn);

    }

    public void OnBattleFinished(PlayerInstance winningPlayer)
    {
        Debug.Log("BattleView -> OnBattleFinished = " + winningPlayer);

        player1Element.SetWinState(winningPlayer);
        player2Element.SetWinState(winningPlayer);
    }

    #endregion

    #region UI Events

    public void OnExitButton()
    {
        if (onExitButtonCallback != null)
        {
            onExitButtonCallback();
        }
    }

    private void OnTileEntryClicked(BattleTileEntry tileEntry)
    {
        Debug.Log($"LobbyView -> OnPlayerClicked: Row[{tileEntry.Tile.Row}] Column[{tileEntry.Tile.Column}] Player[{tileEntry.Tile.Player}]");

        if (onTileClickedCallback != null)
        {
            onTileClickedCallback(tileEntry.Tile);
        }

    }

    #endregion
}
