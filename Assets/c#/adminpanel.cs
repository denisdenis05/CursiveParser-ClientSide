using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adminpanel : MonoBehaviour
{
    GameObject Panel;
    GameObject Login;

    // Start is called before the first frame update
    void Start()
    {
        Panel = GameObject.Find("ADMIN COMMANDS");
        Login = GameObject.Find("ADMIN LOGIN");
    }

    public void switchmodes(int i)
    {
        //0 inseamna not logged in, 1 inseamna logged in
        if (i == 0)
        {
            Panel.SetActive(false);
            Login.SetActive(true);
        }
        else
        {
            Panel.SetActive(true);
            Login.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
