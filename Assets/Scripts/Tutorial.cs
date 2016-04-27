using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class Tutorial : MonoBehaviour {

	public Canvas tutorialPopup;
	public Text tutorialText;

	// Use this for initialization
	void Start () {
		tutorialPopup = tutorialPopup.GetComponent<Canvas> ();
		tutorialText = tutorialText.GetComponent<Text> ();
		tutorialPopup.enabled = false;
	}

	public void MovementTutorial()
	{
		tutorialText.text = "To move, use WASD or the arrow keys !";
		Invoke("RemovePopup", 5);
	}

	public void EnemyTutorial()
	{
		tutorialText.text = "This is an enemy ! Just walk against it to kill it !";
		Invoke("RemovePopup", 5);
	}

	public void ObjectTutorial()
	{
		tutorialText.text = "Walk on the objects to pick them up ! Bombs kill enemies, Ads give you money and Sponsorship...";
		Invoke("RemovePopup", 5);
	}


	void removePopup()
	{
		tutorialPopup.enabled = false;
	}

}
