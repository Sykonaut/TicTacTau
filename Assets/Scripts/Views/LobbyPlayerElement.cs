using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerElement : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField playerNameInput;

    [SerializeField]
    private TMP_Text characterNameText;

    [SerializeField]
    private Image characterImage;

    [SerializeField]
    private Toggle humanToggle;

    [SerializeField]
    private Toggle computerToggle;

    [SerializeField]
    private GameObject selectedObject;

    public PlayerInstance Player { get; private set; }

    private PlayerTemplate humanPlayer;
    private PlayerTemplate computerPlayer;

    private Action<LobbyPlayerElement> onElementClickedCallback;

    public void Initialize(PlayerInstance player, PlayerTemplate humanPlayerTemplate, PlayerTemplate computerPlayerTemplate, Action<LobbyPlayerElement> OnElementClickedCallback)
    {
        Player = player;
        
        humanPlayer = humanPlayerTemplate;
        computerPlayer = computerPlayerTemplate;

        onElementClickedCallback = OnElementClickedCallback;

        Refresh();
    }

    public void Refresh()
    {
        playerNameInput.text = Player.PlayerName;

        characterNameText.text = Player.PlayerCharacter.CharacterName;
        characterImage.sprite = Player.PlayerCharacter.CharacterSprite;
        characterImage.color = Player.PlayerColor;

        humanToggle.SetIsOnWithoutNotify(Player.PlayerType.PlayerBehaviorType == PlayerTemplate.PlayerBehavior.Human);
        computerToggle.SetIsOnWithoutNotify(Player.PlayerType.PlayerBehaviorType == PlayerTemplate.PlayerBehavior.Computer);
    }

    public void SetSelected(bool selected)
    {
        selectedObject.SetActive(selected);
    }

    public void OnElementClicked()
    {
        if (onElementClickedCallback != null)
        {
            onElementClickedCallback(this);
        }
    }

    public void OnPlayerNameInputField(string fieldName)
    {
        if (!string.IsNullOrEmpty(fieldName)) 
        {
            Player.PlayerName = fieldName;
        }
    }

    public void OnHumanToggle()
    {
        Player.PlayerType = humanPlayer;

        Refresh();
    }

    public void OnComputerToggle()
    {
        Player.PlayerType = computerPlayer;

        Refresh();
    }
}
