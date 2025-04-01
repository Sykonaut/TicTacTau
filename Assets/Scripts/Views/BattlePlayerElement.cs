using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerElement : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private TMP_Text characterNameText;

    [SerializeField]
    private Image characterImage;

    [SerializeField]
    private RectTransform playerTurnParent;

    [SerializeField]
    private RectTransform playerWinsParent;

    [SerializeField]
    private RectTransform playerLosesParent;

    [SerializeField]
    private RectTransform playerTiesParent;

    public PlayerInstance Player { get; private set; }

    #endregion

    public void Initialize(PlayerInstance player)
    {
        Player = player;

        playerNameText.text = Player.PlayerName;
        characterNameText.text = Player.PlayerCharacter.CharacterName;
        characterImage.sprite = Player.PlayerCharacter.CharacterSprite;
        characterImage.color = Player.PlayerColor;

        playerWinsParent.gameObject.SetActive(false);
        playerLosesParent.gameObject.SetActive(false);
        playerTiesParent.gameObject.SetActive(false);

        SetPlayerTurn(false);
    }

    public void SetPlayerTurn(bool isTurn)
    {
        playerTurnParent.gameObject.SetActive(isTurn);
    }

    public void SetWinState(PlayerInstance winningPlayer)
    {
        playerWinsParent.gameObject.SetActive(winningPlayer == Player);
        playerLosesParent.gameObject.SetActive(winningPlayer != null && winningPlayer != Player);
        playerTiesParent.gameObject.SetActive(winningPlayer == null);
    }
}
