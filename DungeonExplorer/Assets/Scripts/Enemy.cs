using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   public int curHP;
   public int maxHP;

   private Rigidbody rigid;
   private BoxCollider boxCollider;
   private Material material;
   

   private void Awake()
   {
      rigid = GetComponent<Rigidbody>();
      boxCollider = GetComponent<BoxCollider>();
      material = GetComponent<MeshRenderer>().material;

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
         
         reactVec = reactVec.normalized;
         reactVec += Vector3.up;
         rigid.AddForce(reactVec * 5,ForceMode.Impulse);
         
         Destroy(gameObject,4f);
      }
   }
}
