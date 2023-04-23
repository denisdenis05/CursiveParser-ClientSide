using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cam : MonoBehaviour
{

    WebCamTexture CamTex;
    string filepath, picname;

    GameObject sendInput1;
    GameObject sendInput2;
    GameObject Checkboxes;
    GameObject imagineNeagra;

    public string username="necompletat", userparola="necompletat", serverip="necompletat";


    void Start()
    {
        sendInput1 = GameObject.Find("SSH-Com");
        sendInput2 = GameObject.Find("sendButon");
        Checkboxes = GameObject.Find("Checkboxes");
        imagineNeagra = GameObject.Find("IMAGINENEAGRA");

        GameObject.Find("back").GetComponent<Image>().enabled = false;
        Checkboxes.SetActive(false);
        imagineNeagra.SetActive(false);

        Color objectColor = imagineNeagra.GetComponent<Image>().color;
        objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, 0);
        imagineNeagra.GetComponent<Image>().color = objectColor;
        var webCamDevices = new WebCamTexture(WebCamTexture.devices[0].name, 1920, 1080, (int)Screen.currentResolution.refreshRate);
        gameObject.GetComponent<RawImage>().texture = webCamDevices;
        CamTex = webCamDevices;
        webCamDevices.Play();

        if (File.Exists(Application.persistentDataPath + "/user.ip"))
        {
            byte[] name = File.ReadAllBytes(Application.persistentDataPath + "/user.name");
            string numetemp = System.Text.Encoding.ASCII.GetString(name);
            byte[] pass = File.ReadAllBytes(Application.persistentDataPath + "/user.pass");
            string passtemp = System.Text.Encoding.ASCII.GetString(pass);
            byte[] ip = File.ReadAllBytes(Application.persistentDataPath + "/user.ip");
            string iptemp = System.Text.Encoding.ASCII.GetString(ip);
            Debug.Log(iptemp);
            if (numetemp != null && passtemp != null && iptemp != null)
            {
                username = numetemp;
                userparola = passtemp;
                serverip = iptemp;
                GameObject.Find("Canvas").GetComponent<adminpanel>().switchmodes(1);
            }
        }
        else
            GameObject.Find("Canvas").GetComponent<adminpanel>().switchmodes(0);
    }

    void Update()
    {

    }

    public void MenuOpen()
    {

        GameObject.Find("Menu-Open").GetComponent<Image>().enabled = false;
        GameObject.Find("Menu-Close").GetComponent<Image>().enabled = true;

    }

    public void MenuClose()
    {

        GameObject.Find("Menu-Open").GetComponent<Image>().enabled = true;
        GameObject.Find("Menu-Close").GetComponent<Image>().enabled = false;

    }

    public void LogInAsAdmin()
    {
        username = GameObject.Find("Admin-UsernameText").GetComponent<Text>().text;
        userparola= GameObject.Find("Admin-PasswordText").GetComponent<Text>().text;
        serverip = GameObject.Find("Admin-IPText").GetComponent<Text>().text;

        byte[] user = Encoding.ASCII.GetBytes(username);
        byte[] pass = Encoding.ASCII.GetBytes(userparola);
        byte[] ip = Encoding.ASCII.GetBytes(serverip);
        if (File.Exists(Application.persistentDataPath + "/user.name"))
        {
            File.Delete(Application.persistentDataPath + "/user.name");
            File.Delete(Application.persistentDataPath + "/user.pass");
            File.Delete(Application.persistentDataPath + "/user.ip");
        }
        using var writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/user.name"));
        writer.Write(user);
        using var writer2 = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/user.pass"));
        writer2.Write(pass);
        using var writer3 = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/user.ip"));
        writer3.Write(ip);
        GameObject.Find("Canvas").GetComponent<adminpanel>().switchmodes(1);
    }

    public void LogOutAsAdmin()
    {
        File.Delete(Application.persistentDataPath + "/user.name");
        File.Delete(Application.persistentDataPath + "/user.pass");
        File.Delete(Application.persistentDataPath + "/user.ip");
        GameObject.Find("Canvas").GetComponent<adminpanel>().switchmodes(0);
        username = "necompletat";
        userparola = "necompletat";
        serverip = "necompletat";
    }

    public void SendImage()
    {
        bool prelucrare, stocare;
        if (username == "necompletat" || userparola == "necompletat")
            StartCoroutine(EroareConectare());
        else
        {
            if(GameObject.Find("prelucrare").GetComponent<Toggle>().isOn == true || GameObject.Find("stocare").GetComponent<Toggle>().isOn == true)
            {
                prelucrare = GameObject.Find("prelucrare").GetComponent<Toggle>().isOn;
                stocare = GameObject.Find("stocare").GetComponent<Toggle>().isOn;
                GameObject.Find("Canvas").GetComponent<sftp>().MoveFiles(filepath, picname, prelucrare, stocare, username, userparola);
            }
            else
            {
                StartCoroutine(EroareCheckbox());
            }

        }

    }

    public void Back()
    {
        GameObject.Find("shutter").GetComponent<Image>().enabled = true;
        GameObject.Find("back").GetComponent<Image>().enabled = false;
        GameObject.Find("Menu-Open").GetComponent<Image>().enabled = true;
        GameObject.Find("eroareusername4").GetComponent<Text>().enabled = false;
        GameObject.Find("loading...").GetComponent<Text>().enabled = false;
        GameObject.Find("Imagew").GetComponent<Image>().enabled = false;
        GameObject.Find("pozatemp").GetComponent<RawImage>().enabled = false;

        gameObject.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);
        gameObject.GetComponent<RawImage>().texture = CamTex;

        Checkboxes.SetActive(false);

        File.Delete(filepath);
    }

    public void Poza() //pe telefon trebuie rotit 270 grade
    {
        Texture2D snap = new Texture2D(CamTex.width, CamTex.height);
        snap.SetPixels(CamTex.GetPixels());
        snap.Apply();
        SaveTexture(snap);
        StartCoroutine(FadeIn(imagineNeagra.GetComponent<Image>(), 20f));

        string name = "snapshot" + System.DateTime.Now.Year.ToString("00") + System.DateTime.Now.Month.ToString("00") + System.DateTime.Now.Day.ToString("00") + System.DateTime.Now.Hour.ToString("00") + System.DateTime.Now.Minute.ToString("00") + System.DateTime.Now.Second.ToString("00") + ".png";
        picname = name;
        filepath = Application.persistentDataPath + "/" + name;
        System.IO.File.WriteAllBytes(filepath, snap.EncodeToPNG());

        Checkboxes.SetActive(true);
        gameObject.GetComponent<RawImage>().texture = null;
        gameObject.GetComponent<RawImage>().color = new Color32(41, 34, 54, 100);

        GameObject.Find("pozamin").GetComponent<RawImage>().texture = snap;

        GameObject.Find("shutter").GetComponent<Image>().enabled = false;
        GameObject.Find("back").GetComponent<Image>().enabled = true;
        GameObject.Find("Menu-Open").GetComponent<Image>().enabled = false;

    }

    public void startCam()
    {
        CamTex.Play();
    }

    public void stopCam()
    {
        CamTex.Stop();
    }

    void SaveTexture(Texture2D snap)
    {
        GameObject.Find("Canvas").GetComponent<sftp>().snap = snap;

    }

    IEnumerator FadeIn(Image rend, float speed)
    {
        imagineNeagra.SetActive(true);

        while (rend.color.a < 1)
        {
            Color objectColor = rend.color;
            float fadeAmount = objectColor.a + (speed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            rend.color = objectColor;
            yield return null;
        }
        StartCoroutine(FadeOut(imagineNeagra.GetComponent<Image>(), 20f));
        yield return null;
    }

    IEnumerator EroareConectare()
    {
        GameObject error = GameObject.Find("eroareusername");
        error.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(5);
        error.GetComponent<Text>().enabled = false;
    }

    IEnumerator EroareCheckbox()
    {
        GameObject error = GameObject.Find("noCheckboxSelected");
        error.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(5);
        error.GetComponent<Text>().enabled = false;
    }

    IEnumerator FadeOut(Image rend, float speed)
    {
        while (rend.color.a > 0)
        {
            Color objectColor = rend.color;
            float fadeAmount = objectColor.a - (speed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            rend.color = objectColor;
            yield return null;
        }

        imagineNeagra.SetActive(false);
    }

    public void check(int i)
    {
        if (i == 0)
            GameObject.Find("prelucraremark").GetComponent<Image>().enabled = GameObject.Find("prelucrare").GetComponent<Toggle>().isOn;
        else
            GameObject.Find("stocaremark").GetComponent<Image>().enabled = GameObject.Find("stocare").GetComponent<Toggle>().isOn;

    }

}
