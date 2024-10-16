using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpinManager : MonoBehaviour
{

    public int AutoSpinCount = 5;


    void StartAutoSpin()
    {
        for(int i = 0; i< AutoSpinCount; i++)
        {
            EventManager.Play();
        }
    }

    
}
