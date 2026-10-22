using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectShopManager : MonoBehaviour
{
    [Header("エフェクト一覧")]
    [SerializeField]
    private List<EffectData> effects = new List<EffectData>();

    [Header("ショップUI")]
    [SerializeField] private Transform itemParent;
    [SerializeField] private GameObject itemTemplate;
    [SerializeField] private TMP_Text trophyCountText;

    private readonly List<EffectShopItem> shopItems =
        new List<EffectShopItem>();

    private const string UnlockedKey = "EffectUnlocked_";
    private const string EquippedKey = "EquippedEffect";

    // TrophyManagerから所持数を取得
    public int TrophyCount => TrophyManager.TrophyCount;

    public string EquippedEffectId =>
        PlayerPrefs.GetString(EquippedKey, "");

    private void Start()
    {
        BuildShop();
        RefreshShop();
    }

    // テンプレートを複製してショップのアイテムを作成
    private void BuildShop()
    {
        if (itemParent == null || itemTemplate == null)
        {
            Debug.LogError(
                "Item ParentまたはItem Templateが未設定です。"
            );
            return;
        }

        shopItems.Clear();
        itemTemplate.SetActive(false);

        foreach (EffectData data in effects)
        {
            if (data == null)
                continue;

            GameObject itemObject = Instantiate(
                itemTemplate,
                itemParent
            );

            itemObject.name = "ShopItem_" + data.effectId;
            itemObject.SetActive(true);

            EffectShopItem item =
                itemObject.GetComponent<EffectShopItem>();

            if (item == null)
            {
                Debug.LogError(
                    "UIItemTemplateにEffectShopItemがありません。"
                );

                Destroy(itemObject);
                continue;
            }

            item.Setup(data, this);
            shopItems.Add(item);
        }
    }

    // 開放済みか確認
    public bool IsUnlocked(EffectData data)
    {
        if (data == null)
            return false;

        return PlayerPrefs.GetInt(
            UnlockedKey + data.effectId, 0
        ) == 1;
    }

    // 装備中か確認
    public bool IsEquipped(EffectData data)
    {
        return data != null &&
               EquippedEffectId == data.effectId;
    }

    // トロフィーを消費してエフェクトを開放
    public void UnlockEffect(EffectData data)
    {
        if (data == null || IsUnlocked(data))
            return;

        if (data.trophyCost < 0)
            return;

        if (!TrophyManager.TrySpendTrophies(data.trophyCost))
        {
            Debug.Log("トロフィーが足りません！");
            RefreshShop();
            return;
        }

        PlayerPrefs.SetInt(
            UnlockedKey + data.effectId, 1
        );

        PlayerPrefs.Save();

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

        RefreshShop();
    }

    // 装備中のEffectDataを取得
    public EffectData GetEquippedEffectData()
    {
        return effects.Find(data =>
            data != null &&
            data.effectId == EquippedEffectId
        );
    }

    // 所持トロフィー数とアイテム表示を更新
    public void RefreshShop()
    {
        if (trophyCountText != null)
            trophyCountText.text = TrophyCount.ToString();

        foreach (EffectShopItem item in shopItems)
        {
            if (item != null)
                item.Refresh();
        }
    }

    // ショップ状況をリセット
    private void ResetShopData()
    {
        // トロフィー数を0にする
        PlayerPrefs.SetInt("PlayerTrophyCount", 0);

        // 装備中のエフェクトを解除
        PlayerPrefs.DeleteKey("EquippedEffect");

        // 解放状態をリセット
        foreach (EffectData data in effects)
        {
            if (data != null)
            {
                PlayerPrefs.DeleteKey("EffectUnlocked_" + data.effectId);
            }
        }

        PlayerPrefs.Save();

        // ショップ表示を更新
        RefreshShop();
        Debug.Log("ショップ状況をリセットしました");
    }
}