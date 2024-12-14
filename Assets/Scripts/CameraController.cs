using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        }
        pitch = Mathf.Clamp(pitch, -90f, 90f);
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

        // CHANGE THIS LATER
        if (Input.GetKeyDown(KeyCode.F) && boidDisplay != null)
        {
            follow = !follow;
        }
        if (boidDisplay == null) follow = false;

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
            if (EventSystem.current.IsPointerOverGameObject()) {}
            else if (hit.transform.CompareTag("terrain") && hit.point.y < 1 && Stats.points > 0 && Stats._SIMULATION_TIME > 0)
            {
                Stats.points -= seeds[0].GetComponent<Plant>().cost;
                Instantiate(seeds[0], hit.point, transform.rotation);
            }
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
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Stats.points = Stats._STARTING_POINTS;
    }
}
