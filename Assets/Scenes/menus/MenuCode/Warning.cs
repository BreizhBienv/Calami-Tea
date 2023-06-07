using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{

    public GameObject warning1;
    public GameObject warning2;
    public GameObject warning3;
    // Start is called before the first frame update
    void Start()
    {
        warning1.gameObject.SetActive(false);
        warning2.gameObject.SetActive(false);
        warning3.gameObject.SetActive(false);
    }

    
    public void attention1()
    {
        warning1.gameObject.SetActive(true);
    }
    public void attention2()
    {
        warning2.gameObject.SetActive(true);
    }
    public void attention3()
    {
        warning3.gameObject.SetActive(true);
    }
}
