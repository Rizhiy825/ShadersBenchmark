using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelViewer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(Vector3.down, Input.GetAxis("Mouse X"), Space.World);
        }
        //transform.Rotate(Vector3.left, Input.GetAxis("Mouse Y"), Space.World);
    }
}
