using UnityEngine;

public static class TrophyManager
{
    private const string TrophyKey = "PlayerTrophyCount";

    // 所持トロフィー数
    public static int TrophyCount
    {
        get { return PlayerPrefs.GetInt(TrophyKey, 0); }
    }

    // トロフィーを獲得
    public static void AddTrophies(int amount)
    {
        if (amount <= 0)
            return;

        PlayerPrefs.SetInt(TrophyKey, TrophyCount + amount);
        PlayerPrefs.Save();
    }

    // トロフィーを消費
    public static bool TrySpendTrophies(int amount)
    {
        if (amount < 0 || TrophyCount < amount)
            return false;

        PlayerPrefs.SetInt(TrophyKey, TrophyCount - amount);
        PlayerPrefs.Save();

        return true;
    }
}