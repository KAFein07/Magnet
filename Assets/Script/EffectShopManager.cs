
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectShopManager : MonoBehaviour
{
    [Header("エフェクト一覧")]
    [SerializeField]
    private List<EffectData> effects =
        new List<EffectData>();

    [Header("ショップUI")]
    [SerializeField] private Transform itemParent;
    [SerializeField] private EffectShopItem itemPrefab;
    [SerializeField] private TMP_Text trophyCountText;

    private readonly List<EffectShopItem> shopItems =
        new List<EffectShopItem>();

    private const string UnlockedKey = "EffectUnlocked_";
    private const string EquippedKey = "EquippedEffect";

    public int TrophyCount => GameClearManager.TrophyCount;

    public string EquippedEffectId =>
        PlayerPrefs.GetString(EquippedKey, "");

    private void Start()
    {
        BuildShop();
        RefreshShop();
    }

    // エフェクト一覧をテンプレートから自動生成
    private void BuildShop()
    {
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }

        shopItems.Clear();

        foreach (EffectData data in effects)
        {
            if (data == null)
                continue;

            EffectShopItem item = Instantiate(
                itemPrefab,
                itemParent
            );

            item.Setup(data, this);
            shopItems.Add(item);

            item.gameObject.SetActive(true);
        }
    }

    // このエフェクトを開放済みか確認
    public bool IsUnlocked(EffectData data)
    {
        return PlayerPrefs.GetInt(
            UnlockedKey + data.effectId, 0
        ) == 1;
    }

    // このエフェクトを装備中か確認
    public bool IsEquipped(EffectData data)
    {
        return EquippedEffectId == data.effectId;
    }

    // トロフィーを消費して永久開放
    public void UnlockEffect(EffectData data)
    {
        if (data == null || IsUnlocked(data))
            return;

        if (data.trophyCost < 0)
            return;

        if (!GameClearManager.TrySpendTrophies(data.trophyCost))
        {
            Debug.Log("トロフィーが足りません！");
            return;
        }

        PlayerPrefs.SetInt(
            UnlockedKey + data.effectId, 1
        );

        PlayerPrefs.Save();

        Debug.Log(data.effectName + "を開放しました！");
        RefreshShop();
    }

    // 開放済みエフェクトを装備
    public void EquipEffect(EffectData data)
    {
        if (data == null || !IsUnlocked(data))
            return;

        PlayerPrefs.SetString(
            EquippedKey, data.effectId
        );

        PlayerPrefs.Save();

        Debug.Log(data.effectName + "を装備しました！");
        RefreshShop();
    }

    // トロフィー数と各ボタンを更新
    public void RefreshShop()
    {
        if (trophyCountText != null)
        {
            trophyCountText.text = TrophyCount.ToString();
        }

        foreach (EffectShopItem item in shopItems)
        {
            if (item != null)
                item.Refresh();
        }
    }
}
