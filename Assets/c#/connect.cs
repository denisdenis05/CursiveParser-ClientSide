using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using Renci.SshNet;
using System.Threading;
using System.Text.RegularExpressions;


public class connect : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }




    private static void WriteStream(string cmd, ShellStream stream)
    {
        stream.WriteLine(cmd + "; echo this-is-the-end");
        while (stream.Length == 0)
            Thread.Sleep(500);
    }

    private static string ReadStream(ShellStream stream)
    {
        StringBuilder result = new StringBuilder();

        string line;
        while ((line = stream.ReadLine()) != "this-is-the-end")
            result.AppendLine(line);

        return result.ToString();
    }

    private static void SwithToRoot(string password, ShellStream stream)
    {
        // Get logged in and get user prompt
        string prompt = stream.Expect(new Regex(@"[$>]"));
        //Console.WriteLine(prompt);

        // Send command and expect password or user prompt
        stream.WriteLine("root");
        prompt = stream.Expect(new Regex(@"([$#>:])"));
        //Console.WriteLine(prompt);

        // Check to send password
        if (prompt.Contains(":"))
        {
            // Send password
            stream.WriteLine(password);
            prompt = stream.Expect(new Regex(@"[$#>]"));
            //Console.WriteLine(prompt);
        }
    }







    public void SSH()
    {
        string hostIp = "159.223.23.226";
        string hostName = "root";
        string pass = "O1!parolarandomOm";

        string command = GameObject.Find("SendTextSSH").GetComponent<Text>().text;
        string result = null;

        using (SshClient client = new SshClient(hostIp, 22, hostName, pass))
        {

                client.Connect();

                SshCommand sshCommand =  client.CreateCommand(command);
                result = sshCommand.Execute();
                GameObject.Find("ConnectionResultText").GetComponent<Text>().text = result;
                var reader = new StreamReader(sshCommand.ExtendedOutputStream);
                result = reader.ReadToEnd();
                GameObject.Find("ConnectionResultText").GetComponent<Text>().text = result;

                client.Disconnect();
            
        }
        GameObject.Find("ConnectionResultText").GetComponent<Text>().text = result;
    }

}


