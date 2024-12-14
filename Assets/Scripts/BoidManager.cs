using System.Collections.Generic;
using UnityEngine;

// boid data struct for compute shader
public struct BoidData
{
    // input
    public Vector3 pos;
    public Vector3 vel;
    public int species;
    public float vision;
    public float power;

    // output
    public Vector3 separationVec;
    public Vector3 alignmentVec;
    public Vector3 cohesionVec;
}

public class BoidManager : MonoBehaviour
{
    // spawn settings
    public int radius;
    public GameObject boidObject;
    public Transform target;
    public bool follow;
    public List<Boid> Boids;
    public ComputeShader boidMath;
    public bool debug;

    private int threadGroups;
    private BoidData[] boidData;
    private ComputeBuffer boidBuffer;
    private LayerMask terrain;

    // vectors
    private Vector3 separationVec;
    private Vector3 alignmentVec;
    private Vector3 cohesionVec;
    private Vector3 obstacleVec;
    private Vector3 targetVec;

    private bool updating;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        threadGroups = Mathf.CeilToInt(Stats.population / 1024f);
        terrain = LayerMask.GetMask("terrain");
        if (debug) SpawnBoids(2000);
    }

    public void SpawnBoids(int num)
    {
        if (Stats.points < 30) return;
        Stats.points -= 30;
        for (int i = 0; i < num; i++)
        {
            Boid boid = Instantiate(boidObject, transform.position, transform.rotation).GetComponent<Boid>();
            Vector3 vel = new Vector3(Random.value < 0.5f ? 1 : -1,
                                      Random.value < 0.5f ? 1 : -1,
                                      Random.value < 0.5f ? 1 : -1) * boid.genes.speed;
            Vector3 pos = Random.insideUnitSphere * radius + transform.position;
            boid.init(pos, vel, boid.genes, this);
            Boids.Add(boid);
        }
        Stats.population += num;
        updateBuffer();
    }

    private void Update()
    {
        if (Stats.population > 0 && !updating && Stats._SIMULATION_TIME > 0)
        {
            // update boid data
            for (int i = 0; i < Stats.population; i++)
            {
                boidData[i].pos = Boids[i].pos;
                boidData[i].vel = Boids[i].vel;
            }
            boidBuffer.SetData(boidData);

            // dispatch shader
            boidMath.Dispatch(0, threadGroups, 1, 1);

            // get shader data
            boidBuffer.GetData(boidData);

            for (int i = 0; i < Stats.population; i++)
            {
                Boid boid = Boids[i];
                BoidData data = boidData[i];

                // get vectors from boidData
                separationVec = data.separationVec * boid.genes.separationWeight;
                alignmentVec = data.alignmentVec * boid.genes.alignmentWeight;
                cohesionVec = data.cohesionVec * boid.genes.cohesionWeight;

                if (debug)
                {
                    Debug.DrawRay(boid.pos, separationVec.normalized, Color.green);   // Separation force
                    Debug.DrawRay(boid.pos, alignmentVec.normalized, Color.cyan);     // Alignment force
                    Debug.DrawRay(boid.pos, cohesionVec.normalized, Color.magenta);   // Cohesion force
                }

                // check for obstacles
                RaycastHit hit;
                if (debug) Debug.DrawRay(boid.pos, boid.vel.normalized * boid.genes.vision, Color.blue);
                if (Physics.Raycast(boid.pos, boid.vel, out hit, boid.genes.vision, terrain)) ObstacleAvoidance(boid);
                else obstacleVec = Vector3.zero;

                // check for target
                if (follow) MoveToTarget(boid, target.position);

                // add vectors
                boid.vel += separationVec + alignmentVec + cohesionVec + (obstacleVec * 100 * Stats._TIME_MULTIPLIER) + targetVec * (follow ? 1 : 0);

                // clamp speed
                boid.vel = boid.vel.normalized * Mathf.Clamp(boid.vel.magnitude, 1, boid.genes.speed * 10);

                // update boid position
                boid.pos += boid.vel * Stats._SIMULATION_TIME;
            }
        }
    }

    private void updateBuffer()
    {
        updating = true;

        if (boidBuffer != null)
        {
            boidBuffer.Release();
            boidBuffer = null;
        }

        if (Stats.population > 0)
        {
            boidData = new BoidData[Stats.population];
            for (int i = 0; i < Stats.population; i++)
            {
                boidData[i].pos = Boids[i].pos;
                boidData[i].vel = Boids[i].vel;
                boidData[i].species = Boids[i].genes.species;
                boidData[i].vision = Boids[i].genes.vision;
                boidData[i].power = Boids[i].genes.power;
            }

            threadGroups = Mathf.CeilToInt(Stats.population / 1024f);
            boidBuffer = new ComputeBuffer(Stats.population, sizeof(float) * 17 + sizeof(int) * 1);
            boidBuffer.SetData(boidData);
            boidMath.SetBuffer(0, "Boids", boidBuffer);
            boidMath.SetInt("count", Stats.population);
        }

        updating = false;
    }

    private void ObstacleAvoidance(Boid boid)
    {
        RaycastHit hit;
        Vector3 avoidVec = Vector3.zero;

        Vector3[] dirs = {
            boid.transform.right, -boid.transform.right,
            boid.transform.up, -boid.transform.up,
            (boid.transform.right + boid.transform.forward).normalized,
            (-boid.transform.right + boid.transform.forward).normalized,
            (boid.transform.up + boid.transform.forward).normalized,
            (-boid.transform.up + boid.transform.forward).normalized
        };

        foreach (Vector3 dir in dirs)
        {
            if (Physics.Raycast(boid.pos, dir, out hit, boid.genes.vision, terrain))
            {
                if (debug) Debug.DrawRay(boid.pos, dir * boid.genes.vision, Color.red);
                avoidVec -= dir * (2 / hit.distance + 0.001f);
            }
            else
            {
                if (debug) Debug.DrawRay(boid.pos, dir * boid.genes.vision, Color.yellow);
            }
        }

        obstacleVec = avoidVec;
    }

    private void MoveToTarget(Boid boid, Vector3 target)
    {
        targetVec = (target - boid.pos).normalized * boid.genes.speed * 0.01f;
    }

    public void addBoid(Boid boid)
    {
        Boids.Add(boid);
        Stats.population++;
        updateBuffer();
    }

    public void removeBoid(Boid boid)
    {
        Boids.Remove(boid);
        Stats.population--;
        Destroy(boid.gameObject);
        updateBuffer();
    }

    // weird fix
    private void OnDestroy()
    {
        if (boidBuffer != null)
        {
            boidBuffer.Release();
            boidBuffer = null;
        }

        foreach (var boid in Boids)
        {
            if (boid != null)
                Destroy(boid.gameObject);
        }
        Boids.Clear();
    }
}