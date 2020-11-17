using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractableAlert : MonoBehaviour
{
    public GameObject countdownText;
    private TextMeshPro tMesh;
    private float timeLeft;
    private float newScale;
    private float shrinkRate;
    private bool ready;

    void Start()
    {
        tMesh = countdownText.GetComponent<TextMeshPro>();
        tMesh.text = "";
        gameObject.SetActive(false);
    }

    public void Alert(string text, float time)
    {
        Debug.Log("ALERTING " + text + " FOR " + time + " SECONDS");
        tMesh.text = text;
        timeLeft = time;
        newScale = 1f;
        shrinkRate = 0.2f;
        ready = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (ready)
        {
            if (timeLeft <= 0)
            {
                newScale -= shrinkRate * Time.deltaTime;
                float tScale = newScale * gameObject.transform.localScale.x;
                gameObject.transform.localScale = new Vector3(tScale, tScale, 1);
                if (gameObject.transform.localScale.x < 0.1) {
                    gameObject.transform.localScale = new Vector3(1f,1f,1f);
                    ready = false;
                    tMesh.text = "";
                    gameObject.SetActive(false);
                }
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
