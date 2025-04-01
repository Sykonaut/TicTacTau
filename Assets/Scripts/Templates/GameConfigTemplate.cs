using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfigTemplate : ScriptableObject
{
    /// <summary>
    /// Data for the default/initial values of a player at startup; These can be overidden in the lobby scene.
    /// </summary>
    [Serializable]
    public class PlayerDefaultValues
    {
        public string DefaultName;
        public Color DefaultColor;
        public PlayerTemplate DefaultPlayer;
        public CharacterTemplate DefaultCharacter;
    }

    #region Variables

    [Header("Level Settings")]

    [SerializeField]
    private LevelTemplate[] levelTemplates;

    [SerializeField]
    private LevelTemplate defaultLevelTemplate;

    [Header("Character Settings")]

    [SerializeField]
    private CharacterTemplate[] characterTemplates;

    [Header("Player Settings")]

    [SerializeField]
    private PlayerTemplate playerHumanTemplate;

    [SerializeField]
    private PlayerTemplate playerComputerTemplate;

    [SerializeField]
    private PlayerDefaultValues player1DefaultValues;

    [SerializeField]
    private PlayerDefaultValues player2DefaultValues;

    #endregion

    #region Properties

    public IEnumerable<LevelTemplate> LevelTemplates => levelTemplates;

    public LevelTemplate DefaultLevelTemplate => defaultLevelTemplate;

    public IEnumerable<CharacterTemplate> CharacterTemplates => characterTemplates;

    public PlayerTemplate HumanTemplate => playerHumanTemplate;
    public PlayerTemplate ComputerTemplate => playerComputerTemplate;

    public PlayerDefaultValues Player1DefaultValues => player1DefaultValues;

    public PlayerDefaultValues Player2DefaultValues => player2DefaultValues;

    #endregion

}
