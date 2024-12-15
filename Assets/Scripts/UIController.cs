using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // GUI elements
    public Text globalStats;
    public Text boidStats;
    public RawImage boidColor;
    public GameObject help;
    public GameObject[] buttons;
    public GameObject pauseMenu;
    public Text tutorial;

    private bool hide = false;
    private bool pause = false;
    private int textCounter;

    private void Start()
    {
        // "1x speed" select color
        buttons[1].GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    private void Update()
    {
        // enable/disable GUI
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

        // pause menu
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            pause = !pause;
            pauseMenu.SetActive(pause);
            SetTime(pause ? 0 : 1);
        }

        if (!hide)
        {
            globalStats.text =
              "Points: " + Stats.points +
            "\nCount: " + Stats.population +
            "\nFood: " + Stats.foodAmount;

            // display boid stats rounded to hundredths place
            if (Stats.boidDisplay != null)
            {
                boidStats.transform.parent.gameObject.SetActive(true);
                boidColor.color = Stats.boidDisplay.genes.color;
                boidStats.text =
                  "energy: " + Mathf.Round(Stats.boidDisplay.energy * 100) / 100 +
                "\nage: " + Mathf.Round(Stats.boidDisplay.age * 100) / 100 +
                "\nspecies = " + Mathf.Round(Stats.boidDisplay.genes.species * 100) / 100 +
                "\nmaturity = " + Mathf.Round(Stats.boidDisplay.genes.maturity * 100) / 100 +
                "\nvision = " + Mathf.Round(Stats.boidDisplay.genes.vision * 100) / 100 +
                "\npower = " + Mathf.Round(Stats.boidDisplay.genes.power * 100) / 100 +
                "\nspeed = " + Mathf.Round(Stats.boidDisplay.genes.speed * 100) / 100 +
                "\nseparation weight = " + Mathf.Round(Stats.boidDisplay.genes.separationWeight * 100) / 100 +
                "\nalignment weight = " + Mathf.Round(Stats.boidDisplay.genes.alignmentWeight * 100) / 100 +
                "\ncohesion weight = " + Mathf.Round(Stats.boidDisplay.genes.cohesionWeight * 100) / 100;
            }
            else
            {
                boidStats.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void SetTime(int speed)
    {
        // identify speed set
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Image>().color = new Color(0, 0, 0, i == speed ? 0 : (80f / 255f));
        }
        Stats._TIME_MULTIPLIER = (float)speed / 10;
    }

    public void EnableTutorial()
    {
        textCounter = 0;
        tutorial.transform.parent.gameObject.SetActive(true);
        tutorial.text = 
            "Welcome to Boids Ecosystem!\n" +
          "\nThe goal of the game is to create a suitable habitat for the fish by balancing their population and food source." +
          "\nAs you watch them evolve, notice how new species may appear and their behavior change!";
    }

    public void Next()
    {
        textCounter++;
        switch(textCounter) 
        {
            case 1:
                tutorial.text =
                    "Movement:\n" +
                  "\nUse [W] [A] [S] [D] to move around and [E/Space] [Q] to go up and down. " +
                  "\nDrag using [Right Mouse Button] to rotate your camera.";
                break;
            case 2:
                tutorial.text =
                    "Speed Control:\n" +
                  "\nAt the bottom right  you will see 4 buttons that adjust the game speed by:" +
                  "\n0x, 1x, 2x, and 3x";
                break;
            case 3:
                tutorial.text =
                    "Getting started:\n" +
                  "\nThe first step of the game is to start planting the food source." +
                  "\nUse [Left Mouse Button] to start planting seeds on the sea bed." +
                  "\nPlanting seeds costs points (2).";
                break;
            case 4:
                tutorial.text =
                    "Your seeds will begin to grow into beautiful seaweed!" +
                  "\nThis seaweed will serve as the primary food source for the fish." +
                  "\nWhile growing, they will start generating more points.";
                break;
            case 5:
                tutorial.text =
                    "Fill the Tank:\n" +
                  "\nOnce you have enough seaweed & at least 30 points," +
                  "\npress the Fish button at the top left to spawn some fish!";
                break;
            case 6:
                tutorial.text =
                    "These fish will evolve over generations and take on new traits." +
                  "\nYou can click any fish you want to view their stats. " +
                  "\nYou can also press [F] while selecting a fish to follow them.";
                break;
            case 7:
                tutorial.text =
                    "Fish Behavior:\n" +
                  "\nEach fish has an energy and age stat that determines their survivability. " +
                  "\nMost fish will die at 100, but require energy to make it that far." +
                  "\nMake sure there is enough food for everyone!";
                break;
            case 8:
                tutorial.text =
                    "Once a fish hits age 30, they will have a chance to (asexually) reproduce! " +
                  "\nThe children will have a chance alter their genome." +
                  "\nIf a new strand is significantly different from their ancestors, they are considered a new species! ";
                break;
            case 9:
                tutorial.text = "press [P] to hide the UI\npress [Esc] for the pause menu\nPress [R] to restart";
                break;
            case 10:
                tutorial.text = "That's all! \nHave fun!";
                break;
            case 11:
                tutorial.transform.parent.gameObject.SetActive(false);
                break;
        }
    }
}
