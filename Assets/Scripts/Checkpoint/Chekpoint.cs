using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chekpoint : MonoBehaviour
{
    [SerializeField]
    GameObject[] AiActiveInZone;

    private List<GameObject> Ai;

    [SerializeField]
    Transform[] AiSpawnPoint;

    [SerializeField]
    Spawner spawner;

    // Start is called before the first frame update
    void Awake()
    {
        Ai = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        bool ZoneCleared = true;
        if(Ai.Count > 0)
        {
            for (int i = 0; i < Ai.Count; i++)
            {
                if (Ai[i] != null)
                {
                    ZoneCleared = false;
                    break;
                }
            }
            if(ZoneCleared)
            {
                spawner.WaveFinished(this);
                this.enabled = false;
            }
        }
    }

    public void SpawnAiWave()
    {
        for(int i = 0; i < AiActiveInZone.Length; i++)
        {
            GameObject newAI = Instantiate(AiActiveInZone[i]);
            newAI.transform.position = AiSpawnPoint[i].position;
            Ai.Add(newAI);
        }
    }
}
