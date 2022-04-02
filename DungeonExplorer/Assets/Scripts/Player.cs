using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    public int speed;
    
    private float hAxis;
    private float vAxis;
    private Vector3 moveVec;
    private Vector3 moveDir;
    private bool walkbtnDown;

    private bool isJump;
    private bool jDown;
    private bool iDown;
    private bool sDown1;


    private Animator anim;
    private Rigidbody rigid;


    public int jumpPower;
    public Camera cam;

    public int coins;
    public int health;

    public int maxCoins;
    public int maxHealth;

    private GameObject nearObject;
   
    public GameObject[] weapons;
    public bool[] hasWeapons;
    private int equipWeaponIndex = -1;
    private GameObject equipWeapon;
    
    private bool isSwap;
    
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
    }


    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkbtnDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
       
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
            
        
        int weaponIndex = -1;

        if (sDown1)
            weaponIndex = 0;
        
        if (sDown1 && !isJump)
        {
            if(equipWeapon != null)
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);
            
            
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
            anim.SetBool("isJump",true);
            anim.SetTrigger("doJump");
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
        
        Debug.Log(nearObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        
    }
}
