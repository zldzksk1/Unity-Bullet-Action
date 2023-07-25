using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public enum Type
    { 
        Ammo, Coin, Grenade, Heart, Weapon
    };

    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    [SerializeField] float speed = 20f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
