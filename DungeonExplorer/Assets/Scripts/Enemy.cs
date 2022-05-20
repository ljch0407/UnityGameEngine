using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
   public enum Type
   {
      A,
      B,
      C,
      D
   };

   public Type enemyType;
   public int curHP;
   public int maxHP;
   public bool isChase;
   public BoxCollider meleeArea;
   public bool isAttack;

   public Rigidbody rigid;
   public BoxCollider boxCollider;
   public MeshRenderer[] meshs;
   public SphereCollider sphereCollider;
 
   public NavMeshAgent nav;
   public Animator anim;

   public GameObject bullet;
   public Transform target;
   public bool isDead;
   private void Awake()
   {
      rigid = GetComponent<Rigidbody>();
      boxCollider = GetComponent<BoxCollider>();
      meshs = GetComponentsInChildren<MeshRenderer>();
      nav = GetComponent<NavMeshAgent>();
      sphereCollider = GetComponent<SphereCollider>();
      anim = GetComponentInChildren<Animator>();
   }

   void ChasseStart()
   {
      isChase = true;
      anim.SetBool("isWalk",true);
   }

   private void Update()
   {
      if (isChase && enemyType != Type.D)
         {
            nav.SetDestination(target.position);
         }
   }

   void FreezeVelocity()
   {
      if (isChase)
      {
         rigid.velocity = Vector3.zero;
         rigid.angularVelocity = Vector3.zero;
      }
   }

   private void FixedUpdate()
   {
      FreezeVelocity();
      Targeting();
   }

   void Targeting()
   {
      float targetRadius = 0;
      float targetRange = 0;
      if (!isDead && enemyType != Type.D)
      {
         switch (enemyType)
         {
            case Type.A:
               targetRadius = 1.5f;
               targetRange = 3f;
               break;
            case Type.B:
               targetRadius = 1f;
               targetRange = 12f;
               break;
            case Type.C:
               targetRadius = 1f;
               targetRange = 25f;
               break;
         }

         RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
            targetRadius, transform.forward,
            targetRange, LayerMask.GetMask("Player"));

         if (rayHits.Length > 0 && !isAttack)
         {
            StartCoroutine(Attack());
         }
      }
   }

   IEnumerator Attack()
   {
      isChase = false;
      isAttack = true;
      
      anim.SetBool("isWalk",false);
      anim.SetBool("isAttack",true);

      switch (enemyType)
      {
         case Type.A:
            yield return new WaitForSeconds(0.2f);
            meleeArea.enabled = true;
            yield return new WaitForSeconds(1f);
            meleeArea.enabled = false;
            yield return new WaitForSeconds(1f);
            break;
         case Type.B:
            yield return new WaitForSeconds(0.1f);
            rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
            meleeArea.enabled = true;
            yield return new WaitForSeconds(0.5f);
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
      
      anim.SetBool("isAttack",false);
      anim.SetBool("isWalk",true);
      
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
      else if (other.tag == "Player")
      {
         if (enemyType != Type.D)
         {
            isChase = true;
            anim.SetBool("isWalk", true);
         }
         sphereCollider.enabled = false;
         target = other.gameObject.transform;
      }
   }

   IEnumerator OnDamage(Vector3 reactVec)
   {
      foreach (MeshRenderer mesh in meshs)
         mesh.material.color = Color.red;
      
    
      yield return new WaitForSeconds(0.1f);

      if (curHP > 0)
      {
         foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.white;
         
         reactVec = reactVec.normalized;
         reactVec += Vector3.up;
         rigid.AddForce(reactVec * 3,ForceMode.Impulse);
      }
      else
      {
         foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.gray;

         isDead = true;
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
