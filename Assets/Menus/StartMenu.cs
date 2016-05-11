using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenu : MonoBehaviour
{

    public Canvas quitMenu;
    public Canvas newGameMenu;
    public Button continueText;
    public Button newGameText;
    public Button exitText;


    // Use this for initialization
    void Start()
    {
        quitMenu = quitMenu.GetComponent<Canvas>();
        newGameMenu = newGameMenu.GetComponent<Canvas>();
        continueText = continueText.GetComponent<Button>();
        newGameText = newGameText.GetComponent<Button>();
        exitText = exitText.GetComponent<Button>();
        quitMenu.enabled = false;
        newGameMenu.enabled = false;
        if (!PlayerPrefs.HasKey("highestLevel"))
        {
            continueText.enabled = false;
            continueText.gameObject.SetActive(false);
        }
	
    }

    public void ContinuePress()
    {
        SceneManager.LoadScene("UpgradeMenu");
    }

    public void NewGamePress()
    {
        if (continueText.enabled == false)
        {
            NewGame();
            return;
        }
        newGameMenu.enabled = true;
        continueText.enabled = false;
        newGameText.enabled = false;
        exitText.enabled = false;
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
		GameManager.CheckPlayerPrefs(true);
		SceneManager.LoadScene("LevelProcedural");
    }

    public void NoNewGamePress()
    {
        newGameMenu.enabled = false;
        continueText.enabled = true;
        newGameText.enabled = true;
        exitText.enabled = true;
    }

    public void ExitPress()
    {
        quitMenu.enabled = true;
        continueText.enabled = false;
        newGameText.enabled = false;
        exitText.enabled = false;
    }

    public void NoExitPress()
    {
        quitMenu.enabled = false;
        continueText.enabled = true;
        newGameText.enabled = true;
        exitText.enabled = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
