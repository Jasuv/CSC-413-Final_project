using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text globalStats;
    public Text boidStats;
    public RawImage boidColor;

    public GameObject help;

    public GameObject[] buttons;

    private bool hide = false;
    private bool enableHelp = true;

    public int round;

    private void Start()
    {
        buttons[1].GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            globalStats.transform.parent.gameObject.SetActive(hide);
            boidStats.transform.parent.gameObject.SetActive(hide);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(hide);
            }
            hide = !hide;
        }

        if (!hide)
        {
            globalStats.text =
              "Points: " + Stats.points +
            "\nCount: " + Stats.population +
            "\nFood: " + Stats.foodAmount;

            if (Stats.boidDisplay != null)
            {
                boidStats.transform.parent.gameObject.SetActive(true);
                boidColor.color = Stats.boidDisplay.genes.color;
                boidStats.text =
                  "energy: " + Mathf.Round(Stats.boidDisplay.energy * round) / round +
                "\nage: " + Mathf.Round(Stats.boidDisplay.age * round) / round +
                "\nspecies = " + Mathf.Round(Stats.boidDisplay.genes.species * round) / round +
                "\nmaturity = " + Mathf.Round(Stats.boidDisplay.genes.maturity * round) / round +
                "\nvision = " + Mathf.Round(Stats.boidDisplay.genes.vision * round) / round +
                "\npower = " + Mathf.Round(Stats.boidDisplay.genes.power * round) / round +
                "\nspeed = " + Mathf.Round(Stats.boidDisplay.genes.speed * round) / round +
                "\nseparation weight = " + Mathf.Round(Stats.boidDisplay.genes.separationWeight * round) / round +
                "\nalignment weight = " + Mathf.Round(Stats.boidDisplay.genes.alignmentWeight * round) / round +
                "\ncohesion weight = " + Mathf.Round(Stats.boidDisplay.genes.cohesionWeight * round) / round;
            }
            else
            {
                boidStats.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void SetTime(int speed)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Image>().color = new Color(0, 0, 0, i == speed ? 0 : (80f / 255f));
        }
        Stats._TIME_MULTIPLIER = (float)speed / 10;
    }

    public void EnableHelp()
    {
        enableHelp = !enableHelp;
        help.SetActive(enableHelp);
    }
}
