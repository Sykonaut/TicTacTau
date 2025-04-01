using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum BattleState
    {
        None,
        Initializing,
        Player1Turn,
        Player2Turn,
        Finished    // Win, Tie, Loss
    }

    #region Variables

    //--- Serialized ---

    [Header("Configs")]

    [SerializeField]
    private GameConfigTemplate gameConfig;

    [Header("Scene Views")]

    [SerializeField, Tooltip("Main view for showing the Lobby scene")]
    private LobbyView lobbyViewPrefab;

    [SerializeField, Tooltip("Main view for showing the Battle scene")]
    private BattleView battleViewPrefab;

    [SerializeField]
    private Canvas sceneCanvas;

    [Header("Game Settings")]

    [SerializeField]
    private bool showLobbyScene = true;

    //--- Non-Serialized ---

    // Views
    private LobbyView lobbyView;
    private BattleView battleView;

    // Level
    private LevelInstance level;

    // Players
    private PlayerInstance player1;
    private PlayerInstance player2;

    // Battle state

    public BattleState CurrentBattleState { get; private set; } 
    public PlayerInstance CurrentPlayer
    { 
        get
        {
            if (CurrentBattleState == BattleState.Player1Turn)
                return player1;
            
            if (CurrentBattleState == BattleState.Player2Turn)
                return player2;

            return null; 
        }
    }
    public PlayerInstance NonCurrentPlayer
    {
        get
        {
            if (CurrentBattleState == BattleState.Player1Turn)
                return player2;

            if (CurrentBattleState == BattleState.Player2Turn)
                return player1;

            return null;
        }
    }
    public PlayerInstance WinningPlayer { get; private set; }   // Null on Tie

    private Coroutine computerMoveCoroutine;

    #endregion

    #region Unity Messages

    private void Awake()
    {
        // TODO: Normally I would assert or log exceptions for required null references like gameConfig, sceneCanvas, etc.

        // Create the initial player instances with default values; this allows us to play the battle scene directly if desired
        player1 = new PlayerInstance(gameConfig.Player1DefaultValues);
        player2 = new PlayerInstance(gameConfig.Player2DefaultValues);

        // Create the default level instance (the board gets created later; this is just a placeholder
        level = new LevelInstance();    // TODO: Fix constructor?
        level.levelTemplate = gameConfig.DefaultLevelTemplate;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (showLobbyScene)
        {
            LoadLobbyView();
        }
        else
        {
            LoadBattleView();
        }
    }

    #endregion

    #region View Management

    private void LoadLobbyView()
    {
        Debug.Log("GameManager -> LoadLobbyView");

        ClearLobbyView();
        ClearBattleView();

        // Create and parent the lobby view
        lobbyView = Instantiate(lobbyViewPrefab, sceneCanvas.transform);

        // Call initialize of LobbyView and register events -- TODO: Refactor
        lobbyView.Initialize(level, player1, player2, gameConfig.HumanTemplate, gameConfig.ComputerTemplate, gameConfig.CharacterTemplates, gameConfig.LevelTemplates, OnLobbyPlayButton);
    }

    private void ClearLobbyView()
    {
        // Clear lobby if already loaded
        if (lobbyView != null)
        {
            Destroy(lobbyView.gameObject);
            lobbyView = null;
        }
    }

    private void LoadBattleView()
    {
        Debug.Log("GameManager -> LoadBattleView");

        ClearLobbyView();
        ClearBattleView();

        // TODO: Better place?
        CreateBattleBoard(); // Board needed to be created before initializing battleView

        // Create and parent the battle view
        battleView = Instantiate(battleViewPrefab, sceneCanvas.transform);

        // Call initialize of Battleview and register events -- TODO: Refactor
        battleView.Initialize(level, player1, player2, OnTileClicked, OnExitBattleButton);

        // Start the game
        StartBattle();
    }

    private void ClearBattleView()
    {
        // Clear lobby if already loaded
        if (battleView != null)
        {
            Destroy(battleView.gameObject);
            battleView = null;
        }
    }

    private void OnLobbyPlayButton()
    {
        Debug.Log("GameManager -> OnLobbyPlayButton");

        LoadBattleView();
    }

    private void OnExitBattleButton()
    {
        Debug.Log("GameManager -> OnExitBattleButton");

        ClearBattleBoard();

        LoadLobbyView();
    }

    #endregion

    #region Battle State Methods

    private void CreateBattleBoard()
    {
        Debug.Log("GameManager -> CreateBattleBoard");

        CurrentBattleState = BattleState.Initializing;

        if (level == null || level.levelTemplate == null)
        {
            Debug.LogError("Cannot create board because level/template is null");
            return;
        }

        level.CreateLevelBoard();
    }

    private void ClearBattleBoard()
    {
        level.ClearLevelBoard();
        CurrentBattleState = BattleState.None;
        WinningPlayer = null;
        
        if (computerMoveCoroutine != null)
        {
            StopCoroutine(computerMoveCoroutine);
            computerMoveCoroutine = null;
        }
    }

    private void StartBattle()
    {
        // Randomly choose whoever goes first
        int randomPlayer = Random.Range(1, 3);

        if (randomPlayer == 1)
        {
            CurrentBattleState = BattleState.Player1Turn;
        }
        else
        {
            CurrentBattleState = BattleState.Player2Turn;
        }

        if (CurrentPlayer.PlayerType.PlayerBehaviorType == PlayerTemplate.PlayerBehavior.Computer)
        {
            computerMoveCoroutine = StartCoroutine(ComputerWaitCoroutine());
        }

        // Update the view
        battleView.OnBattleStateChanged(CurrentBattleState);
    }

    private void SetNextPlayerTurn()
    {
        Debug.Log($"GameManager - PlacePlayerTile - CurrentBattleState[{CurrentBattleState}]");

        switch (CurrentBattleState)
        {
            case (BattleState.Player1Turn):
                CurrentBattleState = BattleState.Player2Turn;
                break;
            case (BattleState.Player2Turn):
                CurrentBattleState = BattleState.Player1Turn;
                break;
        }

        if (CurrentPlayer.PlayerType.PlayerBehaviorType == PlayerTemplate.PlayerBehavior.Computer)
        {
            computerMoveCoroutine = StartCoroutine(ComputerWaitCoroutine());
        }
    }

    private void PlacePlayerTile(PlayerInstance player, TileInstance tile)
    {
        Debug.Log($"GameManager - PlacePlayerTile - Player[{player.PlayerName}] Tile[{tile.Row},{tile.Column}]");

        // If the tile is already occupied then you cannot place here
        if (tile.Player != null)
        {
            return;
        }

        // Set the player
        tile.Player = player;

        // Check if this move was a winning move
        if (GridHasWinPlayerCondition(player))
        {
            WinningPlayer = player;

            SetGameOver();
        }
        // Check if there is a tie
        else if (GridHasTieWinCondition())
        {
            WinningPlayer = null;

            SetGameOver();
        }
        // Still playing, go to next turn
        else
        {
            SetNextPlayerTurn();
        }

        // Update the view
        battleView.OnBattleStateChanged(CurrentBattleState);
    }

    private bool GridHasWinPlayerCondition(PlayerInstance player)
    {
        // BF: Loop through every tile and every win conditions
        foreach (var tile in level.tileGrid)
        {
            // Early out if the tile is empty or not this player
            if (tile.Player == null || tile.Player != player)
                continue;

            // Check this tile against every win rule
            foreach (var winRule in level.levelTemplate.WinRules)
            {
                bool hasWinCondition = false;
                switch (winRule.WinType)
                {
                    case LevelTemplate.LevelWinType.Row:
                        hasWinCondition = level.CheckPlayerTilesInRow(tile, winRule.WinTileAmount);
                        break;

                    case LevelTemplate.LevelWinType.Column:
                        hasWinCondition = level.CheckPlayerTilesInColumn(tile, winRule.WinTileAmount);
                        break;

                    case LevelTemplate.LevelWinType.Diagonal:
                        hasWinCondition = level.CheckPlayerTilesInDiagonal(tile, winRule.WinTileAmount);
                        break;

                    case LevelTemplate.LevelWinType.Square:
                        hasWinCondition = level.CheckPlayerTilesInSquare(tile, winRule.WinTileAmount);
                        break;
                }

                // Found win condition so return true (TODO: Send list of tiles that make win condition)
                if (hasWinCondition)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool GridHasTieWinCondition()
    {
        // Check if there are any empty tiles, only one needed
        foreach (var tile in level.tileGrid)
        {
            if (tile.Player == null)
                return false;
        }

        return true;
    }

    private void SetGameOver()
    {
        Debug.Log($"GameManager - SetGameOver - Winner[{WinningPlayer?.PlayerName}]");

        CurrentBattleState = BattleState.Finished;

        // Update the view
        battleView.OnBattleFinished(WinningPlayer);
    }

    private TileInstance GetComputerMove(PlayerInstance computerPlayer, PlayerInstance otherPlayer)
    {
        // Let's check all open spaces and see if putting our player in that space would cause a winning move OR a blocking move
        List<TileInstance> winningMoves = new List<TileInstance>();
        List<TileInstance> blockingMoves = new List<TileInstance>();
        List<TileInstance> openMoves = new List<TileInstance>();
        foreach (var tile in level.tileGrid)
        {
            // Early out if the tile is occupied
            if (tile.Player != null)
            {
                continue;
            }

            // First check if changing to the computer player would cause a move
            tile.Player = computerPlayer;
            if (GridHasWinPlayerCondition(computerPlayer))
            {
                winningMoves.Add(tile);
            }

            // Second check if the other player would win if placing here
            tile.Player = otherPlayer;
            if (GridHasWinPlayerCondition(otherPlayer))
            {
                blockingMoves.Add(tile);
            }

            // Always return to null player
            tile.Player = null;

            // Add all open moves
            openMoves.Add(tile);
        }

        // NOTE: We have to check the entire grid, not just the tile becuse this tile might be part of a win.
        //       The current check win in row/col/diag only goes in 1 direction; it would need upgrading to do all directions

        // NOTE: We don't have logic of going towards a winning condition

        // Always choose wins over blocks, and if none then an open tile (also choose randomly)
        if (winningMoves.Count > 0)
        {
            Debug.Log($"Computer Player choosing a winning move; WinningMoves[{winningMoves.Count}] BlockingMoves[{blockingMoves.Count}] OpenMoves[{openMoves.Count}]");

            return winningMoves[Random.Range(0, winningMoves.Count)];
        }

        if (blockingMoves.Count > 0)
        {
            Debug.Log($"Computer Player choosing a blocking move; WinningMoves[{winningMoves.Count}] BlockingMoves[{blockingMoves.Count}] OpenMoves[{openMoves.Count}]");

            return blockingMoves[Random.Range(0, blockingMoves.Count)];
        }

        if (openMoves.Count > 0)
        {
            Debug.Log($"Computer Player choosing an open move; WinningMoves[{winningMoves.Count}] BlockingMoves[{blockingMoves.Count}] OpenMoves[{openMoves.Count}]");

            return openMoves[Random.Range(0, openMoves.Count)];
        }

        // Should never get in this state
        return null;
    }

    private IEnumerator ComputerWaitCoroutine()
    {
        Debug.Log("GameManager - ComputerWaitCoroutine - Start");

        yield return new WaitForSeconds(2);

        Debug.Log("GameManager - ComputerWaitCoroutine - AfterWait");

        computerMoveCoroutine = null;

        var computerMove = GetComputerMove(CurrentPlayer, NonCurrentPlayer);
        PlacePlayerTile(CurrentPlayer, computerMove);
    }

    #endregion

    #region Battle Input Methods

    private void OnTileClicked(TileInstance tile)
    {
        Debug.Log("GameManager - OnTileClicked");

        if (CurrentBattleState == BattleState.Initializing ||
            CurrentBattleState == BattleState.Finished)
        {
            return;
        }

        // TODO: Check if we are currently waiting in the Coroutine
        if (computerMoveCoroutine != null)
        {
            return;
        }

        PlacePlayerTile(CurrentPlayer, tile);
    }

    private bool OnUndoLastTurn()
    {
        // TODO: Possible UI Button to undo the last test easier

        return false;
    }

    #endregion
}
