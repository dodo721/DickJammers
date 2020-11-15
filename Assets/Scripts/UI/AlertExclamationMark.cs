using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AlertExclamationMark : MonoBehaviour
{
    [SerializeField, Range(0,1)]
    public float adjust;
    public GameObject sprite;
    SpriteRenderer spriteRend;
    public float rotateAngle;
    public float rotSpeed;

    void Start()
    {
        spriteRend = sprite.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (adjust != 0) {
            spriteRend.color = new Color(Mathf.Round(adjust * 256), spriteRend.color.g, spriteRend.color.b, adjust);
            float maxAngle = rotateAngle * adjust;
            float lastAngle = transform.rotation.eulerAngles.z;

            float a = Mathf.PingPong(rotSpeed * adjust * Time.time, maxAngle * 2) - maxAngle;

            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, a));
        } else {
            spriteRend.color = new Color(1f, spriteRend.color.g, spriteRend.color.b,0);
        }
        
    }
}
