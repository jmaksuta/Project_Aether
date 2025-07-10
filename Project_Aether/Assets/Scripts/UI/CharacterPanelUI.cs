using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectAether.Objects.Models;
using Microsoft.Extensions.Options;
using System;
using Assets.Scripts.Character;
public class CharacterPanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    public Image avatarImage;
    [SerializeField]
    public TextMeshProUGUI nameText;
    [SerializeField]
    public TextMeshProUGUI archetypeNameText;
    [SerializeField]
    public TextMeshProUGUI archetypeDescriptionText;
    [SerializeField]
    public TextMeshProUGUI healthText;
    [SerializeField]
    public TextMeshProUGUI magicText;
    [SerializeField]
    public TextMeshProUGUI experienceText;
    [SerializeField]
    public TextMeshProUGUI goldText;

    private PlayerCharacter characterData;

    [SerializeField]
    public Button nextButton;
    [SerializeField]
    public Button prevButton;

    [SerializeField]
    public Button selectButton;

    public void SetCharacterData(PlayerCharacter characterData)
    {
        this.characterData = characterData;

        if (avatarImage != null)
        {
            // Assuming you have a method to load the avatar image from character data
            avatarImage.sprite = LoadAvatarImage(characterData.profilePictureId);
        }
        if (nameText != null)
        {
            nameText.text = characterData.Name;
        }
        if (archetypeNameText != null)
        {
            archetypeNameText.text = characterData.ArchetypeDefinition.Name;
        }
        if (archetypeDescriptionText != null)
        {
            archetypeDescriptionText.text = characterData.ArchetypeDefinition.Description;
        }
        if (healthText != null)
        {
            healthText.text = $"Health: {characterData.Health}";
        }
        if (magicText != null)
        {
            magicText.text = $"Magic: {characterData.Mana}";
        }
        if (experienceText != null)
        {
            experienceText.text = $"Experience: {characterData.Experience}";
        }
        //if (goldText != null)
        //{
        //    goldText.text = $"Gold: {characterData.gold}";
        //}
    }

    private Sprite LoadAvatarImage(string profilePictureId)
    {
        // TODO: load avatar Image
        throw new NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(CharacterSelectionManager.Instance.NextCharacter);
        }
        if (prevButton != null)
        {
            prevButton.onClick.AddListener(CharacterSelectionManager.Instance.PreviousCharacter);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelectCharacter()
    {
        Debug.Log($"Selected character: {characterData.Name}");
        CharacterSelectionManager.Instance.SelectCharacter(characterData);
    }
}
