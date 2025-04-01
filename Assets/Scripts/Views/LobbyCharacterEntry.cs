using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterEntry : MonoBehaviour
{
    [SerializeField]
    private Image characterImage;

    private Action<LobbyCharacterEntry> onEntryClickedCallback;

    public CharacterTemplate Template { get; private set; }
    public PlayerInstance Player { get; private set; }

    public void Initialize(CharacterTemplate template, Action<LobbyCharacterEntry> OnEntryClickedCallback)
    {
        Template = template;
        onEntryClickedCallback = OnEntryClickedCallback;

        characterImage.sprite = template.CharacterSprite;
    }

    public void SetSelected(PlayerInstance player)
    {
        Player = player;

        if (player == null)
        {
            characterImage.color = Color.white;
        }
        else
        {
            characterImage.color = player.PlayerColor;
        }
    }

    public void OnEntryClicked()
    {
        if (onEntryClickedCallback != null)
        {
            onEntryClickedCallback(this);
        }
    }
}
