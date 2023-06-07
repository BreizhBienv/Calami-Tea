using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public List<BaseAI> list;
    }

    [System.Serializable]
    public class WaveList
    {
        public List<Wave> list;
    }
}
