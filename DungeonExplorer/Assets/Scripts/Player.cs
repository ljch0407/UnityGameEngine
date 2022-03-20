using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int speed;
    
    private float hAxis;
    private float vAxis;
    private Vector3 moveVec;
    private bool walkbtnDown;

    private Animator anim;
    
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkbtnDown = Input.GetButton("Walk");
        
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        
        transform.position += moveVec * speed * (walkbtnDown ? 0.3f : 1f ) * Time.deltaTime;
        
        anim.SetBool("isRun",moveVec != Vector3.zero);
        anim.SetBool("isWalk",walkbtnDown);
        
        transform.LookAt(transform.position + moveVec);
    }
}
