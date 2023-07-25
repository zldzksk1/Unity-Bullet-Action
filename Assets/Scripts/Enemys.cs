using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemys : MonoBehaviour
{
    //enemyType
    public enum Type { A, B, C};
    public Type enemyType;

    //enement body
    public int maxHealth;
    public int currHealth;
    Rigidbody rigid;
    BoxCollider boxCollider;

    Material material;

    //attack target
    public Transform target;
    NavMeshAgent nav;

    Animator anim;
    bool isChase;

    //enemy attack style
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isAttack;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2f);
    }

    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity();
        targgeting();
    }

    void FreezeVelocity()
    {
        //When enemy collide with a player, if player pushs the enement, it keeps moving back because of the velocity
        //so make the velocity zero
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void targgeting()
    {
        float targetRadius = 0f;
        float tagetRange = 0f;

        switch (enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                tagetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                tagetRange = 10f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                tagetRange = 25f;
                break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, tagetRange, LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.5f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 30, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;

        }


        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);



    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapons weapon = other.GetComponent<Weapons>();
            currHealth -= weapon.damage;

            //dmage knock back
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));
        }
        else if(other.tag == "Bullet")
        {
            Bullets bullet = other.GetComponent<Bullets>();
            currHealth -= bullet.damage;

            //dmage knock back
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);


            StartCoroutine(OnDamage(reactVec, false));

        }
    }

    public void HitByGrenade(Vector3 grenadePos)
    {
        currHealth -= 55;
        Vector3 reactVec = transform.position - grenadePos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (currHealth > 0)
        {
            material.color = Color.white;
        }
        else
        {
            material.color = Color.grey;
            gameObject.layer = 14;

            anim.SetTrigger("doDie");
            anim.SetBool("isWalk", false);
            isChase = false;
            nav.enabled = false;

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);

            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            Destroy(gameObject, 4);
        }
    }

}
