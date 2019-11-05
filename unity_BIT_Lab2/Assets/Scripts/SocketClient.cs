using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class SocketClient
{
    private Socket socketClient;
    private Thread thread;
    private byte[] data = new byte[1024];
    public bool isTrigger;
    public Quaternion quaternion = Quaternion.Euler(0,0,0);
    public float GX, GY, GZ;
    public float w, x, y, z;

    public SocketClient(string hostIP, int port) {
        thread = new Thread(() => {
            // while the status is "Disconnect", this loop will keep trying to connect.
            while (true) {
                try {
                    socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketClient.Connect(new IPEndPoint(IPAddress.Parse(hostIP), port));
                    // while the connection
                    while (true) {
                        /*********************************************************
                         * TODO: you need to modify receive function by yourself *
                         *********************************************************/
                        if (socketClient.Available < 100) {
                            //Thread.Sleep(1);
                            continue;
                        }
                        int length = socketClient.Receive(data);
                        string message = Encoding.UTF8.GetString(data, 0, length);
                        Debug.Log("mes  = " + message);
                        string[] messages = message.Split('\t');
                        for (int i = 0; i < messages.Length - 1; i++) {
                            //Debug.Log("str = " + messages[i]);
                            string[] datas = messages[i].Split(' ');

                            w = Convert.ToSingle(datas[1]);
                            x = Convert.ToSingle(datas[2]);
                            y = Convert.ToSingle(datas[3]);
                            z = Convert.ToSingle(datas[4]);
                            //Debug.Log("quaternion = " + x + " " + y + " " + z + " " + w);
                            isTrigger = datas[0] == "1" ? true : false;
                        }
                    }
                } catch (Exception ex) {
                    if (socketClient != null) {
                        socketClient.Close();
                    }
                    Debug.Log(ex.Message);
                }
            }
        });
        thread.IsBackground = true;
        thread.Start();
    }

    public void Close() {
        thread.Abort();
        if (socketClient != null) {
            socketClient.Close();
        }
    }
}
