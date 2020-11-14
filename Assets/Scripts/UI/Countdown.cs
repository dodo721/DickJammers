using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
    public GameObject countdownText;
    public float timerVal;
    private TextMeshPro tMesh;
    private float newScale;
    private float shrinkRate;
    private bool ready;

    void Start()
    {
        tMesh = countdownText.GetComponent<TextMeshPro>();
    }

    public void passParam(float timerVal)
    {
        this.timerVal = timerVal;
        newScale = 1f;
        shrinkRate = 0.2f;
        ready = true;
    }

    void Update()
    {
        if (ready)
        {
            if (timerVal <= 0)
            {
                newScale -= shrinkRate * Time.deltaTime;
                float tScale = newScale * gameObject.transform.localScale.x;
                gameObject.transform.localScale = new Vector3(tScale, tScale, 1);
                if (gameObject.transform.localScale.x < 0.1) GameObject.Destroy(gameObject);
            }
            else
            {
                timerVal -= Time.deltaTime;
                tMesh.text = (Mathf.Round(timerVal * 10f) / 10f).ToString();
            }
        }
    }
}
