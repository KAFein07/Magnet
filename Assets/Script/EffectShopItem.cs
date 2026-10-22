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
    [SerializeField] private Button equippedButton;

    private EffectData effectData;
    private EffectShopManager shopManager;

    public void Setup(EffectData data, EffectShopManager manager)
    {
        effectData = data;
        shopManager = manager;

        if (iconImage != null)
            iconImage.sprite = data.icon;

        if (costText != null)
            costText.text = data.trophyCost.ToString();

        unlockButton.onClick.RemoveAllListeners();
        useButton.onClick.RemoveAllListeners();

        unlockButton.onClick.AddListener(() =>
        {
            shopManager.UnlockEffect(effectData);
            shopManager.RefreshShop();
        });

        useButton.onClick.AddListener(() =>
        {
            shopManager.EquipEffect(effectData);
            shopManager.RefreshShop();
        });

        Refresh();
    }

    public void Refresh()
    {
        if (effectData == null || shopManager == null)
            return;

        bool unlocked = shopManager.IsUnlocked(effectData);
        bool equipped = shopManager.IsEquipped(effectData);

        // 未解放ならアンロックボタンを表示
        unlockButton.gameObject.SetActive(!unlocked);

        // 解放済みで、装備中でなければ装備ボタンを表示
        useButton.gameObject.SetActive(unlocked && !equipped);

        // 装備中なら装備中ボタンを表示
        if (equippedButton != null)
            equippedButton.gameObject.SetActive(unlocked && equipped);

        // トロフィーが足りなければアンロックできない
        unlockButton.interactable =
            shopManager.TrophyCount >= effectData.trophyCost;
    }
}