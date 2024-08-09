using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstOpen : MonoBehaviour
{


    public void NextScreen()
    {
        int menu = 2;
        PlayerPrefs.SetInt("Start", menu);
        PlayerPrefs.Save();
        SceneManager.LoadScene(menu);
    }

    public void CheckPolicyFirst()
    {
        Utils.POLICY = true;
        Utils.POLICY_IND = 1;
        SceneManager.LoadScene(3);
    }
}
