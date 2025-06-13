using UnityEngine;

public class TestSkinnedMeshRendererBones : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRendererPrefab;
    [SerializeField]
    private SkinnedMeshRenderer originalSkinnedMeshRenderer;
    [SerializeField]
    private Transform rootBone;

    private void Start()
    {
        SkinnedMeshRenderer spawnedSkinnedMeshRenderer = Instantiate(skinnedMeshRendererPrefab, transform);
        spawnedSkinnedMeshRenderer.bones = originalSkinnedMeshRenderer.bones;
        spawnedSkinnedMeshRenderer.rootBone = rootBone;
    }
}
