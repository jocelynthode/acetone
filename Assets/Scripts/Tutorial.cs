using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class Tutorial : MonoBehaviour {

	public Canvas tutorialPopup;
	public Text tutorialText;
	public int displayed = 0;

	// Use this for initialization
	void Awake () {
		tutorialPopup = tutorialPopup.GetComponent<Canvas> ();
		tutorialText = tutorialText.GetComponent<Text> ();
		tutorialPopup.enabled = false;
	}

	public void MovementTutorial()
	{
		if (displayed > 0)
			return;
		tutorialPopup.enabled = true;
		tutorialText.text = "To move, use WASD or the arrow keys !";
		displayed++;
	}

	public void EnemyTutorial()
	{
		if (displayed > 1)
			return;
		tutorialText.text = "This is an enemy ! Just walk against it to kill it !";
		tutorialPopup.enabled = true;
		displayed++;
	}

	public void ObjectTutorial()
	{
		if (displayed > 2)
			return;
		tutorialText.text = "Walk on objects to pick them up ! Bombs kill enemies, Ads give you money and Sponsorship gives you viewers!";
		tutorialPopup.enabled = true;
		displayed++;
	}

	public void RemovePopup()
	{
		tutorialPopup.enabled = false;
	}
}
