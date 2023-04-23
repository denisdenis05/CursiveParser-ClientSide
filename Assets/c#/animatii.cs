using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class animatii : MonoBehaviour
{

    public float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    //multe if-uri jos pt cazuri speciale
    public void movetospot(int i)
    {
        GameObject obiect, destination;
        if (i == 0)
        {
            obiect = GameObject.Find("Panel");
            destination = GameObject.Find("posmijloc");
        }
        else
        {
            obiect = GameObject.Find("Panel");
            destination = GameObject.Find("posstanga");

        }
        StartCoroutine(move(obiect, destination));

    }



    IEnumerator move(GameObject obiect, GameObject destination)
    {
        Vector3 Gotoposition = destination.transform.position;
        float elapsedTime = 0;
        float waitTime = 0.2f - speed;
        Vector3 currentPos = obiect.transform.position;
        while (elapsedTime < waitTime)
        {
            obiect.transform.position = Vector3.Lerp(currentPos, Gotoposition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obiect.transform.position = Gotoposition;
        yield return null;
    }

}