using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowManager : MonoBehaviour
{
    private float timer;
    private List<Vector4> shadowList;
    private List<float> shadowRange;
    private List<float> shadowTravelSpeed;
    private List<int> shadowPriority;
    [SerializeField] private ShadowController controller;
    [SerializeField] private float shadowWidth;

    public List<Vector4> ShadowList { get => shadowList; set => shadowList = value; }
    public List<float> ShadowRange { get => shadowRange; set => shadowRange = value; }
    public List<float> ShadowTravelSpeed { get => shadowTravelSpeed; set => shadowTravelSpeed = value; }
    public List<int> ShadowPriority { get => shadowPriority; set => shadowPriority = value; }
    public ShadowController Controller { get => controller; set => controller = value; }

    // Start is called before the first frame update
    void Awake()
    {
        ShadowList = new List<Vector4>();
        ShadowRange = new List<float>();
        ShadowTravelSpeed = new List<float>();
        ShadowPriority = new List<int>();
        Controller.Material.SetFloat("_ShadowWidth", shadowWidth);
        Controller.shootEcho += spawnEcho;
        ShadowList.Add(new Vector4());
        ShadowRange.Add(0);
        ShadowTravelSpeed.Add(0);
        ShadowPriority.Add(int.MaxValue);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = ShadowList.Count - 1; i > 0; --i)
        {
            ShadowList[i] = new Vector4(ShadowList[i].x, ShadowList[i].y, ShadowList[i].z, ShadowList[i].w + ((ShadowTravelSpeed[i] * Time.deltaTime) / ShadowRange[i]));

            if (ShadowList[i].w >= 1)
            {
                try
                {
                    ShadowList.RemoveAt(i);
                    ShadowRange.RemoveAt(i);
                    ShadowTravelSpeed.RemoveAt(i);
                    ShadowPriority.RemoveAt(i);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.Log(i);
                }

            }
        }

        for (int i = 0; i < ShadowList.Count; ++i)
        {
            Controller.Points[i] = ShadowList[i];
            Controller.ShadowRange[i] = ShadowRange[i];
        }
        Controller.NbIndex = ShadowList.Count;
    }

    private void spawnEcho(Vector4 position, float echoRange, float echoTravelSpeed, int priority)
    {
        ShadowList.Add(position);
        this.ShadowRange.Add(echoRange);
        this.ShadowTravelSpeed.Add(echoTravelSpeed);
        this.ShadowPriority.Add(priority);
    }
}
