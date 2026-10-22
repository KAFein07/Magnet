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


    [Header("ショップ")]
    public int trophyCost = 1;
    public Sprite icon;

    [Header("ゲーム内エフェクト")]
    [Tooltip("Particle Systemを使う場合に設定")]
    public ParticleSystem particlePrefab;

    [Tooltip("Trail Rendererを使う場合に設定")]
    public TrailRenderer trailPrefab;
}