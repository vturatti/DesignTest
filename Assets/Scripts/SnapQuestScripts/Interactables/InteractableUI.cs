using System;
using Code.Interactables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
    public Image InteractableImage;
    public TextMeshProUGUI Text;
    public Sprite DefaultSprite;

    public void Start()
    {
        DisableUI();
    }

    public void EnableUI()
    {
        InteractableImage.enabled = true;
        Text.enabled = true;
    }

    public void DisableUI()
    {
        InteractableImage.enabled = false;
        Text.enabled = false;
    }

    public void SetImageAndText(
        string specialTextToShow,
        Sprite specialSpriteToShow = null)
    {
        Text.text = specialTextToShow;
        InteractableImage.sprite = specialSpriteToShow == null ? DefaultSprite : specialSpriteToShow;
    }
}