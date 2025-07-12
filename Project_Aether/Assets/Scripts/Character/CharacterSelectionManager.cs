using ProjectAether.Objects.Net._2._1.Standard.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Character
{
    public class CharacterSelectionManager : MonoBehaviour
    {
        public static CharacterSelectionManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField]
        public UnityEngine.GameObject characterPanelPrefab;
        public Transform contentParent;

        [Header("Character Data")]
        public List<PlayerCharacter> availableCharacters;

        [Header("Selected Character Display (Optional)")]
        [SerializeField]
        public TextMeshProUGUI selectedCharacterNameText;
        public Image selectedCharacterAvatarImage;

        private PlayerCharacter selectedCharacter;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadCharacters();
            // Optionally select the first character by default
            if (availableCharacters.Count > 0)
            {
                SelectCharacter(availableCharacters[0]);
            }
        }

        private async Task LoadCharacters()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            var response = await ProjectAetherBackendApi.GetPlayerCharacters();
            
            availableCharacters = response.characters;
            foreach (PlayerCharacter character in availableCharacters)
            {
                UnityEngine.GameObject characterPanel = Instantiate(characterPanelPrefab, contentParent);
                CharacterPanelUI panelUI = characterPanel.GetComponent<CharacterPanelUI>();
                if (panelUI != null)
                {
                    panelUI.SetCharacterData(character);
                    panelUI.selectButton.onClick.AddListener(() => SelectCharacter(character));
                }
            }

            // Force Layout Rebuild if using Layout Groups, especially important for dynamic content
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        }

        public void SelectCharacter(PlayerCharacter character)
        {
            selectedCharacter = character;

            Debug.Log($"Selected character: {character.Name}");

            // TODO: character selected here.
        }   

        private void ConfirmSelection()
        {
            if (selectedCharacter != null)
            {
                Debug.Log($"Confirmed selection: {selectedCharacter.Name}");
                // Here you can add logic to proceed with the selected character, e.g., starting the game
                // TODO: SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.LogWarning("No character selected!");
            }
        }

        public void NextCharacter()
        {
            int currentIndex = availableCharacters.IndexOf(selectedCharacter);
            if (currentIndex < availableCharacters.Count - 1)
            {
                SelectCharacter(availableCharacters[currentIndex + 1]);
            }
            else
            {
                SelectCharacter(availableCharacters[0]); // Loop back to the first character
            }
        }

        public void PreviousCharacter()
        {
            int currentIndex = availableCharacters.IndexOf(selectedCharacter);
            if (currentIndex > 0)
            {
                SelectCharacter(availableCharacters[currentIndex - 1]);
            }
            else
            {
                SelectCharacter(availableCharacters[availableCharacters.Count - 1]); // Loop back to the last character
            }
        }
    }
}
