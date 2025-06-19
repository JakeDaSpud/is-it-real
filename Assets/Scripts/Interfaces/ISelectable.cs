
using UnityEngine;

public interface ISelectable
{
    protected Material m_Original_Material { get; set; }
    protected Material m_Highlighted_Material { get; set; }
    protected Material m_Selected_Material { get; set; }

    public bool IsSelected { get; set; }
    public bool IsHighlighted { get; set; }

    /// <summary>
    /// Reverts this GameObject to its original Material
    /// </summary>
    public void BecomeOriginal();

    /// <summary>
    /// Sets this GameObject as highlighted
    /// </summary>
    public void BecomeHighlighted();

    /// <summary>
    /// Sets this GameObject as selected
    /// </summary>
    public void BecomeSelected();
}