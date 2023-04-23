using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


public class sftp : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    string source, dest, error;
    bool prelucrare, stocare;
    string username, userpassword, pass;
    GameObject Checkboxes;
    public Texture2D snap;
    bool loading = false, descarcat = false, eroare = false, startdownload = false, sprelucrare = false;
    Stream file1;
    string sqlid = null;
    cam camerascript;

    // Start is called before the first frame update
    void Start()
    {
        camerascript = GameObject.Find("cam").GetComponent<cam>();
        GameObject.Find("Imagew").GetComponent<Image>().enabled = false;
        GameObject.Find("pozatemp").GetComponent<RawImage>().enabled = false;
        Checkboxes = GameObject.Find("Checkboxes");
        GameObject.Find("loading...").GetComponent<Text>().enabled = false;
        GameObject.Find("eroareusername4").GetComponent<Text>().enabled = false;
    }




    // Update is called once per frame
    void Update()
    {
        if(loading ==true )
        {
            loading = false;
            GameObject.Find("loading...").GetComponent<Text>().enabled = false;

        }
        if (eroare == true)
        {
            eroare = false;
            GameObject er = GameObject.Find("eroareusername4");
            er.GetComponent<Text>().enabled = true;
            er.GetComponent<Text>().text = error;

        }
        if (startdownload == true)
        {
            startdownload = false;
            startrequests(); 
        }
        if(sprelucrare == true)
        {
            startprelucrare();
            sprelucrare = false;
        }

        /*if (descarcat == true)
        {
            descarcat = false;
            GameObject error = GameObject.Find("eroareusername5");
            error.GetComponent<Text>().enabled = true;
            error.GetComponent<Text>().text = "sadescarcat";
        }*/ //only for debugging
    }

    public async void MoveFiles(string sourcee, string destt, bool prelucraree, bool stocaree, string usernamee, string userpasswordd)
    {
        source = sourcee;
        dest = destt;
        prelucrare=prelucraree;
        stocare=stocaree;
        username=usernamee;
        userpassword = userpasswordd;
        Thread _thread = new Thread(Move);
        _thread.Start();
        GameObject.Find("loading...").GetComponent<Text>().enabled = true;
        Checkboxes.SetActive(false);
    }
    string passw;
    public async void Move()
    {
        

        print(dest);
        string destination = "/var/www/html/Poze/" + dest;
        var values = new Dictionary<string, string>
            {
                { "upload", "upload" },
                { "numefisier", destination },
                { "prelucrare", prelucrare.ToString() },
                { "stocare", stocare.ToString() },
                { "username", username.ToString() },
                { "userpassword", userpassword.ToString() }
            };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://"+ camerascript.serverip + "/api_server.php", content);
        string password = await response.Content.ReadAsStringAsync();
        print(password);
        passw = password;
        if (password.Contains("parola incorecta"))
        {
            GameObject error = GameObject.Find("eroareusername4");
            error.GetComponent<Text>().enabled = true;
            StartCoroutine(EroareConectare2());
        }
        else
        {
            try
            {
                using (SftpClient sftp = new SftpClient(camerascript.serverip, 22, "root", password))
                {
                    sftp.Connect();
                    sftp.ChangeDirectory("/");
                    Stream localFile = File.OpenRead(source);
                    sftp.UploadFile(localFile, destination, true);
                    localFile.Close();
                    File.Delete(source);


                    Debug.Log(destination);
                    
                    Debug.Log("da??");
                    loading = true;
                    pass = password;
                    Debug.Log("da??");
                    //Thread _thread = new Thread(download);
                    //_thread.Start();
                    //GameObject.Find("cam").GetComponent<cam>().Back();
                    startdownload=true;
                    //nu merge da trebuie verificat
                }
            }
            catch (Exception e)
            {
                error = e.ToString();
                eroare = true;
                StartCoroutine(EroareConectare3(e));
            }
        }
    }

    async void startrequests()
    {
        string username = GameObject.Find("cam").GetComponent<cam>().username;
        string userparola = GameObject.Find("cam").GetComponent<cam>().userparola;
        if (username == "necompletat" || userparola == "necompletat")
            StartCoroutine(EroareConectare());
        var values = new Dictionary<string, string>
            {
                { "upload", "req" },
                { "username", username.ToString() },
                { "userpassword", userparola.ToString() }
            };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://"+ camerascript.serverip + "/check_if_running.php", content);
        sqlid = await response.Content.ReadAsStringAsync();
        print(sqlid);
        response.Dispose();
        StopAllCoroutines();
        StartCoroutine(CheckRequest(username, userparola));

    }


    string res = "csv";
    int nr = 0;
    IEnumerator CheckRequest(string username, string userparola)
    {
        bool ok = false;

        while (ok == false && nr < 500)
        {
            Task task = SendCheck(username, userparola);
            yield return new WaitUntil(() => task.IsCompleted);


            if (res == "liber")
                ok = true;
            else
            {
                yield return new WaitForSeconds(0.1f);
                nr += 1;
                yield return null;
            }
            print(res);
        }

        req();
        yield return null;
    }


    string fullpath;
    public async void req()
    {
        print("test");
        string destination = "/var/www/NeuralNetwork/cache/" + dest;
        print("test");
        var promptRegex = new Regex(@"\][#$>]");
        print("test");
        var modes = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
        print("test");
        //check if running
        try
        {
            print(passw);
            SshClient ssh = new SshClient(camerascript.serverip, "root", passw.ToString());
            print("test");
            ssh.Connect();
            print("test");
            //var cmd = ssh.CreateCommand("echo -e \'" + passw + "\n\' | java -jar /var/www/html/ProcesareImagine7.jar \"" + stocare.ToString() + "\" \"" + prelucrare.ToString() + "\" \"" + destination + "\" \"121\" \"v\" 2>&1");
            var cmd = ssh.CreateCommand("echo -e \'" + passw + "\n\' | java -jar /var/www/NeuralNetwork/MainJar.jar " + "\"" + stocare.ToString() + "\" \"" + destination + "\" \"v\"");
            var result = cmd.Execute();

            var reader = new StreamReader(cmd.ExtendedOutputStream);
            Console.WriteLine("DEBUG:");
            Console.Write(reader.ReadToEnd());

            Debug.Log(result);
            ssh.Disconnect();
            ssh.Dispose();
        }
        catch (Exception e)
        {
            print(e.ToString());
        }
        fullpath = Application.persistentDataPath + "/pozitii temporare.txt";
        Thread _thread = new Thread(download);
        _thread.Start();
    }
    async Task SendCheck(string username, string userparola)
    {
        var values = new Dictionary<string, string>
            {
                { "upload", "check" },  
                { "username", username.ToString() },
                { "userpassword", userparola.ToString() },
                { "id", sqlid.ToString() }
            };
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://"+ camerascript.serverip + "/check_if_running.php", content);
        var raspuns = await response.Content.ReadAsStringAsync();
        res = raspuns;
        response.Dispose();
        print("yay");
    }

    public void download() // neoptimizat pentru telefon, da crash
    {
        try
        {
            using (SftpClient sftp = new SftpClient(camerascript.serverip, 22, "root", pass))
            {
                file1 = File.Create(fullpath);
                sftp.Connect();
                sftp.ChangeDirectory("/");
                sftp.DownloadFile("/var/www/html/cache/pozitii temporare.txt", file1);
                file1.Close();
                Debug.Log("da??");

            }
            //descarcat = true; //for debigging only
            sprelucrare = true;
        }
        catch (Exception e)
        {
            error = e.ToString();
            eroare = true;
        } //trebuie refacut
    }
    
    void startprelucrare()
    {

        string[] lines = System.IO.File.ReadAllLines(fullpath);
        int nr = -1, inc = -1, sf = -1;
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                if (nr == -1)
                    nr = Int32.Parse(line);
                else
                {
                    if (inc == -1)
                        inc = Int32.Parse(line);
                    else if (sf == -1)
                    {
                        sf = Int32.Parse(line);
                        if (inc == sf)
                        {
                            for (int j = 1; j <= snap.width; j++)
                                snap.SetPixel(inc, j, Color.red);
                            snap.Apply();
                        }
                        else
                        {
                            for (int i = inc; i <= sf; i++)
                            {
                                if (i == inc || i == sf)
                                    for (int j = 1; j <= snap.width; j++)
                                        snap.SetPixel(i, j, Color.red);
                                else
                                    for (int j = i % 5; j <= snap.width; j = j + 5)
                                        snap.SetPixel(i, j, Color.red);
                            }
                            snap.Apply();
                        }
                        inc = -1;
                        sf = -1;
                    }
                }
            }
        }


        GameObject.Find("Imagew").GetComponent<Image>().enabled = true;
        GameObject.Find("pozatemp").GetComponent<RawImage>().enabled = true;
        GameObject.Find("pozatemp").GetComponent<RawImage>().texture = snap;



        Debug.Log("wooo??");
    }

    public async void resetrequests()
    {
        string usr=GameObject.Find("cam").GetComponent<cam>().username;
        string pas=GameObject.Find("cam").GetComponent<cam>().userparola;
        var values = new Dictionary<string, string>
            {
                { "upload", "forcereset" },
                { "username", usr.ToString() },
                { "userpassword", pas.ToString() }
            };
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://"+ camerascript.serverip + "/check_if_running.php", content);
        var raspuns = await response.Content.ReadAsStringAsync();
        res = raspuns;
        response.Dispose();
    }

    IEnumerator EroareConectare()
    {
        GameObject error = GameObject.Find("eroareusername2");
        error.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(5);
        error.GetComponent<Text>().enabled = false;
    }

    IEnumerator EroareConectare2()
    {
        GameObject error = GameObject.Find("eroareusername4");
        error.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(5);
        error.GetComponent<Text>().enabled = false;
    }
    IEnumerator EroareConectare3(Exception e)
    {
        GameObject error = GameObject.Find("eroareusername5");
        error.GetComponent<Text>().enabled = true;
        error.GetComponent<Text>().text = e.ToString();
        yield return new WaitForSeconds(5);
        error.GetComponent<Text>().enabled = false;
    }

}
