using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BtnChooseServer : ButttonAbstract
{
    public ServerInfo serverInfo;
    [SerializeField] protected int timeout = 3;
    [SerializeField] protected float checkInterval = 5f;
    [SerializeField] protected string statusText = "-";

    protected override void Start()
    {
        base.Start();
        this.serverInfo = ServerManager.Instance.GetServer(this.serverInfo.ServerName);

        this.UpdateUI();
        //TODO: this is not working yet
        //InvokeRepeating(nameof(this.CheckServerStatus), 1, 3f);
    }

    public override void OnClick()
    {
        Debug.Log("Choose server: " + this.serverInfo.ServerName);
        ServerManager.Instance.ConnectToServer(this.serverInfo.ServerName);
    }

    public void CheckServerStatus()
    {
        Thread thread = new Thread(() =>
        {
            bool isOnline = IsServerOnline(this.serverInfo.IPAddress, this.serverInfo.Port);
            Debug.Log(isOnline ? "Server is ONLINE" : "Server is OFFLINE");

        });
        thread.Start();
    }

    private bool IsServerOnline(string ip, int port)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect(ip, port);
                return true;  // Server is running
            }
        }
        catch
        {
            return false;  // Server is not running
        }
    }

    private IEnumerator UpdateServerStatus(bool isOnline)
    {
        yield return null; 
        Debug.Log(isOnline ? "Server is ONLINE" : "Server is OFFLINE");
    }

    private void UpdateUI()
    {
        if (statusText != null && this.serverInfo != null)
        {
            this.buttonText.text = this.serverInfo.ServerName + ": " + this.statusText;
        }
    }
}
