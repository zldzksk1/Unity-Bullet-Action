using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemies
{
    public GameObject missile;
    public Transform positionA;
    public Transform positionB;

    Vector3 lookVec;
    Vector3 tauntVec;
    bool isLook = true;

    // 상속받아서 작성할 경우 Awake는 child script꺼만 작동됨
    // 해결방법은, awake코드를 그대로 복사해서 child script에 넣어주거나
    // parent script에서 awake를 start로 바꿔주면 됨
    // 하지만 start로 바꾸면 코드에 영향이 가기때문에 주로 복사해서 사용
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Processing());
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Processing()
    {
        //생각하는 시간이 길어질 수록 보스가 쉬워짐
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            //missaile
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            //rock attack
            case 3:
                StartCoroutine(RockShot());
                break;
            //taunt
            case 4:
                StartCoroutine(JumpShot());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, positionA.position, positionA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        Debug.Log("positionA " + instantMissileA.transform.localPosition + " " + instantMissileA.name);
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, positionB.position, positionB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        Debug.Log("positionB " + bossMissileB.gameObject.GetComponent<Transform>().position);
        bossMissileB.target = target;

        StartCoroutine(Processing());
    }
    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);

        yield return new WaitForSeconds(3f);
        isLook = true;

        StartCoroutine(Processing());

    }
    IEnumerator JumpShot()
    {
        tauntVec = target.position + lookVec;
        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");


        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Processing());

    }
}
