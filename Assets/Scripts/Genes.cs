using UnityEngine;

[System.Serializable]
public class Genes
{
    public int species;
    public Genes ancestor;
    public Color color;

    public float maturity;
    public float vision;
    public float power;
    public float speed;
    public float separationWeight;
    public float alignmentWeight;
    public float cohesionWeight;

    public Genes Reproduce()
    {
        Genes child = new Genes()
        {
            species = this.species,
            ancestor = this.ancestor ?? this,
            color = this.color,
            maturity = this.maturity,
            vision = this.vision,
            power = this.power,
            speed = this.speed,
            separationWeight = this.separationWeight,
            alignmentWeight = this.alignmentWeight,
            cohesionWeight = this.cohesionWeight
        };

        if (Random.value < Stats._MUTATION_CHANCE)
        {
            Mutate(child, Stats._MUTATION_FACTOR * Random.Range(0.8f, 1.2f));
        }

        if (DeviationCheck(child)) 
        {
            child.species = Stats.NewID();
            child.ancestor = child;
            child.color = new Color(Random.value, Random.value, Random.value);
        }

        return child;
    }

    private void Mutate(Genes gene, float factor)
    {
        int stat = Random.Range(0, 6);
        switch (stat)
        {
            case 0:
                gene.vision += Random.Range(-factor, factor);
                gene.vision = Mathf.Max(gene.vision, 0.1f);
                break;
            case 1:
                gene.power += Random.Range(-factor, factor);
                gene.power = Mathf.Max(gene.power, 0.1f);
                break;
            case 2:
                gene.speed += Random.Range(-factor, factor);
                gene.speed = Mathf.Max(gene.speed, 0.1f);
                break;
            case 3:
                gene.separationWeight += Random.Range(-factor / 10, factor / 10);
                gene.separationWeight = Mathf.Max(gene.separationWeight, 0.01f);
                break;
            case 4:
                gene.alignmentWeight += Random.Range(-factor / 10, factor / 10);
                gene.alignmentWeight = Mathf.Max(gene.alignmentWeight, 0.01f);
                break;
            case 5:
                gene.cohesionWeight += Random.Range(-factor / 10, factor / 10);
                gene.cohesionWeight = Mathf.Max(gene.cohesionWeight, 0.01f);
                break;
        }
    }

    private bool DeviationCheck(Genes child) 
    {
        float visionDiff = Mathf.Abs(child.ancestor.vision - child.vision);
        float powerDiff = Mathf.Abs(child.ancestor.power - child.power);
        float speedDiff = Mathf.Abs(child.ancestor.speed - child.speed);
        float separationDiff = Mathf.Abs(child.ancestor.separationWeight * 10 - child.separationWeight * 10);
        float alignmentDiff = Mathf.Abs(child.ancestor.alignmentWeight * 10 - child.alignmentWeight * 10);
        float cohesionDiff = Mathf.Abs(child.ancestor.cohesionWeight * 10 - child.cohesionWeight * 10);
        float deviation = visionDiff + powerDiff + speedDiff + separationDiff + alignmentDiff + cohesionDiff;
        return deviation > Stats._DEVIATION_THRESHOLD;
    }
}