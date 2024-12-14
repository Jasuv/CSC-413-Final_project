using UnityEngine;

public static class Stats
{
    // constants
    public static int _STARTING_POINTS = 20;
    public static float _MUTATION_CHANCE = 0.45f;
    public static float _MUTATION_FACTOR = 1f;
    public static float _DEVIATION_THRESHOLD = 6f;
    public static float _ENERGY_DRAIN = 1.5f;
    
    // variable
    public static int _SPECIES = 0;
    public static float _TIME_MULTIPLIER = 0.1f;
    public static float _SIMULATION_TIME
    {
        get { return Time.deltaTime * _TIME_MULTIPLIER; }
    }

    // game stats
    public static int points = _STARTING_POINTS;
    public static int population = 0;
    public static int foodAmount = 0;
    public static Boid boidDisplay;

    public static bool newSpecies = false; // for audio soun

    public static int NewID()
    {
        newSpecies = true;
        return ++_SPECIES;
    }
}
