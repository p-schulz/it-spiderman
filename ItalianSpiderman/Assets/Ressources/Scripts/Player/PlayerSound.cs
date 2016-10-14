using UnityEngine;
using System.Collections;

public class PlayerSound : MonoBehaviour {

    public AudioSource SoundSource;
    public AudioSource VoiceSource;

    // slide, run and walk effects
    public AudioClip Footstep;
    public AudioClip Impact;
    public AudioClip Skid;
    public AudioClip Land;
    public AudioClip Slide;
    public AudioClip WallHit;

    // Game mechanic stuff
    public AudioClip FlipIntoLevel;
    public AudioClip Die;

    // jump effects
    public AudioClip[] jumps;
    public AudioClip[] SideFlip;
    public AudioClip[] SlideSpin;
    public AudioClip BackFlip;
    public AudioClip LandHurt;

    // combat jumps
    public AudioClip KickJump;
    public AudioClip KickJumpLand;
    public AudioClip GroundKickJump;
    public AudioClip StrikeJump;
    public AudioClip StrikeJumpLand;
    public AudioClip GroundStrikeJump;

    // punch/kick/strike voices and effects
    public AudioClip[] punches_voice;
    public AudioClip[] punches_effect;
    public AudioClip TakeDamage;
    public AudioClip HeavyKnockback;

    // climbing and hanging
    public AudioClip Climb;
    public AudioClip Hang;

    // picking up items sounds
    public AudioClip EnjoyGuitar;
    public AudioClip SmokeCigarette;


    /// <summary>
    /// general use functions
    /// </summary>

    private IEnumerator voiceRoutine;

    IEnumerator Footsteps(float speed, float delay) {
        float lastStepTime = Time.time - speed + delay;

        while (true) {
            if (SuperMath.Timer(lastStepTime, speed)) {
                SoundSource.PlayOneShot(Footstep, 0.07f);
                lastStepTime = Time.time;
            }

            yield return 0;
        }
    }

    IEnumerator PlayClipWithDelay(AudioSource source, AudioClip clip, float delay) {
        yield return new WaitForSeconds(delay);

        source.PlayOneShot(clip);
    }
    
    void PlayVoice(AudioClip clip)
    {
        if (voiceRoutine != null)
            StopCoroutine(voiceRoutine);

        VoiceSource.Stop();

        VoiceSource.PlayOneShot(clip);
    }

    void PlayRandomVoice(AudioClip[] clips)
    {
        if (voiceRoutine != null)
            StopCoroutine(voiceRoutine);

        VoiceSource.Stop();

        VoiceSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    // Might be deleted
    void PlayRandomVoice(AudioClip[] clips, float delay) {
        if (voiceRoutine != null)
            StopCoroutine(voiceRoutine);

        VoiceSource.Stop();

        voiceRoutine = PlayClipWithDelay(VoiceSource, clips[Random.Range(0, clips.Length)], delay);
        StartCoroutine(voiceRoutine);
    }

    // Might be deleted
    public void StopVoices()
    {
        if (voiceRoutine != null)
            StopCoroutine(voiceRoutine);
    }

    public void Stop() {
        SoundSource.Stop();
        SoundSource.volume = 1;
        SoundSource.loop = false;
    }

    public void StartFootsteps(float speed, float delay) {
        StopAllCoroutines();
        StartCoroutine(Footsteps(speed, delay));
    }

    public void EndFootsteps()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Sounds for Combat
    /// </summary>
     
    public void PlayRandomPunch() {
        PlayRandomVoice(punches_voice);
    }

    public void PlayPunch(int i) {
        PlayVoice(punches_voice[i]);
    }

    public void PlayTakeDamage() {
        PlayVoice(TakeDamage);
    }

    public void PlayHeavyKnockback() {
        PlayVoice(HeavyKnockback);
    }

    /// <summary>
    /// Sounds for Jumping
    /// </summary>

    public void PlayRandomJump() {
        PlayRandomVoice(jumps);
    }

    public void PlayJump(int i) {
        PlayVoice(jumps[i]);
    }

    public void PlaySideFlip() {
        PlayRandomVoice(SideFlip);
    }

    public void PlayBackFlip() {
        PlayVoice(BackFlip);
    }

    public void PlaySlideSpin() {
        PlayRandomVoice(SlideSpin);
    }

    public void PlayLand() {
        SoundSource.PlayOneShot(Land, 0.5f);
    }

    public void PlayLandHurt() {
        PlayVoice(LandHurt);
    }

    public void PlayImpact() {
        SoundSource.PlayOneShot(Impact, 0.6f);
    }

    /// <summary>
    /// Sounds for Game mechanics
    /// </summary>

    public void PlayFlipIntoLevel() {
        SoundSource.PlayOneShot(FlipIntoLevel);
    }

    public void PlayDie() {
        PlayVoice(Die);
    }

    /// <summary>
    /// Sounds for Picking up coffee or cigarettes
    /// </summary>

    public void PlayEnjoyGuitar() {
        PlayVoice(EnjoyGuitar);
    }

    public void PlaySmokeCigarette() {
        PlayVoice(SmokeCigarette);
    }

    /// <summary>
    /// Sounds for KickJump
    /// </summary>

    public void PlayKickJump() {
        PlayVoice(KickJump);
    }

    public void PlayKickJumpLand() {
        SoundSource.PlayOneShot(KickJumpLand);
    }

    public void PlayGroundKickJump() {
        PlayVoice(GroundKickJump);
    }

    /// <summary>
    /// Sounds for StrikeJump
    /// </summary>

    public void PlayStrikeJump() {
        PlayVoice(StrikeJump);
    }

    public void PlayStrikeJumpLand() {
        SoundSource.PlayOneShot(StrikeJumpLand);
    }

    public void PlayGroundStrikeJump() {
        PlayVoice(GroundStrikeJump);
    }

    /// <summary>
    /// Sound for sliding, stopping, wall hitting
    /// </summary>

    // sliding
    public void PlaySlide() {
        SoundSource.loop = true;
        SoundSource.clip = Slide;
        SoundSource.volume = 0.75f;
        SoundSource.Play();
    }
    
    // stopping
    public void PlaySkid() {
        SoundSource.PlayOneShot(Skid, 0.6f);
    }

    // walking against wall
    public void PlayWallHit() {
        PlayVoice(WallHit);
    }

    /// <summary>
    /// climbing and hanging
    /// </summary>

    public void PlayClimb() {
        PlayVoice(Climb);
    }

    public void PlayHang() {
        PlayVoice(Hang);
    }
}
