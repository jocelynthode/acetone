using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class Tutorial : MonoBehaviour {

	public Image tutorialPopup;
	public Text tutorialText;

	// Use this for initialization
	void Awake () {
		tutorialPopup = tutorialPopup.GetComponent<Image> ();
		tutorialText = tutorialText.GetComponent<Text> ();
        tutorialPopup.gameObject.SetActive(false);
	}

	public void MovementTutorial()
	{
        int tutorialDisplayed = PlayerPrefs.GetInt("tutorialDisplayed");
        if (tutorialDisplayed > 0)
			return;
        tutorialPopup.gameObject.SetActive(true);
		tutorialText.text = "To move, use WASD or the arrow keys!\nPress ESC for more controls and options.";
        PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
		Invoke ("RemovePopup", 8);
	}

	public void EnemyTutorial()
	{
        int tutorialDisplayed = PlayerPrefs.GetInt("tutorialDisplayed");
        if (tutorialDisplayed > 1)
			return;
        tutorialText.text = "This is an enemy! Just walk against it to fight it! Sponsors pay you for every enemy killed, but the payout is only every 10 levels.";
        tutorialPopup.gameObject.SetActive(true);
        PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
		Invoke ("RemovePopup", 12);
	}

    public void EnemyOthersTutorial()
    {
        int tutorialDisplayed = PlayerPrefs.GetInt("tutorialDisplayed");
        if (tutorialDisplayed > 4)
            return;
        if (GameManager.instance.level == 10)
        {
            if (tutorialDisplayed > 3)
                return;
            tutorialText.text = "Beware! The Yellah can attack you from a far distance! They also earn you twice as much money.";
            PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);

        } 
        else if (GameManager.instance.level >= 20)
        {
            tutorialText.text = "Beware! The Redah is twice as fast as you are! Sponsors will pay a lot for their death.";
            PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
        }
        tutorialPopup.gameObject.SetActive(true);
        Invoke ("RemovePopup", 8);
    }

	public void ObjectTutorial()
	{
        int tutorialDisplayed = PlayerPrefs.GetInt("tutorialDisplayed");
        if (tutorialDisplayed > 2)
			return;
		tutorialText.text = "Walk on items to pick them up! Bombs kill enemies, Bananes heal you, View bots bring you more viewers and Ladders let you skip levels!";
        tutorialPopup.gameObject.SetActive(true);
        PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
		Invoke ("RemovePopup", 12);
	}

	public void RemovePopup()
	{
        tutorialPopup.gameObject.SetActive(false);
	}
}
