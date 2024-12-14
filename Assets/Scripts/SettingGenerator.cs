using UnityEngine;

public class SettingGenerator : MonoBehaviour
{

    /* terrain generation code from Brackey's procedural terrain mesh generation video */

    // terrain settings
    public int size;
    public float height;
    public float min;
    public float max;
    public float offset;
    public float intensity;
    public Gradient gradient;
    public Color[] gradientColors;

    // terrain info
    private Vector3[] verticies;
    private int[] triangles;
    private Color[] colors;
    private Mesh mesh;
    private MeshCollider col;

    // ocean color
    public Color[] oceanColors;

    private void Start()
    {
        // initialize terrain settings
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        col = GetComponent<MeshCollider>();
        size = Mathf.Clamp(size, 1, 255);
        offset = Random.Range(1, 255);
        intensity = Random.Range(0.2f, 1f);

        // get terrain colors
        GradientColorKey[] colorKeys = gradient.colorKeys;
        for (int i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i].color = gradientColors[Random.Range(0, gradientColors.Length)];
        }
        gradient.colorKeys = colorKeys;

        // get ocean color
        Color color = oceanColors[Random.Range(0, oceanColors.Length)];
        RenderSettings.fogColor = color;
        Camera.main.backgroundColor = color;
        GameObject.Find("Sun").GetComponent<Light>().color = color;

        CreateShape();
        UpdateMesh();
    }

    private void CreateShape()
    {
        // place vertices on perlin noise
        int v, i = 0;
        verticies = new Vector3[(size + 1) * (size + 1)];
        for (int z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float y = Mathf.PerlinNoise(offset + x * intensity / 10, offset + z * intensity / 10) * height;
                y = Mathf.Clamp(y, min, max) - min;
                verticies[i] = new Vector3(x, y, z);
                i++;
            }
        }

        // calculate number of triangles
        i = 0;
        v = 0;
        triangles = new int[size * size * 6];
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                triangles[i] = v;
                triangles[i + 1] = v + size + 1;
                triangles[i + 2] = v + 1;
                triangles[i + 3] = v + 1;
                triangles[i + 4] = v + size + 1;
                triangles[i + 5] = v + size + 2;
                v++;
                i += 6;
            }
            v++;
        }

        // determine triangle color based on height
        i = 0;
        colors = new Color[(size + 1) * (size + 1)];
        for (int z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float height = Mathf.InverseLerp(0, max - min, verticies[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    // create mesh
    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        col.sharedMesh = mesh;
    }
}
