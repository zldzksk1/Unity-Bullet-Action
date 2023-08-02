using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    private void Update()
    {
        checkOutOfRange();
    }

    void checkOutOfRange()
    {
        //some reason, bullters does not get destoyed,
        //so this will check the bullet position, and if it goes out the bound
        //the object will be destroyed 
        if (this.transform.position.y > 150 || this.transform.position.x > 150 || this.transform.position.z > 150 ||
            this.transform.position.y < 0 || this.transform.position.x < -150 || this.transform.position.z < -150)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
