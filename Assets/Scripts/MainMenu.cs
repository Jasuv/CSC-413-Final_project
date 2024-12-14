using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject start;
    public GameObject controls;
    public GameObject panel;
    public GameObject quit;

    public AudioClip[] songs;

    private void Awake()
    {
        try
        {
            AudioSource menuPlayer = GameObject.Find("Menu Camera").GetComponent<AudioSource>();
            if (menuPlayer != null)
            {
                menuPlayer.clip = songs[Random.Range(0, 3)];
                menuPlayer.Play();
            }
        }
        catch { }
    }

    public void Controls()
    {
        start.SetActive(false);
        controls.SetActive(false);
        quit.SetActive(false);
        panel.SetActive(true);
    }

    public void Back()
    {
        start.SetActive(true);
        controls.SetActive(true);
        quit.SetActive(true);
        panel.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene("Ocean");
    }

    public void Menu()
    {
        try
        {
            Destroy(Camera.main.gameObject);
        }
        catch { }
        SceneManager.LoadScene("Main Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
