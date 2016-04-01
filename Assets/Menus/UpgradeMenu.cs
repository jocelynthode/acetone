using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UpgradeMenu : MonoBehaviour {

    Dictionary<string, Upgrade> upgrades;

    // Use this for initialization
    void Awake () {
        RefreshMenu();
    }

    void RefreshMenu()
    {
        // Pre-compute all upgrades
        upgrades = new Dictionary<string, Upgrade>();
        foreach (var item in Upgrade.upgradeFunctions)
        {
            int stat = PlayerPrefs.GetInt(item.Key);
            int level = PlayerPrefs.GetInt(item.Key + "Level");
            Upgrade upgrade = item.Value(level, stat);
            upgrades.Add(item.Key, upgrade);

            GameObject.Find(item.Key).GetComponent<Text>().text = PlayerPrefs.GetInt(item.Key).ToString();
            // TODO: Set cost
        }

        GameObject.Find("moneyText").GetComponent<Text>().text = string.Format("Money: {0:C2}", PlayerPrefs.GetInt("money"));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Restart()
    {
        SceneManager.LoadScene("LevelProcedural");
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        GameManager.CheckPlayerPrefs(true);
        RefreshMenu();
    }

    public void UpdatePrefs(string name)
    {
        int level = PlayerPrefs.GetInt(name + "Level");
        Upgrade upgrade = upgrades[name];
        int money = PlayerPrefs.GetInt("money");
        int newMoney = money - upgrade.Cost;

        if (newMoney < 0)
            return;
        PlayerPrefs.SetInt("money", newMoney);
        PlayerPrefs.SetInt(name, upgrade.Stat);
        PlayerPrefs.SetInt(name + "Level", level + 1);
        PlayerPrefs.Save();

        RefreshMenu();
    }


    public class Upgrade
    {
        public int Cost { get; private set; }
        public int Stat { get; private set; }
        public Upgrade(int cost, int stat)
        {
            Cost = cost;
            Stat = stat;
        }

        public delegate Upgrade UpgradeFunc(int baseLevel, int baseStat);
        public static Dictionary<string, UpgradeFunc> upgradeFunctions;

        static Upgrade() {
            upgradeFunctions = new Dictionary<string, UpgradeFunc>();
            upgradeFunctions.Add("maxHealth", (baseLevel, baseStat) =>
                new Upgrade((baseLevel + 1) * 10, baseStat + 10)
            );
            upgradeFunctions.Add("attack", (baseLevel, baseStat) =>
                new Upgrade((baseLevel + 1) * 10, baseStat + 10)
            );
        }
    }
}

