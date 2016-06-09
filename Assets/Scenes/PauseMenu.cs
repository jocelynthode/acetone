using UnityEngine;
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
    private Text muteText;
    private AudioSource music;

	// Use this for initialization
	void Start () {
		pauseMenu = pauseMenu.GetComponent<Canvas> ();
		mainMenu = mainMenu.GetComponent<Canvas> ();
		quitMenu = quitMenu.GetComponent<Canvas> ();
		continueText = continueText.GetComponent<Button> ();
		mainMenuText = mainMenuText.GetComponent<Button> ();
		exitText = exitText.GetComponent<Button> ();
        muteText = GameObject.Find("MuteButton").GetComponent<Text>();
        music = GameObject.Find("Sound").transform.FindChild("acetone").GetComponent<AudioSource>();
        quitMenu.enabled = false;
		mainMenu.enabled = false;
		pauseMenu.enabled = false;

        UpdateMuteText();
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

    public void MutePress()
    {
        music.mute = !music.mute;
        UpdateMuteText();
    }

    public void UpdateMuteText()
    {
        muteText.text = music.mute ? "Unmute Music" : "Mute Music";
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
        if (SceneManager.GetActiveScene().name == "UpgradeMenu")
        {
            SceneManager.LoadScene("StartMenu");
            GameManager.instance.state = GameManager.GameState.MENU;
        }
        else GameManager.instance.OnGameOver(false);
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
