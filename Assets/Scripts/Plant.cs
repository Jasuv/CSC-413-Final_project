using System.Collections;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public GameObject grassPad;
    public GameObject plantType;
    public int cost;
    public float growTime;
    public int generatePoints;

    private GameObject grass;
    private GameObject plant;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        float flavor = Random.Range(0.5f, 1.5f); // adds variety
        GetComponent<SphereCollider>().radius = flavor * 2;
        growTime = growTime * flavor;
        generatePoints = (int)(generatePoints * flavor);

        // spawn grass pad and plant
        grass = Instantiate(grassPad, transform.position, transform.rotation);
        Vector3 rand = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        plant = Instantiate(plantType, transform.position + rand, transform.rotation);

        // start grow anim and point generator
        StartCoroutine(Sprout(grass, 0.5f / (Stats._TIME_MULTIPLIER * 10), flavor));
        StartCoroutine(Sprout(plant, growTime, flavor));
        StartCoroutine(Generate(growTime, generatePoints));

        Stats.foodAmount++;
    }

    // growing animation
    private IEnumerator Sprout(GameObject plant, float duration, float size)
    {
        Vector3 start = Vector3.zero;
        Vector3 target = new Vector3(size, size, size);
        float timer = 0;
        while (timer < duration)
        {
            // pause waiting loop
            while (Stats._SIMULATION_TIME == 0)
            {
                yield return null;
            }

            // grow animation
            timer += Stats._SIMULATION_TIME * 10;
            plant.transform.localScale = Vector3.Lerp(start, target, timer / duration);
            yield return null;
        }
        plant.transform.localScale = target;
    }

    // generate points
    private IEnumerator Generate(float duration, int amount)
    {
        float rate = duration / amount;
        float mileStone = rate;
        float timer = 0;
        while (timer < duration)
        {
            // pause waiting loop
            while (Stats._SIMULATION_TIME == 0)
            {
                yield return null;
            }

            // generate points
            timer += Stats._SIMULATION_TIME * 10;
            if (timer >= mileStone)
            {
                if (Random.value > 0.4) Stats.points++;
                mileStone += rate;
            }
            yield return null;
        }
        // keep alive for a while
        yield return new WaitForSeconds(duration * (Stats.foodAmount / (Stats.population + 0.001f)));
        StartCoroutine(Die());
    }

    // kinda ugly
    private IEnumerator Die()
    {
        Vector3 grassStart = new Vector3(grass.transform.localScale.x, grass.transform.localScale.y, grass.transform.localScale.z);
        Vector3 plantStart = new Vector3(plant.transform.localScale.x, plant.transform.localScale.y, plant.transform.localScale.z);
        Vector3 target = Vector3.zero;
        float timer = 0;
        while (timer < 2)
        {
            // pause waiting loop
            while (Stats._SIMULATION_TIME == 0)
            {
                yield return null;
            }

            // shrink
            timer += Stats._SIMULATION_TIME * 10;
            grass.transform.localScale = Vector3.Lerp(grassStart, target, timer / 2);
            plant.transform.localScale = Vector3.Lerp(plantStart, target, timer / 2);
            yield return null;
        }
        grass.transform.localScale = target;
        plant.transform.localScale = target;
        Stats.foodAmount--;
        Destroy(grass);
        Destroy(plant);
        Destroy(this);
    }
}