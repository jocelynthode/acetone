﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UpgradeMenu : MonoBehaviour
{

    public GameObject upgradeButton;
    public GameObject upgradeText;
    public GameObject upgradePanel;

    Dictionary<string, Upgrade> upgrades;

    // Use this for initialization
    void Start()
    {
        CreateUpgradeButton("maxHealth");
        CreateUpgradeButton("attack");
        CreateUpgradeButton("viewers");
        CreateUpgradeButton("itemsPower");
        CreateUpgradeButton("startGameLevel");

        if (PlayerPrefs.GetInt("highestLevel") >= 15)
        {
            CreateUpgradeButton("moneyGain");
        }

        RefreshMenu();
    }

    void CreateUpgradeButton(string name)
    {
        var upgradesPanel = GameObject.Find("UpgradesPanel").transform;
        var panel = Instantiate(upgradePanel).transform;
        panel.SetParent(upgradesPanel);

        GameObject button = Instantiate(upgradeButton) as GameObject;
        button.name = name + "Button";
        button.GetComponent<Button>().onClick.AddListener(() => UpdatePref(name));
        button.transform.SetParent(panel);

        GameObject text = Instantiate(upgradeText) as GameObject;
        text.name = name;
        text.GetComponent<Text>().text = "#" + name;
        text.transform.SetParent(panel);
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

            GameObject upgradeText = GameObject.Find(item.Key);
            if (upgradeText)
            {
                upgradeText.GetComponent<Text>().text = string.Format("{0}\n\n{1}", upgrade.Name, upgrade.Text);
                var upgradeButton = GameObject.Find(item.Key + "Button");
                var costText = upgradeButton.transform.Find("costText").GetComponent<Text>();
                costText.text = string.Format("\n${0}", upgrade.Cost);
                upgradeButton.GetComponent<Button>().interactable = (PlayerPrefs.GetInt("money") >= upgrade.Cost);
            }
        }

        GameObject.Find("moneyText").GetComponent<Text>().text = string.Format("Money: ${0}", PlayerPrefs.GetInt("money"));
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    public void Restart()
    {
        SceneManager.LoadScene("LevelProcedural");
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

        Utils.PlaySound("cashtill");
        RefreshMenu();
    }


    public class Upgrade
    {
        public int Cost { get; private set; }
        public int Stat { get; private set; }
        public string Name { get; private set; }
        public string Text { get; private set; }

        public Upgrade(int cost, int stat, string name, string text = "")
        {
            Cost = cost;
            Stat = stat;
            Name = name;
            Text = text;
        }

        public delegate Upgrade UpgradeFunc(int baseLevel,int baseStat);

        public static Dictionary<string, UpgradeFunc> upgradeFunctions;

        static Upgrade()
        {
            upgradeFunctions = new Dictionary<string, UpgradeFunc>();
            upgradeFunctions.Add("maxHealth", (baseLevel, baseStat) => {
                var up = new Upgrade((baseLevel + 1) * 10, baseStat + 10, "Maximum Health");
                up.Text = string.Format("Increase maximum health from {0} to {1}.", baseStat, up.Stat);
                return up;
            });
            upgradeFunctions.Add("attack", (baseLevel, baseStat) => {
                var up = new Upgrade((baseLevel + 1) * 10, baseStat + 10, "Attack");
                up.Text = string.Format("Increase attack from {0} to {1}.", baseStat, up.Stat);
                return up;
            });
            upgradeFunctions.Add("viewers", (baseLevel, baseStat) => {
                var up = new Upgrade((baseLevel + 1) * 10, baseStat + 100, "Daily Giveaways");
                up.Text = string.Format("Offer giveaways to viewers to get more popular. "
                    + "This increases the number of viewers from {0} to {1}.", baseStat, up.Stat);
                return up;
            });
            upgradeFunctions.Add("itemsPower", (baseLevel, baseStat) => {
                var cost = 200 * Mathf.Pow(5, baseLevel);
                var up = new Upgrade((int) cost, baseStat + 1, "Item Specialist");
                // Improve description
                up.Text = string.Format("Improve the power of picked-up items: from level {0} to {1}.", baseStat, up.Stat);
                return up;
            });
            upgradeFunctions.Add("startGameLevel", (baseLevel, baseStat) => {
                var cost = 100 * Mathf.Pow(4, baseLevel);
                var up = new Upgrade((int) cost, (baseStat / 10) * 10 + 10, "Starting Level");
                up.Text = string.Format("Start game from level {1} instead of {0}.", baseStat, up.Stat);
                return up;
            });
				
            upgradeFunctions.Add("moneyGain", (baseLevel, baseStat) => {
                var nextStat = baseStat + 30;
                var cost = (baseLevel + 1) * 100 * Mathf.Pow(1.1f, baseLevel + 1);
                var up = new Upgrade((int) cost, nextStat, "Sign with Sponsor");
                up.Text = string.Format("Sign with sponsors to earn more money per kill. Total bonus increases from {0:F1}x to {1:F1}x.",
                    baseStat / 100f, nextStat / 100f);
                return up;
            });
        }
    }
}

