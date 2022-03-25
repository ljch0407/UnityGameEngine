using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    private Animator anim;
    private Rigidbody rigid;


    public int jumpPower;
    public Camera cam;

    public int coins;
    public int health;

    public int maxCoins;
    public int maxHealth;

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
    }


    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkbtnDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis);
        
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
        if (jDown && !isJump)
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
}
