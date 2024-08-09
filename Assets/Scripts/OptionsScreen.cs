using SolarSaga.SoundsManagerSolar;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{
    [SerializeField] Image soundsImage;
    [SerializeField] Image musicImage;
    
    void Start()
    {
        Invoke("check", 0.001f);
    }

    public void ChangeMusicByButton()
    {
        int MO = PlayerPrefs.GetInt("MusicConst", 1);
        MO++;
        MO %= 2;
        PlayerPrefs.SetInt("MusicConst", MO);
        PlayerPrefs.Save();
        SoundsManager.MusicVolume = MO;
        check();
    }

    public void ChangeSoundsByButton()
    {
        int SO = PlayerPrefs.GetInt("SoundConst", 1);
        SO++;
        SO %= 2;
        PlayerPrefs.SetInt("SoundConst", SO);
        PlayerPrefs.Save();
        SoundsManager.SoundVolume = SO;
        check();

    }


    private void check()
    {
        int SO = PlayerPrefs.GetInt("SoundConst", 1);
        if(SO==1) soundsImage.color = Color.white;
        else soundsImage.color = Color.gray;
        int MO = PlayerPrefs.GetInt("MusicConst", 1);
        if (MO == 1) musicImage.color = Color.white;
        else musicImage.color = Color.gray;
    }

}
