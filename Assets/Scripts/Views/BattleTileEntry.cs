using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTileEntry : MonoBehaviour
{
    [SerializeField]
    private Image characterImage;

    [SerializeField]
    private Sprite emptyTileSprite;

    [SerializeField]
    private RectTransform winStateParent;

    [SerializeField]
    private Image winStateImage;

    private Action<BattleTileEntry> onTileClickedCallback;

    public TileInstance Tile { get; private set; }

    public void Initialize(TileInstance tile, Action<BattleTileEntry> OnTileClickedCallback)
    {
        Tile = tile;
        onTileClickedCallback = OnTileClickedCallback;

        Refresh();

        SetWinState(false);
    }

    public void Refresh()
    {
        if (Tile.Player != null)
        {
            characterImage.sprite = Tile.Player.PlayerCharacter.CharacterSprite;
            characterImage.color = Tile.Player.PlayerColor;

            winStateImage.color = Tile.Player.PlayerColor;
        }
        else
        {
            characterImage.sprite = emptyTileSprite;
            characterImage.color = Color.white;

            winStateImage.color = Color.white;
        }
    }

    public void SetWinState(bool winState)
    {
        winStateParent.gameObject.SetActive(winState);
    }

    public void OnTileClicked()
    {
        if (onTileClickedCallback != null)
        {
            onTileClickedCallback(this);
        }
    }
}
