using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public void OpenGame()
    {
        Save();
        SceneManager.LoadScene(6);
    }
    public void OpenOptions()
    {
        Save();
        SceneManager.LoadScene(4);
    }
    public void OpenPolicyFromMenu()
    {
        Save();
        SceneManager.LoadScene(3);
    }
    public void ExitFromMenu()
    {
        Application.Quit(1);
    }

    public void OpenRules()
    {
        Save();
        SceneManager.LoadScene(5);
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Prev", 2);
        PlayerPrefs.Save();
    }
}
