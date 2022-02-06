using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clipboard : MonoBehaviour
{

    //public GameObject clipBoard;
    public Camera theCam;


    void Start()
    {
        gameObject.transform.position = theCam.transform.position; //      new Vector3(theCam.transform.position.x, theCam.transform.position.y, theCam.transform.position.z);
    }

    void Update()
    {
     if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            gameObject.transform.position = new Vector3(theCam.transform.position.x - 0.2f, theCam.transform.position.y, theCam.transform.position.z + 0.5f);
        }

     if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            gameObject.transform.position = new Vector3(theCam.transform.position.x - 2, theCam.transform.position.y, theCam.transform.position.z);
        }
    }
}
