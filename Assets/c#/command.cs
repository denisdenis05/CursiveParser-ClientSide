using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class command : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    string sqlid=null;
    cam camerascript;
    // Start is called before the first frame update
    void Start()
    {
        camerascript = GameObject.Find("cam").GetComponent<cam>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void sendcommand()
    {
        string comanda = GameObject.Find("SendTextSSH").GetComponent<Text>().text;

        string username = GameObject.Find("cam").GetComponent<cam>().username;
        string userparola = GameObject.Find("cam").GetComponent<cam>().userparola;
        if (username == "necompletat" || userparola == "necompletat")
            StartCoroutine(EroareConectare());
        var values = new Dictionary<string, string>
            {
                { "upload", "upload" },
                { "username", username.ToString() },
                { "userpassword", userparola.ToString() }
            };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://"+ camerascript.serverip+"/api_server.php", content);
        string password = await response.Content.ReadAsStringAsync();
        print(password);
        string resultat;
            try
            {

            var promptRegex = new Regex(@"\][#$>]");
                var modes = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                SshClient ssh = new SshClient(camerascript.serverip, "root", password);
                ssh.Connect();
                var cmd = ssh.RunCommand("echo -e \'" + password + "\n\' | " + comanda + " 2>&1");
                resultat = cmd.Result.ToString();
                GameObject.Find("ConnectionResultText").GetComponent<Text>().text = resultat;
            }
            catch (Exception e)
            {
                StartCoroutine(EroareConectare());
            }
        

    }


    public async void deletecache()
    {
        string username = GameObject.Find("cam").GetComponent<cam>().username;
        string userparola = GameObject.Find("cam").GetComponent<cam>().userparola;
        if (username == "necompletat" || userparola == "necompletat")
            StartCoroutine(EroareConectare());
        var values = new Dictionary<string, string>
            {
                { "upload", "upload" },
                { "username", username.ToString() },
                { "userpassword", userparola.ToString() }
            };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://"+ camerascript.serverip + "/api_server.php", content);
        string password = await response.Content.ReadAsStringAsync();
        print(password);
        string resultat;
        try
        {

            var promptRegex = new Regex(@"\][#$>]");
            var modes = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            SshClient ssh = new SshClient(camerascript.serverip, "root", password);
            ssh.Connect();
            var cmd = ssh.RunCommand("echo -e \'" + password + "\n\' | rm -r "+ camerascript.locatiefolder + "cache/*");
            resultat = cmd.Result.ToString();
            GameObject.Find("ConnectionResultText").GetComponent<Text>().text = resultat;
        }
        catch (Exception e)
        {
            StartCoroutine(EroareConectare());
        }


    }


    IEnumerator EroareConectare()
    {
        GameObject error = GameObject.Find("eroareusername3");
        error.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(5);
        error.GetComponent<Text>().enabled = false;
    }

}
