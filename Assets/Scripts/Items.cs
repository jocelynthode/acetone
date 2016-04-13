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
        throw new System.NotImplementedException();
    }

    static void useBomb()
    {
        throw new System.NotImplementedException();
    }

    static void useViewbot()
    {
        throw new System.NotImplementedException();
    }

}
