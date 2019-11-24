using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // Con Fig Vars
    [SerializeField] float levelLoadDelay = 2f;

    [Header("Thrust Levels")]
    [SerializeField] float mainThrust = 10f;
    [SerializeField] float rcsThrust = 100f;

    [Header("Audio Clips")]
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip clearLevel;
    [SerializeField] AudioClip death;

    [Header("Particle VFX")]
    [SerializeField] ParticleSystem mainEngineVFX;
    [SerializeField] ParticleSystem clearLevelVFX;
    [SerializeField] ParticleSystem deathVFX;

    [Header("Lighting")]
    [SerializeField] Light thrusterGlow;

    // State Vars
    Rigidbody rigidBody;
    AudioSource audioSource;
    int curLevelIndex;

    bool isTranscending = false;
    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        curLevelIndex = SceneManager.GetActiveScene().buildIndex;
        thrusterGlow.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTranscending)
        {
            RespondToThrustInput();
            RespondToRotationInput();
        }
        if (Debug.isDebugBuild) RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L)) LoadNextLevel();
        if (Input.GetKeyDown(KeyCode.R)) LoadFirstLevel();
        if (Input.GetKeyDown(KeyCode.C)) collisionsDisabled = !collisionsDisabled;
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) { ApplyThrust(); }
        else
        {
            if (audioSource) audioSource.Stop();
            mainEngineVFX.Stop();
            thrusterGlow.enabled = false;
        }
        
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (audioSource && !audioSource.isPlaying && mainEngine) audioSource.PlayOneShot(mainEngine);
        mainEngineVFX.Play();
        thrusterGlow.enabled = !thrusterGlow.enabled;
    }

    private void RespondToRotationInput()
    {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) { }
        else if (Input.GetKey(KeyCode.A)) { Rotate(rcsThrust * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.D)) { Rotate(-rcsThrust * Time.deltaTime); }
    }

    private void Rotate(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTranscending || collisionsDisabled) return; // Ignore collision when dead or transcending
        if (audioSource) audioSource.Stop();
        mainEngineVFX.Stop();
        thrusterGlow.enabled = false;

        switch (collision.gameObject.tag)
        {
            case "Friendly": // Do nothing
                break;
            case "Finish":
                HandleVictory();
                break;
            default:
                HandleDeath();
                break;
        }
    }

    private void HandleVictory()
    {
        isTranscending = true;
        if (audioSource && clearLevel) audioSource.PlayOneShot(clearLevel);
        clearLevelVFX.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void HandleDeath()
    {
        isTranscending = true;
        if (audioSource && death) audioSource.PlayOneShot(death);
        deathVFX.Play();
        Invoke("RestartLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        if (curLevelIndex + 1 == SceneManager.sceneCountInBuildSettings) { SceneManager.LoadScene(0); }
        else { SceneManager.LoadScene(curLevelIndex + 1); }
    }

    private void RestartLevel() // Implement later to restart level on death
    {
        SceneManager.LoadScene(curLevelIndex);
    }

    private void LoadFirstLevel() // Use to restart the game
    {
        SceneManager.LoadScene(0);
    }
}
