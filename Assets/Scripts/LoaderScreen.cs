using SolarSaga.SoundsManagerSolar;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderScreen : MonoBehaviour
{
    [SerializeField] AudioSource MusicSource;
    private int var;

    void Start()
    {   
        SoundsManager.SoundVolume = PlayerPrefs.GetInt("SoundConst", 1) + 0;
        SoundsManager.MusicVolume = PlayerPrefs.GetInt("MusicConst", 1)*1;
        MusicSource.PlayLopingMusicManaged(1.0f, 0.9f, true);
        var = PlayerPrefs.GetInt("Start", 1);
        Invoke("OpenNext", 1.3f);
    }


    public void OpenNext()
    { 
        SceneManager.LoadScene(var);
    }
}
