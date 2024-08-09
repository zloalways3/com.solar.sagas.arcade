using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Plane PlayerObject;
    [SerializeField] TextMeshProUGUI ScoreLabel;
    [SerializeField] GameObject HeartGameObject;
    [SerializeField] GameObject BombGameObject;
    public int countScore = 0;
    float itemTimer = 0;
    public bool invali = true;
    private float speedSolar = 0.15f;
    public int LevelConst = -1;
    private int marker = 0;

    void Update()
    {
        if(LevelConst==-1) LevelConst = PlayerPrefs.GetInt("LevelName", 0);

        if (invali)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SceneManager.LoadScene(2);
            }
            if (Time.timeSinceLevelLoad > itemTimer)
            {
                itemTimer += Random.Range(0.3f, 1.2f);
                int randomize = Random.Range(-10, 25);
                GameObject gameItem = Instantiate(randomize >= 0 ? HeartGameObject : BombGameObject, transform, true);
                gameItem.transform.position = new Vector3(Random.Range(-2.31f, 2.31f), 6, 0);
            }
            ScoreLabel.text = $"SCORE\n${countScore}/${((LevelConst + 1) * 50)}";
            if (marker==1)
            {
                PlayerObject.PosPlane -= speedSolar;
            }
            else if (marker==-1)
            {
                PlayerObject.PosPlane += speedSolar;
            }
        }
    }

    public void LevelButtonDown()
    {
        marker = 1;
    }
    public void LevelButtonUp()
    {
        marker = 0;
    }

    public void RightButtonDown()
    {
        marker = -1;
    }
    public void RightButtonUp() {
        marker = 0;
    }

    private const int settings = 4;

    public void EndGame()
    {
        invali = false;
        PlayerPrefs.SetInt("ScoreName", countScore);
        PlayerPrefs.Save();
        SceneManager.LoadScene(7);
    }

    public void GoToSettingsFromGame()
    {
        PlayerPrefs.SetInt("Prev", 6);
        PlayerPrefs.Save();
        SceneManager.LoadScene(settings);
    }
}
