using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data for selectable Greek character/letter such as Alpha, Beta, Gamma, Tau, etc.
/// </summary>
[CreateAssetMenu(fileName = "CharacterTemplate", menuName = "ScriptableObjects/CharacterTemplate", order = 1)]
public class CharacterTemplate : ScriptableObject
{
    #region Variables

    [SerializeField]
    private string characterName;   // Ex. "Tau" or "Alpha"

    [SerializeField]
    private Sprite characterSprite;

    #endregion

    #region Definitions

    public string CharacterName => characterName;

    public Sprite CharacterSprite => characterSprite;

    #endregion
}
