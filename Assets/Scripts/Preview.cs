using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{

    public List<MeshRenderer> mats = new List<MeshRenderer>();
    public List<ParticleSystemRenderer> particles = new List<ParticleSystemRenderer>();
    public bool valid;
    public Color validColor;
    public Color invalidColor;

    // Update is called once per frame
    void Update()
    {
        if (valid) {
            foreach (MeshRenderer mat in mats) {
                foreach (Material m in mat.materials) {
                    m.SetColor("_EmissionColor", validColor);
                }
            }
            foreach (ParticleSystemRenderer p in particles) {
                foreach (Material m in p.materials) {
                    m.SetColor("_EmissionColor", validColor);
                }
            }
        } else {
            foreach (MeshRenderer mat in mats) {
                foreach (Material m in mat.materials) {
                    m.SetColor("_EmissionColor", invalidColor);
                }
            }
            foreach (ParticleSystemRenderer p in particles) {
                foreach (Material m in p.materials) {
                    m.SetColor("_EmissionColor", invalidColor);
                }
            }
        }
    }
}
