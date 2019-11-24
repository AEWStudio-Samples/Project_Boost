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

    // State Vars
    Rigidbody rigidBody;
    AudioSource audioSource;
    int curLevelIndex;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        curLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotationInput();
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) { ApplyThrust(); }
        else
        {
            if (audioSource) audioSource.Stop();
            mainEngineVFX.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (audioSource && !audioSource.isPlaying) audioSource.PlayOneShot(mainEngine);
        mainEngineVFX.Play();
    }

    private void RespondToRotationInput()
    {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {

        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return; // Ignore collision when dead or transcending
        if (audioSource) audioSource.Stop();
        mainEngineVFX.Stop();

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
        state = State.Transcending;
        audioSource.PlayOneShot(clearLevel);
        clearLevelVFX.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void HandleDeath()
    {
        state = State.Dying;
        audioSource.PlayOneShot(death);
        deathVFX.Play();
        Invoke("RestartLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        if (curLevelIndex + 1 == SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
        }
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
