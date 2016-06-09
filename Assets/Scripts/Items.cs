using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static class Items
{
    public enum ItemType
    {
        SODA,
        BOMB,
        VIEWBOT,
        LADDER,
        LETTER
    }
        
    static Dictionary<int, string> lettersText = new Dictionary<int, string> {
        {1, "Fuck yeah! Today I received my new PC! It should be good enough for streaming!\nI can finally try to become a streamer and hopefully leave my useless job..."},
        {2, "Today I got my first viewers! I can already feel the subscribe button coming to me! On the other hand my boss couldn't stop\nchewing on me today! What a bitch! Once I hit 1000 viewers consistently I'm done with this job!"},
        {5, "I couldn't stream today, I'm sure I'll lose all the progress I made since I started streaming. This bitch, making me work on\nChristmas eve... Please let me quit this dead end of a job!"},
        {10, "I forgot to keep my diary up to date.. My stream is literally taking me all my free time. I still can't quit my job! These people watching me\ndon't deserve me! I'm giving them everything and nobody is donating or subscribing! What a bunch of losers"},
        {14, "I didn't go to my job this morning, I wasn't feeling well. I keep having these big headaches. I also woke up in front of my neighbor's house\nand I have no memories on how I got there..."},
        {17, "Everyone is a fucking tool! My boss won't give me a raise and makes me work every fucking day of the week! My viewers aren't even helping me \nlive my dream! Maybe I should pay a visit to some of them, I know where they live! \n\nStill I'm a bit worried about my absences, maybe I should go check a doctor."},
        {20, "What the fuck! I woke up in one of my viewer's home! I don't remember how but there's blood everywhere! This can't be me! I thought about it\nbut it was just a joke, I didn't mean it! I can already hear the sirens coming to my house and everything is again turning white I don't feel my le..."}
    };
    static Dictionary<int, string> lettersDate = new Dictionary<int, string> {
        {1, "21 September 2013"},
        {2, "02 November 2014"},
        {5, "24 December 2013"},
        {10, "02 May 2014"},
        {14, "15 May 2014"},
        {17, "15 June 2014"},
        {20, "02 September 2014"} 
    };

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
            case ItemType.LETTER:
                useLetter();
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
        GameManager.instance.DisplayText(string.Format("{0}HP regained!", healthGained));
    }

    static void useBomb()
    {
        int i = 3 + PlayerPrefs.GetInt("itemsPowerLevel");
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
        Player.instance.ViewerBots += 50 + (PlayerPrefs.GetInt("viewers") / 2);
    }

    static void useLadder()
    {
        Utils.PlaySound("LadderUse");
        GameManager.instance.OnLevelCompletion(PlayerPrefs.GetInt("itemsPowerLevel")*2+2);
    }

    static void useLetter()
    {
        Canvas letter = GameObject.Find("Letter").GetComponent<Canvas>();
        Text letterText = GameObject.Find("Text").GetComponent<Text>();
        Text dateText = GameObject.Find("Date").GetComponent<Text>();

        letterText.text = lettersText[GameManager.instance.level];
        dateText.text = lettersDate[GameManager.instance.level];
        letter.enabled = true;
    }
}
