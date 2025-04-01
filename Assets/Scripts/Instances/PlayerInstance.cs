using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Standard class for the selected values of a given player (player 1 or player 2)
/// </summary>
public class PlayerInstance
{
    public string PlayerName;

    public Color PlayerColor;

    public PlayerTemplate PlayerType;

    public CharacterTemplate PlayerCharacter;

    public PlayerInstance(GameConfigTemplate.PlayerDefaultValues defaultValues)
    {
        PlayerName = defaultValues.DefaultName; 
        PlayerColor = defaultValues.DefaultColor;
        PlayerType = defaultValues.DefaultPlayer;
        PlayerCharacter = defaultValues.DefaultCharacter;
    }
}
