using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeRange;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public int maxAmmo;
    public int currAmmo;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

        else if (type == Type.Range && currAmmo > 0)
        {
            currAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {

        yield return new WaitForSeconds(0.2f);
        meleeRange.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeRange.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

    }

    IEnumerator Shot()
    {
        //shot bullet
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return null;

        //trash bullet case
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();

        Vector3 caseDirection = bulletCasePos.forward * Random.Range(-3f, -2f) + Vector3.up * Random.Range(2,3);
        caseRigid.AddForce(caseDirection, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

}
