using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UpgradeMenu : MonoBehaviour {

    public GameObject upgradeButton;
    public GameObject upgradeText;

    Dictionary<string, Upgrade> upgrades;

    // Use this for initialization
    void Start () {
        // TODO: Maybe put this into the Upgrade class ?
        CreateUpgradeButton("maxHealth", "Maximum Health");
        CreateUpgradeButton("attack", "Attack");
        CreateUpgradeButton("viewers", "Daily Giveaways");
        CreateUpgradeButton("moneyGain", "Sign Sponsor");
        CreateUpgradeButton("startGameLevel", "Starting Level");

        RefreshMenu();
    }

    void CreateUpgradeButton(string name, string prettyName)
    {
        var upgradesPanel = GameObject.Find("UpgradesPanel").transform;
        GameObject button = Instantiate(upgradeButton) as GameObject;
        button.name = name + "Button";
        button.GetComponent<Button>().onClick.AddListener(() => UpdatePref(name));
        button.GetComponentInChildren<Text>().text = prettyName;
        button.transform.Find("nameText").GetComponent<Text>().text = prettyName;
        button.transform.SetParent(upgradesPanel);

        GameObject text = Instantiate(upgradeText) as GameObject;
        text.name = name;
        text.GetComponent<Text>().text = "#" + name;
        text.transform.SetParent(upgradesPanel);
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

            string statText = string.Format("Current: {0}        \nNext: {1}        ", stat, upgrade.Stat);
            GameObject.Find(item.Key).GetComponent<Text>().text = statText;
            var costText = GameObject.Find(item.Key + "Button").transform.Find("costText").GetComponent<Text>();
            costText.text = string.Format("Cost: {0:C0}", upgrade.Cost);
            // TODO: Display cost
        }

        GameObject.Find("moneyText").GetComponent<Text>().text = string.Format("Money: {0:C0}", PlayerPrefs.GetInt("money"));
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

    public void UpdatePref(string name)
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
			upgradeFunctions.Add("viewers", (baseLevel, baseStat) =>
				new Upgrade((baseLevel + 1) * 10, baseStat + 100)
			);
			upgradeFunctions.Add("moneyGain", (baseLevel, baseStat) =>
				new Upgrade((baseLevel + 1) * 10, baseStat + 1)
			);
			upgradeFunctions.Add("startGameLevel", (baseLevel, baseStat) =>
				new Upgrade((baseLevel + 1) * 10, (baseStat/10)*10 + 10)
			);
        }
    }
}

