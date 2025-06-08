using Assets.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterCustomizationManager : MonoBehaviour
{
    [Header("Character Part Slots")]
    public GameObject currentHead;
    public GameObject currentTorso;
    public GameObject currentLegs;
    public GameObject currentFeet;


    [Header("Available Modular Parts (Prefabs)")]

    public List<GameObject> availableHeads = new List<GameObject>();
    public List<GameObject> availableTorsos = new List<GameObject>();
    public List<GameObject> availableLegs = new List<GameObject>();
    public List<GameObject> availableFeet = new List<GameObject>();


    private Animator characterAnimator;

    [SerializeField]
    private Transform characterMeshesParent;

    private void Awake()
    {
        initialize();
    }

    private void initialize()
    {
        characterAnimator = GetComponentInChildren<Animator>(); // Finds the animator on the base skeleton
        if (characterMeshesParent == null)
        {
            characterMeshesParent = transform.Find("CharacterMeshes");
            if (characterMeshesParent == null)
            {
                // if styill null, create one for organization
                characterMeshesParent = new GameObject("CharacterMeshes").transform;
                characterMeshesParent.SetParent(this.transform);
                characterMeshesParent.position = this.transform.position;
                characterMeshesParent.rotation = this.transform.rotation;
                characterMeshesParent.localPosition = this.transform.localPosition;
                characterMeshesParent.localRotation = this.transform.localRotation;
                //characterMeshesParent.localPosition = Vector3.zero;
                //characterMeshesParent.localRotation = Quaternion.identity;
            }
        }

        initializeDefaultParts();
    }

    private void initializeDefaultParts()
    {
        if (availableHeads.Count > 0)
        {
            EquipPart(CharacterPartType.Head, availableHeads[0]);
        }
        if (availableTorsos.Count > 0)
        {
            EquipPart(CharacterPartType.Torso, availableTorsos[0]);
        }
        if (availableLegs.Count > 0)
        {
            EquipPart(CharacterPartType.Legs, availableLegs[0]);
        }
        if (availableFeet.Count > 0)
        {
            EquipPart(CharacterPartType.Feet, availableFeet[0]);
        }
    }

    public void EquipPart(CharacterPartType partType, GameObject newPartPrefab)
    {
        DestroyOldPartIfNecessary(partType);

        GameObject newPartInstance = null;

        InstantiateNewPart(ref newPartPrefab, ref newPartInstance);

        UpdateCurrentPart(partType, ref newPartInstance);

    }

    private void DestroyOldPartIfNecessary(CharacterPartType partType)
    {
        GameObject currentPartToDestroy = DetermineWhichPartToReplace(partType);

        DestroyOldPartIfExists(ref currentPartToDestroy);
    }

    private GameObject DetermineWhichPartToReplace(CharacterPartType partType)
    {
        GameObject currentPartToDestroy = null;
        switch (partType)
        {
            case CharacterPartType.Head:
                currentPartToDestroy = currentHead;
                break;
            case CharacterPartType.Torso:
                currentPartToDestroy = currentTorso;
                break;
            case CharacterPartType.Legs:
                currentPartToDestroy = currentLegs;
                break;
            case CharacterPartType.Feet:
                currentPartToDestroy = currentFeet;
                break;
            default:
                Debug.LogWarning("Attempted to equip unknown part type:" + partType);
                break;
        }
        return currentPartToDestroy;
    }

    private void DestroyOldPartIfExists(ref GameObject currentPartToDestroy)
    {
        if (currentPartToDestroy != null)
        {
            Destroy(currentPartToDestroy);
        }
    }

    private void InstantiateNewPart(ref GameObject newPartPrefab, ref GameObject newPartInstance)
    {
        if (newPartPrefab != null)
        {
            newPartInstance = Instantiate(newPartPrefab, characterMeshesParent);
            newPartInstance.transform.localPosition = Vector3.zero;
            newPartInstance.transform.localRotation = Quaternion.identity;
            newPartInstance.transform.localScale = Vector3.one;

            SkinnedMeshRenderer newRenderer = newPartInstance.GetComponent<SkinnedMeshRenderer>();
            if (newRenderer != null)
            {
                Transform hipsBone = characterAnimator.GetBoneTransform(HumanBodyBones.Hips);
                if (hipsBone != null)
                {
                    newRenderer.rootBone = hipsBone;
                }
                else
                {
                    Debug.LogWarning("Could not find Hips bone for " + newPartPrefab.name + ". Automatic bone assignment might fail for this part.");
                }
            }
            else
            {
                Debug.LogError("New Part prefab does not have a SkinnedMeshRenderer." + newPartPrefab.name);
            }
        }
    }

    private void UpdateCurrentPart(CharacterPartType partType, ref GameObject newPartInstance)
    {
        switch (partType)
        {
            case CharacterPartType.Head:
                currentHead = newPartInstance;
                break;
            case CharacterPartType.Torso:
                currentTorso = newPartInstance;
                break;
            case CharacterPartType.Legs:
                currentLegs = newPartInstance;
                break;
            case CharacterPartType.Feet:
                currentFeet = newPartInstance;
                break;
            default:
                break;
        }
    }


    public void SetNextHead()
    {
        SetNextPart(CharacterPartType.Head, ref currentHead);
    }

    public void SetNextTorso()
    {
        SetNextPart(CharacterPartType.Torso, ref currentTorso);
    }

    public void SetNextLegs()
    {
        SetNextPart(CharacterPartType.Legs, ref currentLegs);
    }

    public void SetNextFeet()
    {
        SetNextPart(CharacterPartType.Feet, ref currentFeet);
    }

    private List<GameObject> GetAvailablePartList(CharacterPartType partType)
    {
        switch (partType)
        {
            case CharacterPartType.Head:
                return availableHeads;
            case CharacterPartType.Torso:
                return availableTorsos;
            case CharacterPartType.Legs:
                return availableLegs;
            case CharacterPartType.Feet:
                return availableFeet;
            default:
                Debug.LogWarning("Unknown part type: " + partType);
                return new List<GameObject>();
        }
    }

    public void SetNextPart(CharacterPartType partType, ref GameObject currentPart)
    {
        List<GameObject> availableParts = GetAvailablePartList(partType);
        SetNextPart(partType, ref currentPart, availableParts);
    }


    public void SetNextPart(CharacterPartType partType, ref GameObject currentPart, List<GameObject> availableParts)
    {
        if (availableParts.Count == 0)
        {
            return;
        }
        int instanceId = currentPart.GetInstanceID();
        GameObject currentObj = currentPart;
        int currentIndex = availableParts.FindIndex(go =>
        {
            return go != null && go.name == currentObj.name.Replace("(Clone)", "").Replace("(Core)", "");

        });

        //availableParts.IndexOf(currentPart != null ? currentPart.gameObject : null);
        int nextIndex = (currentIndex + 1) % availableParts.Count;
        EquipPart(partType, availableParts[nextIndex]);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
