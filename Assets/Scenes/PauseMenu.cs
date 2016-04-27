﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	
	public Canvas pauseMenu;
	public Canvas mainMenu;
	public Canvas quitMenu;
	public Button continueText;
	public Button mainMenuText;
	public Button exitText;

	// Use this for initialization
	void Start () {
		pauseMenu = pauseMenu.GetComponent<Canvas> ();
		mainMenu = mainMenu.GetComponent<Canvas> ();
		quitMenu = quitMenu.GetComponent<Canvas> ();
		continueText = continueText.GetComponent<Button> ();
		mainMenuText = mainMenuText.GetComponent<Button> ();
		exitText = exitText.GetComponent<Button> ();
		quitMenu.enabled = false;
		mainMenu.enabled = false;
		pauseMenu.enabled = false;
	}

	public void ContinuePress()
	{
		pauseMenu.enabled = false;
	}

	public void MainMenuPress()
	{
		mainMenu.enabled = true;
		continueText.enabled = false;
		mainMenuText.enabled = false;
		exitText.enabled = false;
	}

	public void NoMainMenuPress()
	{
		mainMenu.enabled = false;
		continueText.enabled = true;
		mainMenuText.enabled = true;
		exitText.enabled = true;
	}

	public void MainMenu()
	{
		//TODO clean before loading scene
		SceneManager.LoadScene ("StartMenu");
	}

	public void ExitPress()
	{
		quitMenu.enabled = true;
		continueText.enabled = false;
		mainMenuText.enabled = false;
		exitText.enabled = false;
	}

	public void NoExitPress()
	{
		quitMenu.enabled = false;
		continueText.enabled = true;
		mainMenuText.enabled = true;
		exitText.enabled = true;
	}

	public void ExitGame()
	{
		Application.Quit ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape)){
			pauseMenu.enabled = !pauseMenu.enabled;
			NoMainMenuPress ();
			NoExitPress ();
		}
	}
}