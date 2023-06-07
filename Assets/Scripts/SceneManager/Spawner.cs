using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static WaveManager;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private List<Chekpoint> chekpoints;

    public PlayerComponent[] Players;

    // Start is called before the first frame update
    private void Start()
    {
        chekpoints[0].SpawnAiWave();
        Players = FindObjectsOfType<PlayerComponent>();
    }

    // Update is called once per frame
    private void Update()
    {
        BaseCharacter.MainStates States = BaseCharacter.MainStates.Dead;
        for (int i = 0; i < Players.Length; i++)
        {
            States = Players[i].CharacterState;
            if (States != BaseCharacter.MainStates.Dead)
                break;
        }
        if (States == BaseCharacter.MainStates.Dead)
        {
            UnityEngine.Vector3 CheckpointPosition = new UnityEngine.Vector3(0, 0, 0);
            for (int i = 0; i < chekpoints.Count; i++)
            {
                if (chekpoints[i].gameObject.GetComponent<BoxCollider>().enabled == false)
                {
                    CheckpointPosition = chekpoints[i].gameObject.transform.position;
                }
            }

            for (int i = 0; i < Players.Length; i++)
            {
                bool a = true;
                foreach (BaseCharacter player in Players)
                {
                    if (player.CharacterState != BaseCharacter.MainStates.Dead)
                        a = false;
                }
                if (a)
                {
                    Players[i].Revive();
                    Players[i].transform.position = CheckpointPosition;
                }
            }
        }
    }

    public void WaveFinished(Chekpoint chekpoint)
    {
        for (int i = 0; i < chekpoints.Count; i++)
        {
            if (chekpoints[0] == chekpoint)
            {
                chekpoint.gameObject.GetComponent<BoxCollider>().enabled = false;
                if (chekpoints[chekpoints.Count - 1] != chekpoint)
                {
                    chekpoints[i + 1].SpawnAiWave();
                    break;
                }
                else
                {
                    //Scene Done
                }
            }
            else if (chekpoints[i] == chekpoint)
            {
                chekpoint.gameObject.GetComponent<BoxCollider>().enabled = false;
                chekpoints[i - 1].gameObject.GetComponent<BoxCollider>().enabled = true;
                if (chekpoints[chekpoints.Count - 1] != chekpoint)
                {
                    chekpoints[i + 1].SpawnAiWave();
                    break;
                }
                else
                {
                    //Scene Done
                }
            }
        }
    }
}