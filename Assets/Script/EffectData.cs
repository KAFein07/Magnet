
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEffect",
    menuName = "Game/Effect Data"
)]
public class EffectData : ScriptableObject
{
    [Header("基本情報")]
    public string effectId;
    public string effectName;

    [TextArea]
    public string description;

    [Header("ショップ")]
    public int trophyCost = 1;
    public Sprite icon;

    [Header("ゲーム内エフェクト")]
    public GameObject effectPrefab;
}
