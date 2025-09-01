using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShortcutDonut/Preference")]
public class G_ShortcutDonutPreference : ScriptableObject
{
    public enum ShortcutDonutAngleMode {TopJunction,TopCentor}
    public ShortcutDonutAngleMode angleMode = ShortcutDonutAngleMode.TopCentor;
    public float expansionDuration = 0.25f,alphaDuration=0.25f, adurationSpeed = 5f;
    public float gamePadThreshold = 0.5f, mouseThreshold = 1f;
    public float examineShowDuration = 1f;
    [Range(0f,0.2f)]public float imageSize=0.05f;
    public Color normalColor = Color.gray, selectingColor = Color.white;
    private void OnValidate()
    {
        gamePadThreshold = Mathf.Max(gamePadThreshold, 0.01f);
    }
}
