using UnityEngine;
using UnityEngine.UI;

public static class Items {
    public enum ItemType
    {
        AD,
        BOMB,
        VIEWBOT
    }

    public static void useItem(ItemType item) 
    {
        switch (item)
        {
            case ItemType.AD:
                useAd();
                break;
            case ItemType.BOMB:
                useBomb();
                break;
            case ItemType.VIEWBOT:
                useViewbot();
                break;
            default:
                throw new System.ArgumentException();
        }
    }

    static void useAd()
    {
        int money = PlayerPrefs.GetInt("money");
        int newMoney = (PlayerPrefs.GetInt("itemsPowerLevel") + 1) * 25;
        GameManager.instance.GainMoney("Endorsement earned: ", money, newMoney);
    }

    static void useBomb()
    {
        int i = 3;
        while (i > 0 && GameManager.instance.boardScript.enemies.Count > 0)
        {
            GameManager.instance.boardScript.enemies[0].Die();
            i = i - 1;
        }
    }

    static void useViewbot()
    {
        Player.instance.ViewerBots += 50 + (Player.instance.TotalViewers / 2);
        cashMoneyBiatch.Play();
    }

}
