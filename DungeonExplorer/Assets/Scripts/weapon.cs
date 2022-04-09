using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using Random = System.Random;

public class weapon : MonoBehaviour
{
    public enum Type
    {
        Melee,
        Range
    };

    public Type type;
    public int damage;
    public float attackRate;
    public BoxCollider meleeArea;
    public TrailRenderer meleeEffect;

    public Transform bulletPos;
    public GameObject bullet;

    public Transform bulletCasePos;
    public GameObject bulletCase;

    public int maxAmmo;
    public int curAmmo;
    
    public void useWeapon()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("swing");
            StartCoroutine("swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("shot");
        }
    }

    IEnumerator swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        meleeEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        meleeEffect.enabled = false;
    }
    
    IEnumerator shot()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position,bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return null;
        
        GameObject instantBulletCase = Instantiate(bulletCase, bulletCasePos.position,bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = instantBulletCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * -1;
        bulletCaseRigid.AddForce(caseVec,ForceMode.Impulse);
        bulletRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
