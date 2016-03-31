using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeMenu : MonoBehaviour {
    // string[] upgrades = new string[] { "maxHealth", "attack", "defense", "viewers" };
    string[] upgrades = new string[] { "maxHealth", "attack" };
    // Use this for initialization
    void Awake () {
        foreach (string up in upgrades)
        {
            GameObject.Find(up).GetComponent<Text>().text = PlayerPrefs.GetInt(up).ToString();
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
        foreach (string up in upgrades)
            GameObject.Find(up).GetComponent<Text>().text = PlayerPrefs.GetInt(up).ToString();
    }
    public void UpdatePrefs(string Name)
    {
        Text updateText;
        updateText = GameObject.Find(Name).GetComponent<Text>();
        PlayerPrefs.SetInt(Name, PlayerPrefs.GetInt(Name)+10);
        updateText.text = PlayerPrefs.GetInt(Name).ToString();
        PlayerPrefs.Save();
    }
}

