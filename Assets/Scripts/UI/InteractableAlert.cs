using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractableAlert : MonoBehaviour
{
    public TextMeshProUGUI tMesh;
    private RectTransform rectTransform;
    private float timeLeft;
    private float newScale;
    private float shrinkRate;
    private bool ready;
    private Vector3 ogScale;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        tMesh.text = "";
        gameObject.SetActive(false);
        ogScale = transform.localScale;
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
                if (gameObject.transform.localScale.x < 0.1 * ogScale.x) {
                    gameObject.transform.localScale = ogScale;
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
