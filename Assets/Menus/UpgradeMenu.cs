using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeMenu : MonoBehaviour {
    string[] upgrades = new string[] { "maxhealth", "attack", "defense", "viewers" };
	// Use this for initialization
	void Start () {
        foreach (string up in upgrades)
        {
            GameObject.Find(up).GetComponent<Text>().text = PlayerPrefs.GetInt(up).ToString();
        } 
        //TODO display money after death
        GameObject.Find("moneyText").GetComponent<Text>().text = string.Format("Money: {0:C2}", PlayerPrefs.GetInt("money"));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Restart()
    {
        SceneManager.LoadScene("levelProcedural");
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
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

