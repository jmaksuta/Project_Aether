using UnityEngine;

public class CheckSkinnedMeshBones : MonoBehaviour
{
    private Transform[] _bones;

    [SerializeField] //[ReadOnly]
    public Transform[] Bones {
        get
        {
            return _bones;
        }
    }

    void Start()
    {
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();

        // If the SkinnedMeshRenderer is on a child object, use GetComponentInChildren
        if (smr == null)
        {
            smr = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        if (smr != null)
        {
            Debug.Log($"[CheckBones] Found SkinnedMeshRenderer on {smr.gameObject.name}");

            this._bones = smr.bones;    

            if (smr.bones != null)
            {
                Debug.Log($"[CheckBones] Bones array size: {smr.bones.Length}");

                if (smr.bones.Length > 0)
                {
                    for (int i = 0; i < smr.bones.Length; i++)
                    {
                        if (smr.bones[i] != null)
                        {
                            Debug.Log($"[CheckBones] Bone Element {i}: {smr.bones[i].name}");
                        }
                        else
                        {
                            Debug.LogWarning($"[CheckBones] Bone Element {i}: NULL reference!");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("[CheckBones] Bones array is empty!");
                }
            }
            else
            {
                Debug.LogError("[CheckBones] Bones array itself is NULL! (This is highly unusual)");
            }

            // Also check rootBone just in case
            if (smr.rootBone != null)
            {
                Debug.Log($"[CheckBones] Root Bone: {smr.rootBone.name}");
            }
            else
            {
                Debug.LogWarning($"[CheckBones] Root Bone is NULL for {smr.gameObject.name}");
            }

        }
        else
        {
            Debug.LogError("[CheckBones] No SkinnedMeshRenderer found on this GameObject or its children.");
        }
    }
}