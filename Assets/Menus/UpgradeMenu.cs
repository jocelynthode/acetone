using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Restart()
    {
        SceneManager.LoadScene("levelProcedural");
    }

    public void UpdatePrefs(string PrefName, int update)
    {
        PlayerPrefs.SetInt(PrefName, update);
    }
}

