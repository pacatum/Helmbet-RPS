using UnityEngine;


public class AudioManager : SingletonMonoBehaviour<AudioManager> {

    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip choosePaperSound;
    [SerializeField] AudioClip chooseRockSound;
    [SerializeField] AudioClip chooseScissorsSound;
    [SerializeField] AudioClip waitingSound;
    [SerializeField] AudioClip winRoundSound;
    [SerializeField] AudioClip looseRoundSound;
    [SerializeField] AudioClip noticeOfStart;
    [SerializeField] AudioClip totalWinSound;
    [SerializeField] AudioClip totalLooseSound;

	
    public void PlayPaperChooseSound() {
        audioSource.PlayOneShot( choosePaperSound );
    }

    public void PlayRockChooseSound() {
        audioSource.PlayOneShot( chooseRockSound );
    }

    public void PlayScissorsChooseSound() {
        audioSource.PlayOneShot( chooseScissorsSound );
    }

    public void PlayWinSound() {
        audioSource.PlayOneShot( winRoundSound );
    }

    public void PlayLooseSound() {
        audioSource.PlayOneShot( looseRoundSound );
    }

    public void PlayWaitingSound() {
        audioSource.Stop();
        audioSource.PlayOneShot( waitingSound );
    }

    public void PlayNoticeSound() {
        audioSource.Stop();
        audioSource.PlayOneShot( noticeOfStart );
    }

    public void StopPlaying() {
        audioSource.Stop();
    }

    public void SetVolume( float volume ) {
        audioSource.volume = volume;
    }

    public void PlayTotalWinSound() {
        audioSource.Stop();
        audioSource.clip = totalWinSound;
        audioSource.PlayOneShot( totalWinSound );
    }

    public void PlayTotalLooseSound() {
        audioSource.Stop();
        audioSource.clip = totalLooseSound;
        audioSource.PlayOneShot( totalLooseSound );
    }
}