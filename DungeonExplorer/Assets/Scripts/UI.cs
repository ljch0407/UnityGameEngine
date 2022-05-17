using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Player player;
    public GameObject gamePanel;
    public TMP_Text health;
    public TMP_Text ammo;
    public TMP_Text coin;

    void LateUpdate()
    {
        health.text = player.health + "/" + player.maxHealth;
        coin.text = string.Format("{0:n0}", player.coins);
        if (player.equipWeapon == null) ammo.text = "-";
        else if (player.equipWeapon.type == weapon.Type.Melee) ammo.text = "-";
        else if (player.equipWeapon.type == weapon.Type.Range) ammo.text = player.equipWeapon.curAmmo + "/" + player.equipWeapon.maxAmmo;
    }
}
