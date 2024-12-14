using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;

    public float slow;
    public float fast;
    public float sensitivity;
    public GameObject[] seeds;

    private bool follow = false;
    private Boid boidDisplay;
    private float speed;
    private float pitch, yaw;
    private Vector3 pos;

    public AudioSource soundBoard;
    public AudioClip[] soundEffects;

    public AudioSource musicPlayer;
    public AudioClip[] songs;

    private void Awake()
    {
        // keeps camera when reloading
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(PlayMusic());
    }

    void Update()
    {
        // rotate camera
        if (Input.GetKey(KeyCode.Mouse1))
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        }
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // movement
        speed = Input.GetKey(KeyCode.LeftShift) ? fast : slow;
        pos = new Vector3(Input.GetAxis("Horizontal"),
                          Input.GetAxis("Height"),
                          Input.GetAxis("Vertical")) * speed * Time.deltaTime;

        if (follow)
        {
            transform.LookAt(boidDisplay.transform);
            transform.position = boidDisplay.pos + new Vector3(3, 3, 3);
        }
        else
        {
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.position += transform.right * pos.x + transform.up * pos.y + transform.forward * pos.z;
        }

        // follow boid
        if (Input.GetKeyDown(KeyCode.F) && boidDisplay != null)
        {
            follow = !follow;
        }
        if (boidDisplay == null) follow = false;

        // scene reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Ocean", LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        RaycastHit hit;
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Input.GetKey(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, 1000))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { }

            // spawn seed
            else if (hit.transform.CompareTag("terrain") && hit.point.y < 1 && Stats._SIMULATION_TIME > 0)
            {
                GameObject seed = seeds[Random.Range(0, seeds.Length - 1)];
                if (Stats.points >= seed.GetComponent<Plant>().cost)
                {
                    Stats.points -= seeds[0].GetComponent<Plant>().cost;
                    Instantiate(seeds[0], hit.point, transform.rotation);
                }
            }

            // select boid
            else if (hit.transform.CompareTag("boid"))
            {
                boidDisplay = hit.transform.GetComponent<Boid>();
                Stats.boidDisplay = boidDisplay;
            }
            else
            {
                boidDisplay = null;
                Stats.boidDisplay = null;
            }
        }

        if (Stats.newSpecies) 
        {
            Play(soundEffects[1]);
            Stats.newSpecies = false;
        }
    }

    // reset stats on reload
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Stats.points = Stats._STARTING_POINTS;
        Stats.population = 0;
        Stats.foodAmount = 0;
    }

    private IEnumerator PlayMusic()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, 60));
            musicPlayer.clip = songs[Random.Range(0, 3)];
            musicPlayer.Play();
            yield return new WaitForSeconds(600);
        }
    }

    public void Play(AudioClip clip) 
    {
        soundBoard.clip = clip;
        soundBoard.Play();
    }
}
