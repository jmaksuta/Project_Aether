using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private SkinnedMeshRenderer originalSkinnedMeshRenderer;

    private List<Animator> animatorInstances;

    [SerializeField]
    private Transform characterMeshesParent;

    private void Awake()
    {
        initialize();

        // Call LoadCustomization() here so the character loads its saved state.
        LoadCustomization();
    }

    private void initialize()
    {
        characterAnimator = GetComponentInChildren<Animator>(); // Finds the animator on the base skeleton
        originalSkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

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

        if (animatorInstances == null)
        {
            animatorInstances = new List<Animator>();
        }
        initializeDefaultParts();
    }


    public void SaveCustomization()
    {
        CharacterAppearanceData data = new CharacterAppearanceData();

        // 1. Save Modular Part Indices
        // Get the index of the currently active part within its respective list
        // Use .IndexOf() to find the position of the current GameObject in the list of available prefabs.
        data.headIndex = (currentHead != null && availableHeads.Contains(currentHead)) ? availableHeads.IndexOf(currentHead) : -1;
        data.torsoIndex = (currentTorso != null && availableTorsos.Contains(currentTorso)) ? availableTorsos.IndexOf(currentTorso) : -1;
        data.legsIndex = (currentLegs != null && availableLegs.Contains(currentLegs)) ? availableLegs.IndexOf(currentLegs) : -1;
        // TODO: Hands logic here.
        //data.handsIndex = (currentHands != null && availableHands.Contains(currentHands)) ? availableHands.IndexOf(currentHands) : -1;
        data.feetIndex = (currentFeet != null && availableFeet.Contains(currentFeet)) ? availableFeet.IndexOf(currentFeet) : -1;
        // ... populate other indices as needed

        // 2. Save Blend Shape Values (if implemented)
        // This assumes your character has a main SkinnedMeshRenderer (e.g., on the 'Torso' or 'BodyBase')
        // that contains all the blend shapes.
        // You'd need to adapt this based on how your blend shapes are structured.
        SkinnedMeshRenderer mainBodyRenderer = null;
        if (currentTorso != null)
        {
            mainBodyRenderer = currentTorso.GetComponent<SkinnedMeshRenderer>();
        }
        else if (currentHead != null) // Fallback if blend shapes are on head or other part
        {
            mainBodyRenderer = currentHead.GetComponent<SkinnedMeshRenderer>();
        }
        // ... find the correct renderer that holds your blend shapes

        if (mainBodyRenderer != null && mainBodyRenderer.sharedMesh != null)
        {
            for (int i = 0; i < mainBodyRenderer.sharedMesh.blendShapeCount; i++)
            {
                string blendShapeName = mainBodyRenderer.sharedMesh.GetBlendShapeName(i);
                float blendShapeWeight = mainBodyRenderer.GetBlendShapeWeight(i);
                data.blendShapeValues.Add(new CharacterAppearanceData.BlendShapeEntry(blendShapeName, blendShapeWeight));
            }
        }

        // 3. Save Material Colors (if implemented)
        // This requires you to have references to the materials you want to tint.
        // For simplicity, let's assume we have a way to get the active skin and hair materials.
        // For example, if your currentHead has a material with skin color and currentTorso has a material with hair color (unlikely, usually head has both)
        // You'd need specific logic to find the correct SkinnedMeshRenderer and its material.
        if (currentHead != null)
        {
            SkinnedMeshRenderer headRenderer = currentHead.GetComponent<SkinnedMeshRenderer>();
            if (headRenderer != null && headRenderer.material != null)
            {
                // Assuming the first material on the head is the skin material
                data.skinColorHex = ColorUtility.ToHtmlStringRGB(headRenderer.material.color);
                // If you have multiple materials on one mesh, you'd access headRenderer.materials[index]
            }
        }
        // Example for hair color, might need a separate component or logic to find hair mesh/material
        // if (hairMesh != null && hairMesh.material != null) {
        //    data.hairColorHex = ColorUtility.ToHtmlStringRGB(hairMesh.material.color);
        // }


        // 4. Serialize to JSON and Save to PlayerPrefs
        string json = JsonUtility.ToJson(data); // Convert the CharacterAppearanceData object to a JSON string
        PlayerPrefs.SetString("PlayerCharacterData", json); // Store the JSON string in PlayerPrefs
        PlayerPrefs.Save(); // Ensure the data is written to disk immediately

        Debug.Log("Character data saved: " + json);
    }

    // --- Loading Character Customization ---
    public void LoadCustomization()
    {
        // Check if saved data exists
        if (PlayerPrefs.HasKey("PlayerCharacterData"))
        {
            string json = PlayerPrefs.GetString("PlayerCharacterData");
            CharacterAppearanceData data = JsonUtility.FromJson<CharacterAppearanceData>(json); // Convert JSON string back to object

            Debug.Log("Character data loaded: " + json);

            // 1. Load Modular Parts
            // Equip parts based on the saved indices
            if (data.headIndex != -1 && data.headIndex < availableHeads.Count)
            {
                EquipPart(CharacterPartType.Head, availableHeads[data.headIndex]);
            }
            else
            {
                // Default if index is invalid or -1
                EquipPart(CharacterPartType.Head, availableHeads.Count > 0 ? availableHeads[0] : null);
            }

            if (data.torsoIndex != -1 && data.torsoIndex < availableTorsos.Count)
                EquipPart(CharacterPartType.Torso, availableTorsos[data.torsoIndex]);
            else
                EquipPart(CharacterPartType.Torso, availableTorsos.Count > 0 ? availableTorsos[0] : null);

            if (data.legsIndex != -1 && data.legsIndex < availableLegs.Count)
                EquipPart(CharacterPartType.Legs, availableLegs[data.legsIndex]);
            else
                EquipPart(CharacterPartType.Legs, availableLegs.Count > 0 ? availableLegs[0] : null);

            // TODO: Hands logic here
            //if (data.handsIndex != -1 && data.handsIndex < availableHands.Count)
            //    EquipPart(CharacterPartType.Hands, availableHands[data.handsIndex]);
            //else
            //    EquipPart(CharacterPartType.Hands, availableHands.Count > 0 ? availableHands[0] : null);

            if (data.feetIndex != -1 && data.feetIndex < availableFeet.Count)
                EquipPart(CharacterPartType.Feet, availableFeet[data.feetIndex]);
            else
                EquipPart(CharacterPartType.Feet, availableFeet.Count > 0 ? availableFeet[0] : null);
            // ... load other parts similarly

            // 2. Load Blend Shape Values (if implemented)
            // You'd need a method like this:
            // public void SetBlendShapeWeight(string blendShapeName, float weight)
            // Example:
            // if (mainBodyRenderer != null) { // Assuming mainBodyRenderer was found in SaveCustomization
            //     foreach (var entry in data.blendShapeValues)
            //     {
            //         // Find the blend shape index by name on the shared mesh
            //         int blendShapeIdx = mainBodyRenderer.sharedMesh.GetBlendShapeIndex(entry.name);
            //         if (blendShapeIdx != -1)
            //         {
            //             mainBodyRenderer.SetBlendShapeWeight(blendShapeIdx, entry.weight);
            //         }
            //     }
            // }

            // 3. Load Material Colors (if implemented)
            // Example:
            // Color loadedSkinColor;
            // if (ColorUtility.TryParseHtmlString("#" + data.skinColorHex, out loadedSkinColor))
            // {
            //     // You need a way to get the correct material reference.
            //     // E.g., a public Material variable for skin material, or find it on the active head/torso mesh.
            //     // skinMaterial.color = loadedSkinColor;
            // }
            // Color loadedHairColor;
            // if (ColorUtility.TryParseHtmlString("#" + data.hairColorHex, out loadedHairColor))
            // {
            //     // hairMaterial.color = loadedHairColor;
            // }
        }
        else
        {
            Debug.Log("No saved character data found. Initializing default appearance.");
            initializeDefaultParts(); // Call your default initialization method if no save exists
        }
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

            Animator partAnimator = newPartInstance.GetComponent<Animator>();
            if (partAnimator != null)
            {
                if (!animatorInstances.Contains(partAnimator))
                {
                    animatorInstances.Add(partAnimator);
                }
                // Option A (Recommended): Disable or remove the Animator component
                // For clothing/hair, it should not have its own Animator
                //Destroy(partAnimator); // Remove it completely at runtime
                // OR: partAnimator.enabled = false; // Just disable it
                //AnimatorController controller = partAnimator.GetComponent<AnimatorController>(); // = this.characterAnimator;
                //if (controller != null)
                //{

                //}


                Debug.LogWarning("Removed/Disabled Animator component on instantiated modular part: " + newPartPrefab.name +
                                 ". Modular parts usually don't need their own Animator.");
            }

            // --- IMPORTANT: Set the Animator Controller on the new part ---
            //Animator partAnimator = newPartInstance.GetComponent<Animator>();
            //if (partAnimator != null)
            //{
            //    if (characterAnimator != null && characterAnimator.runtimeAnimatorController != null)
            //    {
            //        // Assign the main character's Animator Controller to the new part's Animator
            //        partAnimator.runtimeAnimatorController = characterAnimator.runtimeAnimatorController;

            //        Debug.Log("Set Animator Controller of " + newPartPrefab.name + " to " + characterAnimator.runtimeAnimatorController.name);

            //        // Optionally, you might also want to set its Avatar, if the modular part has one and it's needed
            //        // For typical modular clothing/hair, this isn't usually necessary as they're skinned to the main rig
            //        // if (characterAnimator.avatar != null && partAnimator.avatar == null)
            //        // {
            //        //     partAnimator.avatar = characterAnimator.avatar;
            //        // }
            //    }
            //    else
            //    {
            //        Debug.LogWarning("Main character Animator or its Controller is null. Cannot assign controller to modular part: " + newPartPrefab.name);
            //    }
            //}
            // --- End of Animator Controller setup ---


            SkinnedMeshRenderer newRenderer = newPartInstance.GetComponentInChildren<SkinnedMeshRenderer>();//.GetComponent<SkinnedMeshRenderer>();
            if (newRenderer != null)
            {
                // TODO: this is causing it to render strange.
                Transform hipsBone = characterAnimator.GetBoneTransform(HumanBodyBones.Hips);
                if (hipsBone != null)
                {
                    newRenderer.rootBone = hipsBone;
                }
                else
                {
                    Debug.LogWarning("Could not find Hips bone for " + newPartPrefab.name + ". Automatic bone assignment might fail for this part.");
                }
                if (originalSkinnedMeshRenderer != null)
                { 
                    newRenderer.bones = originalSkinnedMeshRenderer.bones;
                }
                else
                {
                    Debug.LogWarning("Original Skinned Mesh Renerer not found.");
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
        animatorInstances.Clear();
    }

    public void SetAnimationValue(String animationVariable, bool value)
    {
        if (animatorInstances != null)
        {
            foreach (Animator animator in animatorInstances)
            {
                animator.SetBool(animationVariable, value);
            }
        }
    }

    public void SetAnimationValue(String animationVariable, float value)
    {
        if (animatorInstances != null)
        {
            foreach (Animator animator in animatorInstances)
            {
                animator.SetFloat(animationVariable, value);
            }
        }
    }

    public void SetAnimationValue(String animationVariable)
    {
        if (animatorInstances != null)
        {
            foreach (Animator animator in animatorInstances)
            {
                animator.SetTrigger(animationVariable);
            }
        }
    }
}
