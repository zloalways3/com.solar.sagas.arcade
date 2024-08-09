using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{

    [SerializeField] Text EndLabel;
    [SerializeField] Text label1;
    [SerializeField] GameObject LabeObject;
    [SerializeField] Button ButtonObject;
    [SerializeField] Sprite OnSprite;
    [SerializeField] Sprite OffSprite;
    private int LevelConstant , ScoreEndSreen;
    [SerializeField] Image BgSprite;
    [SerializeField] Sprite WinSprite;
    [SerializeField] Sprite LoseSprite;

    void Start()
    {
        LevelConstant = PlayerPrefs.GetInt("LevelName", 0);
        ScoreEndSreen = PlayerPrefs.GetInt("ScoreName", 0);
        if(ScoreEndSreen>=(LevelConstant+1)*50)
        {
            PlayerPrefs.SetInt("LevelName", LevelConstant + 1);
            PlayerPrefs.Save();
        } else
        {

        }
        LabeObject.SetActive(ScoreEndSreen < (LevelConstant + 1) * 50);
        EndLabel.text = $"SCORE: ${ScoreEndSreen}/${(LevelConstant + 1) * 50}";
        ButtonObject.image.sprite = ScoreEndSreen >= (LevelConstant + 1) * 50 ? OnSprite : OffSprite;
        label1.text = ScoreEndSreen >= (LevelConstant + 1) * 50 ? "WIN" : "LOSE";
        BgSprite.sprite = ScoreEndSreen >= (LevelConstant + 1) * 50 ? WinSprite : LoseSprite;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(ScoreEndSreen >= (LevelConstant + 1) * 50 ? 2 : 6);
    }

}
