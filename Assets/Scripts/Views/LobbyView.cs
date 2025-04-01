using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class LobbyView : MonoBehaviour
{
    #region Variables

    //--- Serialized ---

    [Header("Player References")]

    [SerializeField]
    private LobbyPlayerElement player1Element;

    [SerializeField]
    private LobbyPlayerElement player2Element;

    [Header("Character References")]

    [SerializeField]
    private LobbyCharacterEntry characterEntryPrefab;

    [SerializeField]
    private RectTransform characterEntryParent;

    [Header("Level References")]

    [SerializeField]
    private LobbyLevelEntry levelEntryPrefab;

    [SerializeField]
    private RectTransform levelEntryParent;

    [SerializeField]
    private TMP_Text levelDescriptionText;


    //--- Non-Serialized ---

    private LevelInstance level;
    private PlayerInstance player1;
    private PlayerInstance player2;
    private Action onPlayButtonCallback;

    private LobbyPlayerElement selectedPlayer;

    private List<LobbyCharacterEntry> lobbyCharacterEntries = new List<LobbyCharacterEntry>();

    private List<LobbyLevelEntry> lobbyLevelEntries = new List<LobbyLevelEntry>();
    private LevelTemplate selectedLevelTemplate;


    #endregion

    #region Methods

    public void Initialize(LevelInstance level,
        PlayerInstance player1,
        PlayerInstance player2,
        PlayerTemplate humanTemplate,
        PlayerTemplate computerTemplate,
        IEnumerable<CharacterTemplate> characterTemplates,
        IEnumerable<LevelTemplate> levelTemplates,
        Action OnPlayButtonCallback)
    {
        Debug.Log("LobbyView -> Initialize");

        // Cache values
        this.level = level;
        this.player1 = player1;
        this.player2 = player2;
        onPlayButtonCallback = OnPlayButtonCallback;

        InitializePlayerElements(humanTemplate, computerTemplate);
        InitializeCharacterEntries(characterTemplates);
        InitializeLevelEntries(levelTemplates);
    }

    private void InitializePlayerElements(PlayerTemplate humanTemplate, PlayerTemplate computerTemplate)
    {
        player1Element.Initialize(player1, humanTemplate, computerTemplate, OnPlayerClicked);
        player2Element.Initialize(player2, humanTemplate, computerTemplate, OnPlayerClicked);

        selectedPlayer = player1Element;

        RefreshSelectedPlayer();
    }

    private void InitializeCharacterEntries(IEnumerable<CharacterTemplate> characterTemplates)
    {
        foreach (var template in characterTemplates)
        {
            var characterEntry = Instantiate(characterEntryPrefab, characterEntryParent);

            characterEntry.Initialize(template, OnCharacterClicked);

            lobbyCharacterEntries.Add(characterEntry);
        }

        RefreshSelectedCharacters();
    }

    private void InitializeLevelEntries(IEnumerable<LevelTemplate> levelTemplates)
    {
        foreach (var template in levelTemplates)
        {
            var levelEntry = Instantiate(levelEntryPrefab, levelEntryParent);

            levelEntry.Initialize(template, OnLevelClicked);

            lobbyLevelEntries.Add(levelEntry);
        }

        RefreshSelectedLevel();
    }

    private void RefreshSelectedPlayer()
    {
        player1Element.SetSelected(selectedPlayer == player1Element);
        player2Element.SetSelected(selectedPlayer == player2Element);
    }

    private void RefreshSelectedLevel()
    {
        foreach (var levelEntry in lobbyLevelEntries)
        {
            levelEntry.SetSelected(levelEntry.Template == level.levelTemplate);
        }

        levelDescriptionText.text = level.levelTemplate.Description;
    }

    private void RefreshSelectedCharacters()
    {
        foreach (var characterEntry in lobbyCharacterEntries)
        {
            bool isPlayer1 = player1.PlayerCharacter == characterEntry.Template;
            bool isPlayer2 = player2.PlayerCharacter == characterEntry.Template;

            if (isPlayer1)
                characterEntry.SetSelected(player1);
            else if (isPlayer2)
                characterEntry.SetSelected(player2);
            else
                characterEntry.SetSelected(null);
        }
    }

    #endregion

    #region UI Events

    private void OnPlayerClicked(LobbyPlayerElement playerElement)
    {
        Debug.Log("LobbyView -> OnPlayerClicked: " + playerElement.Player.PlayerName);

        selectedPlayer = playerElement;

        RefreshSelectedPlayer();
    }

    private void OnCharacterClicked(LobbyCharacterEntry entry)
    {
        Debug.Log("LobbyView -> OnCharacterClicked: " + entry.Template.name);

        // Disallow clicking on the other player's character
        var otherPlayer = selectedPlayer.Player == player1 ? player2 : player1;
        if (entry.Template == otherPlayer.PlayerCharacter)
        {
            Debug.Log("Clicked on the other player's character. Cannot use the same character.");
            return;
        }

        // Set the selected player to the entry
        selectedPlayer.Player.PlayerCharacter = entry.Template;
        selectedPlayer.Refresh();

        RefreshSelectedCharacters();
    }

    private void OnLevelClicked(LobbyLevelEntry entry)
    {
        Debug.Log("LobbyView -> OnLevelClicked: " + entry.Template.name);

        level.levelTemplate = entry.Template;

        RefreshSelectedLevel();
    }

    public void OnPlayButton()
    {
        if (onPlayButtonCallback != null)
        {
            onPlayButtonCallback();
        }
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    #endregion
}
