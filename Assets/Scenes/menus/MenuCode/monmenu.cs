using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//sing UnityEngine.Input;

public class monmenu : MonoBehaviour
{
    
    public void option()
    {
        SceneManager.LoadScene(1);
    }


    public void Play()
    {
        SceneManager.LoadScene(4);
    }

    public void creditemoisvp()
    {
        SceneManager.LoadScene(2);
    }

    public void exit()
    {
        Application.Quit();
    }



}




