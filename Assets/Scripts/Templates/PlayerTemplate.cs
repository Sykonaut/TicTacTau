using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data for the defining human and computer players
/// </summary>
[CreateAssetMenu(fileName = "PlayerTemplate", menuName = "ScriptableObjects/PlayerTemplate", order = 1)]
public class PlayerTemplate : ScriptableObject
{
    #region Definitions

    public enum PlayerBehavior
    {
        Human,
        Computer
    }

    #endregion

    #region Variables

    [SerializeField]
    private PlayerBehavior playerBehaviorType;

    // TODO: Something for different types of Computer behavior (Easy, Medium, Hard)

    #endregion

    #region Definitions

    public PlayerBehavior PlayerBehaviorType => playerBehaviorType;

    #endregion

}
