using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectShopItem : MonoBehaviour
{
    [Header("表示")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text costText;

    [Header("ボタン")]
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button useButton;

    private EffectData effectData;
    private EffectShopManager shopManager;

    public void Setup(
        EffectData data,
        EffectShopManager manager
    )
    {
        effectData = data;
        shopManager = manager;

        // エフェクトのアイコンを設定
        if (iconImage != null)
            iconImage.sprite = data.icon;

        // 開放ボタンに必要なトロフィー数を表示
        if (costText != null)
            costText.text = data.trophyCost.ToString();

        // ボタンのイベントを設定
        unlockButton.onClick.RemoveAllListeners();
        useButton.onClick.RemoveAllListeners();

        unlockButton.onClick.AddListener(
            () => shopManager.UnlockEffect(effectData)
        );

        useButton.onClick.AddListener(
            () => shopManager.EquipEffect(effectData)
        );

        Refresh();
    }

    public void Refresh()
    {
        if (effectData == null || shopManager == null)
            return;

        bool unlocked = shopManager.IsUnlocked(effectData);
        bool equipped = shopManager.IsEquipped(effectData);

        // 開放済みなら開放ボタンを隠す
        unlockButton.gameObject.SetActive(!unlocked);

        // 開放済みなら使用ボタンを表示
        useButton.gameObject.SetActive(unlocked);

        // トロフィーが足りない場合は購入不可
        unlockButton.interactable =
            shopManager.TrophyCount >= effectData.trophyCost;

        // 装備中は使用ボタンを押せない
        useButton.interactable = !equipped;
    }
}