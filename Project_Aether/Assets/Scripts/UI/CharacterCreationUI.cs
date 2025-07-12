using ProjectAether.Objects.Net._2._1.Standard.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    Button characterButton;
    [SerializeField]
    Button bodyButton;
    [SerializeField]
    Button finishButton;

    [SerializeField]
    UnityEngine.GameObject characterPanel;
    [SerializeField]
    UnityEngine.GameObject bodyPanel;

    List<ArchetypeDefinition> availableClasses;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async Task StartAsync()
    {
        availableClasses = await ProjectAetherBackendApi.GetAvailableClasses();
    }

    private void Start()
    {
        if (characterButton != null)
        {
            characterButton.onClick.AddListener(OnCharacterButtonClicked);
        }
        if (bodyButton != null)
        {
            bodyButton.onClick.AddListener(OnBodyButtonClicked);
        }
        if (finishButton != null)
        {
            finishButton.onClick.AddListener(OnFinishButtonClicked);
        }   
    }

    private void OnCharacterButtonClicked()
    {
        if (characterPanel != null)
        {
            characterPanel.SetActive(true);
        }
    }

    private void OnBodyButtonClicked()
    {
        if (bodyPanel != null)
        {
            bodyPanel.SetActive(true);
        }   
    }

    private void OnFinishButtonClicked()
    {
        // TODO: finalize and create character here.
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {

    }


}
