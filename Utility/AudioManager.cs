using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;

    private Coroutine musicRoutine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ServiceLocator.Register(this);
    }

    // FUNGSI BARU: Mematikan musik dan menghentikan coroutine
    public void StopMusic()
    {
        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
            musicRoutine = null;
        }

        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = null; // Reset clip agar lagu berikutnya pasti keputar ulang
        }
    }

    public void PlayMusic(AudioClip clip, float volume = 1f, float fadeTime = 0.5f)
    {
        if (clip == null) return;

        // Jika lagu yang dimintai sudah sama dan sedang berputar, abaikan
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(PlayMusicRoutine(clip, volume, fadeTime));
    }

    private IEnumerator PlayMusicRoutine(AudioClip clip, float volume, float fadeTime)
    {
        yield return Fade(0f, fadeTime);

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();

        yield return Fade(volume, fadeTime);
    }

    private IEnumerator Fade(float target, float duration)
    {
        float start = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        musicSource.volume = target;
    }
}