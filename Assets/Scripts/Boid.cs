using UnityEngine;

// for out-of-bounds check
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    public Vector3 pos;
    public Vector3 vel;

    public Genes genes;
    public float energy;
    public float age;

    private BoidManager manager;

    public void init(Vector3 pos, Vector3 vel, Genes genes, BoidManager manager)
    {
        this.pos = pos;
        this.vel = vel;
        this.genes = genes;
        this.manager = manager;

        energy = 100 - Random.Range(0, 11);
        age = 0;
        transform.localScale = new Vector3(genes.power, genes.power, genes.power);
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = genes.color;

        // makes sure rigidbody doesn't affect boid
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void Update()
    {
        try
        {
            transform.forward = vel.normalized;
            transform.position = pos;
        }
        catch
        {
            Debug.LogError("vel = NaN");
            transform.forward = Vector3.zero;
            transform.position = new Vector3(125, 125, 125);
        }

        age += Stats._SIMULATION_TIME * 2;
        energy += Stats.foodAmount / Stats.population * Stats._SIMULATION_TIME;
        float multiplier = 1 + (float)(0.7 * genes.power + 0.3 * genes.speed) / 10;
        energy -= Stats.population / (Stats.foodAmount + 0.001f) * multiplier * Stats._ENERGY_DRAIN * Stats._SIMULATION_TIME;
        energy = Mathf.Clamp(energy, 0, 100);

        // reproduction
        if ((energy > 60) && (age > (genes.maturity + genes.maturity * Random.Range(0f, 1f))))
        {
            Reproduce();
            energy -= 60;
        }

        // death
        if ((energy <= 0) || (age >= 100 * Random.Range(0.8f, 1.2f))) Die();
    }

    private void Reproduce()
    {
        Genes childGenes = genes.Reproduce();
        GameObject childBoid = Instantiate(this.gameObject, pos, transform.rotation);
        Boid child = childBoid.GetComponent<Boid>();
        child.init(pos + new Vector3(genes.power, 0, 0), vel, childGenes, manager);
        manager.addBoid(child);
    }

    private void Die()
    {
        manager.removeBoid(this);
    }

    // out-of-bounds boids die
    public void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("kill_box")) Die();
    }
}