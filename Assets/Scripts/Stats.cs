using UnityEngine;
using UnityEngine.AI;

public static class Stats
{
    public static int _STARTING_POINTS = 50;
    public static float _MUTATION_CHANCE = 0.45f;
    public static float _MUTATION_FACTOR = 1f;
    public static float _DEVIATION_THRESHOLD = 6f;
    public static float _ENERGY_DRAIN = 3f;
    public static int _SPECIES = 0;
    public static float _TIME_MULTIPLIER = 0.1f;

    public static float _SIMULATION_TIME
    {
        get { return Time.deltaTime * _TIME_MULTIPLIER; }
    }

    public static int points = 50;
    public static int population = 0;
    public static int foodAmount = 0;

    public static Boid boidDisplay;

    public static int NewID()
    {
        return ++_SPECIES;
    }
}
