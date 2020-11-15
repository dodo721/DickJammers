using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class CollisionSounds : MonoBehaviour
{

    public List<AudioClip> sounds = new List<AudioClip>();
    private AudioSource source;
    public float startCountdown;
    private float startTime;
    private bool makeNoise = false;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!makeNoise) {
            if (Time.time - startTime >= startCountdown) {
                makeNoise = true;
            }
        }
    }

    void OnCollisionEnter (Collision collision) {
        if (makeNoise) {
            AudioClip clip = sounds[Random.Range(0, sounds.Count)];
            source.PlayOneShot(clip);
            foreach (Enemy enemy in Enemy.allTheEnemies) {
                enemy.Distract(this);
            }
        }
    }
}
