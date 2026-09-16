using UnityEngine;

/*
 * Highlightable toggles the authored selection shader flag on renderers
 * under this object. PlayerController owns when target focus changes; this class
 * only applies the local cosmetic state.
 */

public class Highlightable : MonoBehaviour
{
    static readonly int SelectionEnabledId = Shader.PropertyToID("_Selection_Enabled");
    Renderer[] _targetRenderers;

    void Awake()
    {
        // TODO Slice 3.1: cache every child renderer and begin unselected.
        _targetRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);


        SetHighlighted(false);
    }

    public void SetHighlighted(bool isHighlighted)

    {

        // PROVIDED Slice 3.2: set _Highlight_Enabled on each material that supports it. </> end of Slice 3
        float highlightEnabled = isHighlighted ? 1f : 0f;
        foreach (Renderer targetRenderer in _targetRenderers)
        {
            Material[] materials = targetRenderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                if (!material.HasProperty(SelectionEnabledId)) continue;
                material.SetFloat(SelectionEnabledId, highlightEnabled);
            }
        }
    }
}
