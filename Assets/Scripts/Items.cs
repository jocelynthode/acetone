using UnityEngine;
using UnityEngine.UI;

public static class Items
{
    public enum ItemType
    {
        SODA,
        BOMB,
        VIEWBOT,
        LADDER
    }

    public static void useItem(ItemType item, Collider2D collider = null)
    {
        switch (item)
        {
            case ItemType.SODA:
                useSoda();
                break;
            case ItemType.BOMB:
                useBomb();
                break;
            case ItemType.VIEWBOT:
                useViewbot();
                break;
            case ItemType.LADDER:
                useLadder();
                break;
            default:
                throw new System.ArgumentException();
        }
        if (collider != null)
            collider.gameObject.SetActive(false);
    }

    static void useSoda()
    {
        Player player = GameManager.instance.boardScript.player;
        int playerHealth = player.hp;
        int healthGained = (PlayerPrefs.GetInt("itemsPowerLevel") + 1) * 25;
        if (playerHealth + healthGained > PlayerPrefs.GetInt("maxHealth"))
        {
            player.hp = PlayerPrefs.GetInt("maxHealth");
        }
        else
        {
            player.hp = playerHealth + healthGained;
        }
        player.healthPoint.text = player.hp.ToString();
        GameManager.instance.DisplayText(string.Format("{0} health regained !", healthGained));
    }

    static void useBomb()
    {
        int i = 3 + PlayerPrefs.GetInt("itemsPower");
        while (i > 0 && GameManager.instance.boardScript.enemies.Count > 0)
        {
            GameManager.instance.boardScript.enemies[0].Die();
            i = i - 1;
            // http://audiosoundclips.com/8-bit-explosion-blast-sound-effects-sfx/
            Utils.PlaySound("BombExplosion");
        }
    }

    static void useViewbot()
    {
        Player.instance.ViewerBots += 50 + (Player.instance.TotalViewers / 2);
    }

    static void useLadder()
    {
        Utils.PlaySound("LadderUse");
        GameManager.instance.OnLevelCompletion(PlayerPrefs.GetInt("itemsPowerLevel")*2+2);
    }

}
