
using UnityEngine;

public interface ISelectable
{
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