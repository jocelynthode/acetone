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
		tutorialText.text = "To move, use WASD or the arrow keys !";
        PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
		Invoke ("RemovePopup", 4);
	}

	public void EnemyTutorial()
	{
        int tutorialDisplayed = PlayerPrefs.GetInt("tutorialDisplayed");
        if (tutorialDisplayed > 1)
			return;
        tutorialText.text = "This is an enemy ! Just walk against it to kill it ! The more ennemies you kill the more money you earn but WATCH OUT you receive money only after 10 levels";
        tutorialPopup.gameObject.SetActive(true);
        PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
		Invoke ("RemovePopup", 8);
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
            tutorialText.text = "Beware ! The Yellah can attack you from a far distance !";
            PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);

        } 
        else if (GameManager.instance.level == 20)
        {
            tutorialText.text = "Beware ! The Redah is twice as fast as you are !";
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
		tutorialText.text = "Walk on objects to pick them up ! Bombs kill enemies, Soda give you some health, Sponsorship gives you viewers and ladders lets you skip levels!";
        tutorialPopup.gameObject.SetActive(true);
        PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
		Invoke ("RemovePopup", 8);
	}

	public void RemovePopup()
	{
        tutorialPopup.gameObject.SetActive(false);
	}
}
