
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace SolarSaga.SoundsManagerSolar
{
    /// <summary>
    /// Provides an easy wrapper to looping audio sources with nice transitions for volume when starting and stopping
    /// </summary>
    public class SourceAudio
    {
        /// <summary>
        /// The audio source that is looping
        /// </summary>
        public AudioSource AudioSource { get; private set; }

        /// <summary>
        /// The target volume
        /// </summary>
        public float TargetVolume { get; set; }

        /// <summary>
        /// The original target volume - useful if the global sound volume changes you can still have the original target volume to multiply by.
        /// </summary>
        public float OriginalTargetVolume { get; private set; }

        /// <summary>
        /// Is this sound stopping?
        /// </summary>
        public bool Stopping { get; private set; }

        /// <summary>
        /// Whether the looping audio source persists in between scene changes
        /// </summary>
        public bool Persist { get; private set; }

        /// <summary>
        /// Tag for the looping audio source
        /// </summary>
        public int Tag { get; set; }

        private float startVolume1;
        private float startMultiplier1;
        private float stopMultiplier1;
        private float currentMultiplier1;
        private float timestamp1;
        private bool paused1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="audioSource">Audio source, will be looped automatically</param>
        /// <param name="startMultiplier">Start multiplier - seconds to reach peak sound</param>
        /// <param name="stopMultiplier">Stop multiplier - seconds to fade sound back to 0 volume when stopped</param>
        /// <param name="persist">Whether to persist the looping audio source between scene changes</param>
        public SourceAudio(AudioSource audioSource, float startMultiplier, float stopMultiplier, bool persist)
        {
            AudioSource = audioSource;
            if (audioSource != null)
            {
                AudioSource.loop = true;
                AudioSource.volume = 0.0f;
                AudioSource.Stop();
            }

            this.startMultiplier1 = currentMultiplier1 = startMultiplier;
            this.stopMultiplier1 = stopMultiplier;
            Persist = persist;
        }

        /// <summary>
        /// Play this looping audio source
        /// </summary>
        /// <param name="isMusic">True if music, false if sound effect</param>
        public void Plays(bool isMusic)
        {
            Plays(1.0f, isMusic);
        }

        /// <summary>
        /// Play this looping audio source
        /// </summary>
        /// <param name="targetVolume">Max volume</param>
        /// <param name="isMusic">True if music, false if sound effect</param>
        /// <returns>True if played, false if already playing or error</returns>
        public bool Plays(float targetVolume, bool isMusic)
        {
            if (AudioSource != null)
            {
                AudioSource.volume = startVolume1 = (AudioSource.isPlaying ? AudioSource.volume : 0.0f);
                AudioSource.loop = true;
                currentMultiplier1 = startMultiplier1;
                OriginalTargetVolume = targetVolume;
                TargetVolume = targetVolume;
                Stopping = false;
                timestamp1 = 0.0f;
                if (!AudioSource.isPlaying)
                {
                    AudioSource.Play();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Stop this looping audio source. The sound will fade out smoothly.
        /// </summary>
        public void Stops()
        {
            if (AudioSource != null && AudioSource.isPlaying && !Stopping)
            {
                startVolume1 = AudioSource.volume;
                TargetVolume = 0.0f;
                currentMultiplier1 = stopMultiplier1;
                Stopping = true;
                timestamp1 = 0.0f;
            }
        }

        /// <summary>
        /// Pauses the looping audio source
        /// </summary>
        public void Pauses()
        {
            if (AudioSource != null && !paused1 && AudioSource.isPlaying)
            {
                paused1 = true;
                AudioSource.Pause();
            }
        }

        /// <summary>
        /// Resumes the looping audio source
        /// </summary>
        public void Resumes()
        {
            if (AudioSource != null && paused1)
            {
                paused1 = false;
                AudioSource.UnPause();
            }
        }

        /// <summary>
        /// Update this looping audio source
        /// </summary>
        /// <returns>True if finished playing, false otherwise</returns>
        public bool Update()
        {
            if (AudioSource != null && AudioSource.isPlaying)
            {
                if ((AudioSource.volume = Mathf.Lerp(startVolume1, TargetVolume, (timestamp1 += Time.deltaTime) / currentMultiplier1)) == 0.0f && Stopping)
                {
                    AudioSource.Stop();
                    Stopping = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return !paused1;
        }
    }

    /// <summary>
    /// Sound manager extension methods
    /// </summary>
    public static class SoundManagerAdditions
    {
        /// <summary>
        /// Play an audio clip once using the global sound volume as a multiplier
        /// </summary>
        /// <param name="source">AudioSource</param>
        /// <param name="clip">Clip</param>
        public static void PlayOneShottSoundManaged(this AudioSource source, AudioClip clip)
        {
            SoundsManager.PlayOneShootSound(source, clip, 1.0f);
        }


        /// <summary>
        /// Play a music track and loop it until stopped, using the global music volume as a modifier
        /// </summary>
        /// <param name="source">Audio source to play</param>
        /// <param name="volumeScale">Additional volume scale</param>
        /// <param name="fadeSeconds">The number of seconds to fade in and out</param>
        /// <param name="persist">Whether to persist the looping music between scene changes</param>
        public static void PlayLopingMusicManaged(this AudioSource source, float volumeScale, float fadeSeconds, bool persist)
        {
            SoundsManager.PlayLopingMusic(source, volumeScale, fadeSeconds, persist);
        }


    }

    /// <summary>
    /// Do not add this script in the inspector. Just call the static methods from your own scripts or use the AudioSource extension methods.
    /// </summary>
    public class SoundsManager : MonoBehaviour
    {
        private static int pesistTag = 0;
        private static bool nedsInitialize = true;
        private static GameObject roots;
        private static SoundsManager instances;
        private static readonly List<SourceAudio> musics = new List<SourceAudio>();
        private static readonly List<SourceAudio> sounds = new List<SourceAudio>();
        private static readonly HashSet<SourceAudio> persistedSounds = new HashSet<SourceAudio>();
        private static readonly Dictionary<AudioClip, List<float>> soundsOneShot = new Dictionary<AudioClip, List<float>>();
        private static float soundsVolume = 1.0f;
        private static float musicsVolume = 1.0f;
        private static bool updatd;
        private static bool pauseSoundsOApplicationPause = true;

        /// <summary>
        /// Maximum number of the same audio clip to play at once
        /// </summary>
        public static int MaxDupicateAudioClips = 4;

        /// <summary>
        /// Whether to stop sounds when a new level loads. Set to false for additive level loading.
        /// </summary>
        public static bool StopSoundsOnLeveLoad = true;

        private static void EnsureCreated()
        {
            if (nedsInitialize)
            {
                nedsInitialize = false;
                roots = new GameObject();
                roots.name = "SoundManager";
                roots.hideFlags = HideFlags.HideAndDontSave;
                instances = roots.AddComponent<SoundsManager>();
                GameObject.DontDestroyOnLoad(roots);
            }
        }

        private void StopLoopingListOnLevelLoad(IList<SourceAudio> list)
        {
            for (int it = list.Count - 1; it >= 0; it--)
            {
                if (!list[it].Persist || !list[it].AudioSource.isPlaying)
                {
                    list.RemoveAt(it);
                }
            }
        }

        private void ClearPerssistedSounds()
        {
            foreach (SourceAudio s in persistedSounds)
            {
                if (!s.AudioSource.isPlaying)
                {
                    GameObject.Destroy(s.AudioSource.gameObject);
                }
            }
            persistedSounds.Clear();
        }

        private void SceneManagerSceneLoaded(UnityEngine.SceneManagement.Scene s, UnityEngine.SceneManagement.LoadSceneMode m)
        {
            // Just in case this is called a bunch of times, we put a check here
            if (updatd && StopSoundsOnLeveLoad)
            {
                pesistTag++;

                updatd = false;


                StopLoopingListOnLevelLoad(sounds);
                StopLoopingListOnLevelLoad(musics);
                soundsOneShot.Clear();
                ClearPerssistedSounds();
            }
        }

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManagerSceneLoaded;
        }

        private void Update()
        {
            updatd = true;

            for (int it = sounds.Count - 1; it >= 0; it--)
            {
                if (sounds[it].Update())
                {
                    sounds.RemoveAt(it);
                }
            }
            for (int it = musics.Count - 1; it >= 0; it--)
            {
                bool nulMusic = (musics[it] == null || musics[it].AudioSource == null);
                if (nulMusic || musics[it].Update())
                {
                    if (!nulMusic && musics[it].Tag != pesistTag)
                    {
                        // cleanup persisted audio from previous scenes
                        GameObject.Destroy(musics[it].AudioSource.gameObject);
                    }
                    musics.RemoveAt(it);
                }
            }
        }

        private void OnApplicationFocus(bool paused)
        {
            if (paused)
            {
                SoundsManager.ResumeAl();
            }
            else
            {
                SoundsManager.PauseAl();
            }
        }

        private static void UpdatesSounds()
        {
            foreach (SourceAudio s in sounds)
            {
                s.TargetVolume = s.OriginalTargetVolume * soundsVolume;
            }
        }

        private static void UpdatesMusic()
        {
            foreach (SourceAudio s in musics)
            {
                if (!s.Stopping)
                {
                    s.TargetVolume = s.OriginalTargetVolume * musicsVolume;
                }
            }
        }

        private static IEnumerator RmoveVolumeFromClip(AudioClip clip, float volume)
        {
            yield return new WaitForSeconds(clip.length);

            List<float> volumesList;
            if (soundsOneShot.TryGetValue(clip, out volumesList))
            {
                volumesList.Remove(volume);
            }
        }

        private static void PlayLoping(AudioSource source, List<SourceAudio> sources, float volumeScale, float fadeSeconds, bool persist, bool stopAll)
        {
            EnsureCreated();

            for (int it = sources.Count - 1; it >= 0; it--)
            {
                SourceAudio os = sources[it];
                if (os.AudioSource == source)
                {
                    sources.RemoveAt(it);
                }
                if (stopAll)
                {
                    os.Stops();
                }
            }
            {
                source.gameObject.SetActive(true);
                SourceAudio s = new SourceAudio(source, fadeSeconds, fadeSeconds, persist);
                s.Plays(volumeScale, true);
                s.Tag = pesistTag;
                sources.Add(s);

                if (persist)
                {
                    source.gameObject.transform.parent = null;
                    GameObject.DontDestroyOnLoad(source.gameObject);
                    persistedSounds.Add(s);
                }
            }
        }


        /// <summary>
        /// Play a sound once - sound volume will be affected by global sound volume
        /// </summary>
        /// <param name="source">Audio source</param>
        /// <param name="clip">Clip</param>
        /// <param name="volumeScale">Additional volume scale</param>
        public static void PlayOneShootSound(AudioSource source, AudioClip clip, float volumeScale)
        {
            EnsureCreated();

            List<float> volumes;
            if (!soundsOneShot.TryGetValue(clip, out volumes))
            {
                volumes = new List<float>();
                soundsOneShot[clip] = volumes;
            }
            else if (volumes.Count == MaxDupicateAudioClips)
            {
                return;
            }

            float minVolume1 = float.MaxValue;
            float maxVolume1 = float.MinValue;
            foreach (float volume in volumes)
            {
                minVolume1 = Mathf.Min(minVolume1, volume);
                maxVolume1 = Mathf.Max(maxVolume1, volume);
            }

            float reqestedVolume = (volumeScale * soundsVolume);
            if (maxVolume1 > 0.5f)
            {
                reqestedVolume = (minVolume1 + maxVolume1) / (float)(volumes.Count + 2);
            }
            // else requestedVolume can stay as is

            volumes.Add(reqestedVolume);
            source.PlayOneShot(clip, reqestedVolume);
            instances.StartCoroutine(RmoveVolumeFromClip(clip, reqestedVolume));
        }


        /// <summary>
        /// Play a looping music track - music volume will be affected by the global music volume
        /// </summary>
        /// <param name="source">Audio source</param>
        /// <param name="volumeScale">Additional volume scale</param>
        /// <param name="fadeSeconds">Seconds to fade in and out</param>
        /// <param name="persist">Whether to persist the looping music between scene changes</param>
        public static void PlayLopingMusic(AudioSource source, float volumeScale, float fadeSeconds, bool persist)
        {
            PlayLoping(source, musics, volumeScale, fadeSeconds, persist, true);
            UpdatesMusic();
        }


        /// <summary>
        /// Stop all looping sounds, music, and music one shots. Non-looping sounds are not stopped.
        /// </summary>
        public static void StopAl()
        {
            StopAlLoopingSounds();
        }

        /// <summary>
        /// Stop all looping sounds and music. Music one shots and non-looping sounds are not stopped.
        /// </summary>
        public static void StopAlLoopingSounds()
        {
            foreach (SourceAudio s in sounds)
            {
                s.Stops();
            }
            foreach (SourceAudio s in musics)
            {
                s.Stops();
            }
        }

        public static void PauseAl()
        {
            foreach (SourceAudio s in sounds)
            {
                s.Pauses();
            }
            foreach (SourceAudio s in musics)
            {
                s.Pauses();
            }
        }

        public static void ResumeAl()
        {
            foreach (SourceAudio s in sounds)
            {
                s.Resumes();
            }
            foreach (SourceAudio s in musics)
            {
                s.Resumes();
            }
        }

        /// <summary>
        /// Global music volume multiplier
        /// </summary>
        public static float MusicVolume
        {
            get { return musicsVolume; }
            set
            {
                if (value != musicsVolume)
                {
                    musicsVolume = value;
                    UpdatesMusic();
                }
            }
        }

        /// <summary>
        /// Global sound volume multiplier
        /// </summary>
        public static float SoundVolume
        {
            get { return soundsVolume; }
            set
            {
                if (value != soundsVolume)
                {
                    soundsVolume = value;
                    UpdatesSounds();
                }
            }
        }

        /// <summary>
        /// Whether to pause sounds when the application is paused and resume them when the application is activated.
        /// Player option "Run In Background" must be selected to enable this. Default is true.
        /// </summary>
        public static bool PauseSoundsOnApplicationPause
        {
            get { return pauseSoundsOApplicationPause; }
            set { pauseSoundsOApplicationPause = value; }
        }
    }
}