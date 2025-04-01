using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLevelEntry : MonoBehaviour
{
    [SerializeField]
    private Image levelImage;

    [SerializeField]
    private TMP_Text levelNameText;

    [SerializeField]
    private Image selectedImage;

    private Action<LobbyLevelEntry> onEntryClickedCallback;

    public LevelTemplate Template { get; private set; }

    public void Initialize(LevelTemplate levelTemplate, Action<LobbyLevelEntry> OnEntryClickedCallback)
    {
        Template = levelTemplate;
        onEntryClickedCallback = OnEntryClickedCallback;

        levelImage.sprite = levelTemplate.Icon;
        levelNameText.text = levelTemplate.DisplayName;
    }

    public void SetSelected(bool selected)
    {
        if (selectedImage != null)
        {
            selectedImage.gameObject.SetActive(selected);
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
