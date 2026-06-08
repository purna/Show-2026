using System.Linq;
using UnityEngine;

public class FirstPersonAudio : MonoBehaviour
{
    // ✔️ CHANGED: Swapped FirstPersonMovement out for SC_RigidbodyWalker
    public SC_RigidbodyWalker character; 
    public GroundCheck groundCheck;

    [Header("Step")]
    public AudioSource stepAudio;
    public AudioSource runningAudio;
    [Tooltip("Minimum velocity for moving audio to play")]
    public float velocityThreshold = .01f;
    Vector3 lastCharacterPosition;
    
    Vector3 CurrentCharacterPosition => character ? character.transform.position : Vector3.zero;

    [Header("Landing")]
    public AudioSource landingAudio;
    public AudioClip[] landingSFX;

    [Header("Jump")]
    public Jump jump;
    public AudioSource jumpAudio;
    public AudioClip[] jumpSFX;

    [Header("Crouch")]
    public Crouch crouch;
    public AudioSource crouchStartAudio, crouchedAudio, crouchEndAudio;
    public AudioClip[] crouchStartSFX, crouchEndSFX;

    private AudioSource[] movingAudiosCached;


    void Reset()
    {
        // ✔️ CHANGED: Looking for SC_RigidbodyWalker now
        character = GetComponentInParent<SC_RigidbodyWalker>();
        groundCheck = (transform.parent ?? transform).GetComponentInChildren<GroundCheck>();
        stepAudio = GetOrCreateAudioSource("Step Audio");
        runningAudio = GetOrCreateAudioSource("Running Audio");
        landingAudio = GetOrCreateAudioSource("Landing Audio");

        // Setup jump audio.
        jump = GetComponentInParent<Jump>();
        if (jump)
        {
            jumpAudio = GetOrCreateAudioSource("Jump audio");
        }

        // Setup crouch audio.
        crouch = GetComponentInParent<Crouch>();
        if (crouch)
        {
            crouchStartAudio = GetOrCreateAudioSource("Crouch Start Audio");
            crouchedAudio = GetOrCreateAudioSource("Crouched Audio");
            crouchEndAudio = GetOrCreateAudioSource("Crouch End Audio");
        }
    }

    void Start()
    {
        movingAudiosCached = new AudioSource[] { stepAudio, runningAudio, crouchedAudio };
        
        if (character)
        {
            lastCharacterPosition = CurrentCharacterPosition;
        }
    }

    void OnEnable() => SubscribeToEvents();

    void OnDisable() => UnsubscribeToEvents();

    void FixedUpdate()
    {
        if (!character) return;

        // Play moving audio if the character is moving and on the ground.
        float velocity = Vector3.Distance(CurrentCharacterPosition, lastCharacterPosition);
        
        if (velocity >= velocityThreshold && groundCheck && groundCheck.isGrounded)
        {
            if (crouch && crouch.IsCrouched)
            {
                SetPlayingMovingAudio(crouchedAudio);
            }
            // ✔️ CHANGED: Since RigidbodyWalker doesn't have an IsRunning flag,
            // we determine "Running" based on whether their physical velocity is high.
            // Adjust the multiplier (0.6f) to change the cutoff point between walking and running SFX.
            else if (velocity > (character.speed * Time.fixedDeltaTime * 0.6f))
            {
                SetPlayingMovingAudio(runningAudio);
            }
            else
            {
                SetPlayingMovingAudio(stepAudio);
            }
        }
        else
        {
            SetPlayingMovingAudio(null);
        }

        lastCharacterPosition = CurrentCharacterPosition;
    }

    void SetPlayingMovingAudio(AudioSource audioToPlay)
    {
        if (movingAudiosCached == null) return;

        for (int i = 0; i < movingAudiosCached.Length; i++)
        {
            AudioSource audio = movingAudiosCached[i];
            if (audio != null && audio != audioToPlay)
            {
                if (audio.isPlaying)
                {
                    audio.Pause();
                }
            }
        }

        if (audioToPlay && !audioToPlay.isPlaying)
        {
            audioToPlay.Play();
        }
    }

    #region Play instant-related audios.
    void PlayLandingAudio() => PlayRandomClip(landingAudio, landingSFX);
    void PlayJumpAudio() => PlayRandomClip(jumpAudio, jumpSFX);
    void PlayCrouchStartAudio() => PlayRandomClip(crouchStartAudio, crouchStartSFX);
    void PlayCrouchEndAudio() => PlayRandomClip(crouchEndAudio, crouchEndSFX);
    #endregion

    #region Subscribe/unsubscribe to events.
    void SubscribeToEvents()
    {
        if (groundCheck)
        {
            groundCheck.Grounded += PlayLandingAudio;
        }

        if (jump)
        {
            jump.Jumped += PlayJumpAudio;
        }

        if (crouch)
        {
            crouch.CrouchStart += PlayCrouchStartAudio;
            crouch.CrouchEnd += PlayCrouchEndAudio;
        }
    }

    void UnsubscribeToEvents()
    {
        if (groundCheck)
        {
            groundCheck.Grounded -= PlayLandingAudio;
        }

        if (jump)
        {
            jump.Jumped -= PlayJumpAudio;
        }

        if (crouch)
        {
            crouch.CrouchStart -= PlayCrouchStartAudio;
            crouch.CrouchEnd -= PlayCrouchEndAudio;
        }
    }
    #endregion

    #region Utility.
    AudioSource GetOrCreateAudioSource(string name)
    {
        AudioSource result = System.Array.Find(GetComponentsInChildren<AudioSource>(), a => a.name == name);
        if (result)
            return result;

        result = new GameObject(name).AddComponent<AudioSource>();
        result.spatialBlend = 1;
        result.playOnAwake = false;
        result.transform.SetParent(transform, false);
        return result;
    }

    static void PlayRandomClip(AudioSource audio, AudioClip[] clips)
    {
        if (!audio || clips == null || clips.Length <= 0)
            return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clips.Length > 1)
        {
            int safetyCounter = 0;
            while (clip == audio.clip && safetyCounter < 10)
            {
                clip = clips[Random.Range(0, clips.Length)];
                safetyCounter++;
            }
        }

        audio.clip = clip;
        audio.Play();
    }
    #endregion 
}