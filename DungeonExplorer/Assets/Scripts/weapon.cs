using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

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

    public void useWeapon()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("swing");
            StartCoroutine("swing");
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
}
