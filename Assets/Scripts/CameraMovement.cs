using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform Target;
    public Vector3 offset;
    // Start is called before the first frame update

    void Awake()
    {
        offset = transform.position;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Target.position + offset;
    }
}
