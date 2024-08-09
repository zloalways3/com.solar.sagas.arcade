using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour
{
    public void GoToMenu()
    {
        if(POLICY)
        {
            POLICY = false;
            SceneManager.LoadScene(POLICY_IND);
        } else
        {
            int Previus = PlayerPrefs.GetInt("Prev", 2);
            SceneManager.LoadScene(Previus);
        }
    }

    public static int POLICY_IND = 2;
    public static bool POLICY = false;
}
