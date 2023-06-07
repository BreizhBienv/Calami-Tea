using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private Material material;
    private Vector4[] points;
    private float[] shadowRange;
    private int nbIndex;
    public delegate void EventHandler(Vector4 position, float echoRange, float echoTravelSpeed, int priority);
    public event EventHandler shootEcho = delegate { };

    public Material Material { get => material; set => material = value; }
    public Vector4[] Points { get => points; set => points = value; }
    public float[] ShadowRange { get => shadowRange; set => shadowRange = value; }
    public int NbIndex { get => nbIndex; set => nbIndex = value; }


    // Start is called before the first frame update
    void Start()
    {
        Points = new Vector4[750];
        ShadowRange = new float[750];
        nbIndex = Points.Length;
        Material.SetInt("_PointsSize", nbIndex);
        Material.SetVectorArray("_Points", Points);
        Material.SetFloatArray("_ShadowRange", ShadowRange);
    }

    // Update is called once per frame
    void Update()
    {
        Material.SetInt("_PointsSize", nbIndex);
        Material.SetVectorArray("_Points", Points);
        Material.SetFloatArray("_ShadowRange", ShadowRange);
    }

    public void Fire(Vector4 position, float ShadowRange, float ShadowTravelSpeed, int priority, int nbShadow)
    {
        StartCoroutine(additionEffect(position, ShadowRange, ShadowTravelSpeed, priority, nbShadow));
    }

    private IEnumerator additionEffect(Vector4 position, float ShadowRange, float ShadowTravelSpeed, int priority, int nbShadow)
    {
        for (int i = 0; i < nbShadow; ++i)
        {
            shootEcho.Invoke(position, ShadowRange, ShadowTravelSpeed, priority);

            yield return new WaitForSeconds(0.2f * (ShadowRange / ShadowTravelSpeed));
        }
    }
}
