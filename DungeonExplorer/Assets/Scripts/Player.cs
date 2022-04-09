using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    //플레이어 public 변수
    public int speed;
    public int jumpPower;
    
    public int coins;
    public int health;

    public int maxCoins;
    public int maxHealth;
    
    //플레이어 움직임 변수
    private float hAxis;
    private float vAxis;
    private Vector3 moveVec;
    private Vector3 moveDir;
    private bool walkbtnDown;

    //플레이어 행동 제어 bools
    private bool isJump;
    private bool jDown;
    private bool iDown;
    private bool sDown1;
    private bool sDown2;
    private bool sDown3;
    private bool fDown;
    private bool isSwap;
    private bool isFireReady;

    //플레이어 객체 compomemt
    private Animator anim;
    private Rigidbody rigid;
    
    //카메라 
    public Camera cam;
    
    //주변 오브젝트 검사용 
    private GameObject nearObject;
    
    //무기 배열
    public GameObject[] weapons;
    public bool[] hasWeapons;
   
    //무기관련 변수
    private int equipWeaponIndex = -1;
    private weapon equipWeapon;
    private float fireDelay;
    
    void Awake()
    {
        
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
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
       
    }


    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.attackRate < fireDelay;

        if (fDown && isFireReady && !isSwap)
        {
            equipWeapon.useWeapon();
            anim.SetTrigger(equipWeapon.type == weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
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

        if (isSwap)
            moveVec = Vector3.zero;
        
        transform.Translate(moveVec*speed*(walkbtnDown ? 0.3f : 1.0f) *Time.deltaTime,Space.Self);
        anim.SetBool("isRun",moveVec != Vector3.zero);
        anim.SetBool("isWalk",walkbtnDown);
    }

    void Turn()
    { 
        var offset = cam.transform.forward;
        offset.y = 0;
        transform.LookAt(transform.position + offset);

    }

    void Jump()
    {
        if (jDown && !isJump && !isSwap)
        {
            rigid.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);
            anim.SetTrigger("doJump");
            anim.SetBool("isJump", true);
            isJump = true;
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
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        
    }
}
