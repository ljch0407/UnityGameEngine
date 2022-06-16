using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    //플레이어 public 변수
    public int speed;
    public int jumpPower;
    
    public int coins;
    public int health;

    public int maxCoins;
    public int maxHealth;

    public int wing;
    public int maxWing;
    
    public int ammo;
    public int maxAmmo;

    public int trapForce;
    public float rotSpeed;
    
    //플레이어 움직임 변수
    private float hAxis;
    private float vAxis;
    private Vector3 moveVec;
    private Vector3 moveDir;
    private bool walkbtnDown;
    private float yRot;

    //플레이어 행동 제어 bools
    private bool isJump;
    private bool jDown;
    private bool iDown;
    private bool sDown1;
    private bool sDown2;
    private bool sDown3;
    private bool fDown;
    private bool rDown;
    private bool isSwap;
    private bool isReload;
    private bool isFireReady;
    private bool isBorder;
    private bool isDamage;

    // 함정에 맞았을 때
    private bool isTrapped;
    
    //플레이어 객체 compomemt
    private Animator anim;
    private Rigidbody rigid;
    private MeshRenderer[] meshs;
    
    //카메라 
    public Camera cam;
    
    //주변 오브젝트 검사용 
    private GameObject nearObject;
    
    //무기 배열
    public GameObject[] weapons;
    public bool[] hasWeapons;
   
    //무기관련 변수
    private int equipWeaponIndex = -1;
    public weapon equipWeapon;
    private float fireDelay;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Interaction();
        Swap();
        Attack();
        Reload();
    }


    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkbtnDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        rDown = Input.GetButtonDown("Reload");

    }


    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.attackRate < fireDelay;
       
        if (equipWeapon.type == weapon.Type.Range && equipWeapon.curAmmo == 0)
            return;

        if (fDown && isFireReady && !isSwap)
        {
            equipWeapon.useWeapon();
            anim.SetTrigger(equipWeapon.type == weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null)
            return;
        
        if(equipWeapon.type == weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if (rDown && !isJump && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 0.5f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;
        
        int weaponIndex = -1;

        if (sDown1)
            weaponIndex = 0;
        if (sDown2)
            weaponIndex = 1;
        if (sDown3)
            weaponIndex = 2;
        
        if ((sDown1||sDown2||sDown3) && !isJump)
        {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<weapon>();
            equipWeapon.gameObject.SetActive(true);
            
            
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut",0.4f);
        }

    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if (iDown && nearObject != null && !isJump) {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;
                
                Destroy(nearObject);
            }
        }
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis);

        if (isSwap || isReload)
            moveVec = Vector3.zero;
        
        if(!isBorder)
            transform.Translate(moveVec*speed*(walkbtnDown ? 0.3f : 1.0f) *Time.deltaTime,Space.Self);
       
        anim.SetBool("isRun",moveVec != Vector3.zero);
        anim.SetBool("isWalk",walkbtnDown);
    }

    void Turn()
    { 
        yRot += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0.0f, yRot, 0.0f);
            
        // var offset = cam.transform.forward;
        // offset.y = 0;
        // transform.LookAt(transform.position + offset);

    }

    void Jump()
    {
        if (jDown && !isSwap)
        {
            if (!isJump)
            {
                rigid.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);
                anim.SetTrigger("doJump");
                anim.SetBool("isJump", true);
                isJump = true;
            }
            else
            {
                if (wing > 0)
                {
                    rigid.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);
                    anim.SetTrigger("doJump");
                    anim.SetBool("isJump", true);
                    isJump = true;
                    wing -= 1;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump",false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Coin:
                    coins += item.value;
                    if (coins > maxCoins)
                        coins = maxCoins;
                    break;
                case Item.Type.Ammo:
                    if (equipWeapon.type == weapon.Type.Range)
                    {
                        equipWeapon.curAmmo += item.value;
                        if (equipWeapon.curAmmo > equipWeapon.maxAmmo)
                            equipWeapon.curAmmo = equipWeapon.maxAmmo;
                    }
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Wing:
                    wing += item.value;
                    if (wing > maxWing)
                        wing = maxWing;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
               
                StartCoroutine(OnDamage());
            }
            
            if(other.GetComponent<Rigidbody>()!=null)
                Destroy(other.gameObject);
        }
        else if(other.tag == "InstantKill")
        {
            //체크포인트로 돌아가기
        }
    }

    IEnumerator OnDamage()
    {
        isDamage = true;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        if (other.tag == "Trap" && !isTrapped)
        {
            var particle = other.gameObject.GetComponentInChildren<ParticleSystem>();

            if (particle.particleCount > 0)
            {
                health -= 1;
                Vector3 dir = new Vector3(0, 0, 1);
                dir = -(other.transform.rotation * dir).normalized;

                var force = trapForce * dir;
                force.y = jumpPower;

                rigid.AddForce(force, ForceMode.Impulse);
                isTrapped = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;

        if (other.tag == "Trap" && isTrapped)
        {
            isTrapped = false;
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
}
