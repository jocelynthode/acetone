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
		tutorialText.text = "This is an enemy ! Just walk against it to kill it !";
        tutorialPopup.gameObject.SetActive(true);
        PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
		Invoke ("RemovePopup", 4);
	}

	public void ObjectTutorial()
	{
        int tutorialDisplayed = PlayerPrefs.GetInt("tutorialDisplayed");
        if (tutorialDisplayed > 2)
			return;
		tutorialText.text = "Walk on objects to pick them up ! Bombs kill enemies, Ads give you money and Sponsorship gives you viewers!";
        tutorialPopup.gameObject.SetActive(true);
        PlayerPrefs.SetInt("tutorialDisplayed", tutorialDisplayed + 1);
		Invoke ("RemovePopup", 4);
	}

	public void RemovePopup()
	{
        tutorialPopup.gameObject.SetActive(false);
	}
}
