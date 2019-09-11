﻿    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 aimDirection;
    Rigidbody2D rigidbody2d;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        transform.position = transform.position + aimDirection * Time.deltaTime;

        aimDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        aimDirection.z = 0;
        aimDirection.Normalize();
    }

    void Update()
    {
        if (transform.position.magnitude > 75.0f)
        {
            Destroy(gameObject);
        }

        if (Weapons.hasShotgun)
        {
            StartCoroutine(KillTime(gameObject));
        }
    }

    IEnumerator KillTime(GameObject gameobject)
    {
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject);
    }
    public void Launch(float force)
    {
        if (Weapons.hasShotgun)
        {
            rigidbody2d.AddForce((aimDirection * .2f) * force);
        }
        rigidbody2d.AddForce(aimDirection * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        float dmgValue = 0;

        if (Weapons.hasLauncher)
        {
            dmgValue = 5;
        }
        if (Weapons.hasMagnum)
        {
            dmgValue = 5;
        }

        if (Weapons.hasPistol)
        {
            dmgValue = 1;
        }

        if (Weapons.hasShotgun)
        {
            dmgValue = 1.5f;
        }

        EnemyController e = other.collider.GetComponent<EnemyController>();

        if (e != null)
        {
            if (Perks.lifesteal)
            {
                DudeController.currentHealth += .1f * dmgValue;
                DudeController.currentHealth = Mathf.Clamp(DudeController.currentHealth, 0, DudeController.maxHealth);
                UIHealthbar.instance.SetValue(DudeController.currentHealth / (float)DudeController.maxHealth);
            }

            e.Damage(dmgValue);
        }

        Destroy(gameObject);
    }
}