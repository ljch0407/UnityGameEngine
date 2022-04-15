using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
   public int curHP;
   public int maxHP;
   public bool isChase;
   
   private Rigidbody rigid;
   private BoxCollider boxCollider;
   private Material material;

   private NavMeshAgent nav;
   private Animator anim;
   private void Awake()
   {
      rigid = GetComponent<Rigidbody>();
      boxCollider = GetComponent<BoxCollider>();
      material = GetComponentInChildren<MeshRenderer>().material;
      nav = GetComponent<NavMeshAgent>();
      anim = GetComponentInChildren<Animator>();
   }

   void ChasseStart()
   {
      isChase = true;
      anim.SetBool("isWalk",true);
   }

   private void Update()
   {
      if (isChase)
      {
         //네비메쉬로 추적하는 코드 필요, Collider하나 크게 넣어서 트리거 설정하고
         //플레이어가 들어오면 추적하도록 설정하면될듯함.
      }
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.tag == "Melee")
      {
         weapon weapon = other.GetComponent<weapon>();
         curHP -= weapon.damage;
         Vector3 reactVec = transform.position - other.transform.position;
         StartCoroutine(OnDamage(reactVec));
         Debug.Log("Melee : "+ curHP);
      }
      else if (other.tag == "Bullet")
      {
         Bullet bullet = other.GetComponent<Bullet>();
         curHP -= bullet.damage;
         Vector3 reactVec = transform.position - other.transform.position;
         Destroy(other.gameObject);
         StartCoroutine(OnDamage(reactVec));
         Debug.Log("Range : "+ curHP);
      }
   }

   IEnumerator OnDamage(Vector3 reactVec)
   {
      material.color = Color.red;
      yield return new WaitForSeconds(0.1f);

      if (curHP > 0)
      {
         material.color = Color.white;
         reactVec = reactVec.normalized;
         reactVec += Vector3.up;
         rigid.AddForce(reactVec * 3,ForceMode.Impulse);
      }
      else
      {
         material.color = Color.gray;
         gameObject.layer = 13;
         isChase = false;
         nav.enabled = false;
         anim.SetTrigger("doDie");

         reactVec = reactVec.normalized;
         reactVec += Vector3.up;
         rigid.AddForce(reactVec * 5,ForceMode.Impulse);
         
         Destroy(gameObject,4f);
      }
   }
}
