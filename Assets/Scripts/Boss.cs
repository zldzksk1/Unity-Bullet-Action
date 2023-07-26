using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemys
{
    public GameObject missile;
    public Transform positionA;
    public Transform positiobB;

    Vector3 lookVec;
    Vector3 tauntVec;
    bool isLook;

    // Start is called before the first frame update
    void Start()
    {
        isLook = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
    }
}
