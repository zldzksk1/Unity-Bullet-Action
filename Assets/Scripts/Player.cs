using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameManager manager;
    public Camera followCamera;

    [SerializeField] float speed = 15;
    float hAxis;
    float vAxis;
    Vector3 moveVec;
    Vector3 dodgeVec;
    Rigidbody rigid;


    //player moveing key
    Animator anim;
    bool wDown;
    bool jDown;
    bool isJump;
    bool isDodge;
    bool isBorder;
    bool isDead = false;

    //player attack key
    bool fDown; //attack
    float fireDelay = 0;
    bool isFireReady = true;
    bool gDown; //grenade

    bool rDown; //reload gun
    bool isReload;

    public bool isShop;

    //weapon key
    GameObject nearByItem;
    public Weapons equipWeapon;
    int equipWeaponIdx = -1;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    bool isSwap;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    public int score;

    public int ammo;
    public int maxAmmo;

    public int coin;
    public int maxCoin;

    public int health;
    public int maxHealth;
    bool isDamage;
    MeshRenderer[] meshs;

    public GameObject[] grenades;
    public GameObject grenade;
    public int hasGrenades;
    public int maxGrenades;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        PlayerPrefs.SetInt("BestScore", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        if (!isDead)
        {
            Move();
            Turn();
            Jump();
            Dodge();
            Swap();
            Interaction();
            Attack();
            useGrenade();
            Reload();
        }

    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void FreezeRotation()
    {
        //angularVelocity = roation speed
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");

        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");

        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || !isFireReady || isReload)
            moveVec = Vector3.zero;

        if (!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.5f : 1f) * Time.deltaTime;
    
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

        if (fDown && equipWeapon != null)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }

    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isShop)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            isJump = true;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");

        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isShop)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapons.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }

    }

    void useGrenade()
    {
        if (hasGrenades == 0)
        {
            return;
        }

        if (gDown && !isReload && !isSwap)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenade, transform.position, transform.rotation);
                Rigidbody rigidGrenande = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenande.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenande.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }

    void Reload()
    {   if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapons.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if (rDown && !isDodge && !isJump && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.currAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        int weaponIdx = -1;

        if (sDown1 && (!hasWeapons[0] || equipWeaponIdx == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIdx == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIdx == 2))
            return;


        if (sDown1)
            weaponIdx = 0;
        if (sDown2)
            weaponIdx = 1;
        if (sDown3)
            weaponIdx = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIdx = weaponIdx;
            // Debug.Log(weapons);
            equipWeapon = weapons[weaponIdx].GetComponent<Weapons>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("swapOut", 0.4f);
        }
    }

    void swapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if (iDown && nearByItem != null && !isJump && !isDodge)
        {
            if (nearByItem.tag == "Weapon")
            {
                Items item = nearByItem.GetComponent<Items>();
                int weaponIdx = item.value;
                hasWeapons[weaponIdx] = true;

                Destroy(nearByItem);
            }

            else if (nearByItem.tag == "Shop")
            {
                isShop = true;
                Shop shop = nearByItem.GetComponent<Shop>();
                shop.Enter(this);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearByItem = other.gameObject;
            //Debug.Log(nearByItem.name);
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapobn")
            nearByItem = null;
        else if (other.tag == "Shop")
        {
            Shop shop = nearByItem.GetComponent<Shop>();
            isShop = false;
            shop.Exit();
            nearByItem = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Items item = other.GetComponent<Items>();
            switch (item.type)
            {
                case Items.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Items.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Items.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;

                case Items.Type.Grenade:

                    grenades[hasGrenades].SetActive(true);

                    hasGrenades += item.value;
                    if (hasGrenades > maxGrenades)
                        hasGrenades = maxGrenades;
                    break;
            }

            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullets enemyBullet = other.GetComponent<Bullets>();
                health -= enemyBullet.damage;

                if (health < 0)
                    health = 0;


                bool isBossAttack = other.name == "Boss Melee Area";

                StartCoroutine(OnDamage(isBossAttack));
            }


            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }

        }
    }

    IEnumerator OnDamage(bool isBossAttack) 
    {
        isDamage = true;
        //Damages vision notification
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        if (isBossAttack)
        {
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }

        if (health <= 0 && !isDead)
        {
            OnDie();
        }

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAttack)
        {
            rigid.velocity = Vector3.zero;
        }


    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        manager.GameOver();
    }
}

