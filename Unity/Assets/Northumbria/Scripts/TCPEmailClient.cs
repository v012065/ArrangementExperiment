using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;

public class TCPEmailClient : MonoBehaviour {  	
	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;
    #endregion

    public string ip = "localhost";
    public int port = 8052;

    public GameObject ui;
    public GameObject debugUi;
    //public GameObject aryzon;

    public InputField ipField;
    public InputField portField;

    //public GameObject ARSession;
    public EmailTest emailTest;

    bool aryzonReceived = false;
    bool startReceived = false;
    bool planesReceived = false;
    bool qrReceived = false;
    bool debugReceived = false;
    bool zoomReceived = false;
    bool panicReceived = false;

    StreamWriter writer;
    StreamReader reader;
    string netFile = "network.txt";
    string path = "";

    // Use this for initialization 	
    void Start () {
        //ConnectToTcpServer();     

        path = Application.persistentDataPath + "/data/";

        Debug.Log(path);

        if (!System.IO.File.Exists(path + "/" + netFile))
        {
            Directory.CreateDirectory(path);
            //writer = new StreamWriter(path + "/" + netFile);

            //writer.WriteLine(ip + " " + port);
            //writer.Flush();
        }
        else
        {
            //reader = new StreamReader(path + "/" + netFile, true);

            //if (reader != null)
            //{
            //    var dataLine = reader.ReadLine();

            //    if (dataLine != null)
            //    {
            //        var dataSplit = dataLine.Split(' ');
            //        ip = dataSplit[0];
            //        ipField.text = ip;
            //        port = int.Parse(dataSplit[1]);
            //        portField.text = dataSplit[1];
            //    }

            //    reader.Close();
            //}

            //writer = new StreamWriter(path + "/" + netFile);
        }
    }  	
	// Update is called once per frame
	void Update ()
    {         
		//if (Input.GetKeyDown(KeyCode.Space))
  //      {             
		//	SendMessage();         
		//}
  //      else if(Input.GetMouseButtonDown(0))
  //      {
  //          SendMessage();
  //      }

        if(aryzonReceived)
        {
            emailTest.ToggleAryzon();

            aryzonReceived = false;
        }

        if(startReceived)
        {
            emailTest.StartTestSetup();

            startReceived = false;
        }

        if(planesReceived)
        {
            emailTest.TogglePlanes();

            planesReceived = false;
        }

        if(qrReceived)
        {
            emailTest.ToggleQR();

            qrReceived = false;
        }

        if(debugReceived)
        {
            debugUi.SetActive(!debugUi.activeSelf);
            ui.SetActive(!ui.activeSelf);

            debugReceived = false;
        }

        if(zoomReceived)
        {
            emailTest.ToggleZoom();

            zoomReceived = false;
        }

        if (panicReceived)
        {
            emailTest.SoftReset();

            panicReceived = false;
        }
	}  	
	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}  	
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData() { 		
		try { 			
			socketConnection = new TcpClient(ip, port);  			
			Byte[] bytes = new Byte[1024];             
			while (true) { 				
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) { 					
					int length; 					
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 						
						var incommingData = new byte[length]; 						
						Array.Copy(bytes, 0, incommingData, 0, length); 						
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData); 						
						Debug.Log("server message received as: " + serverMessage); 	
                        
                        if(serverMessage == "Aryzon")
                        {
                            aryzonReceived = true;
                        }
                        else if(serverMessage == "Start")
                        {
                            startReceived = true;
                        }
                        else if(serverMessage == "Planes")
                        {
                            planesReceived = true;
                        }
                        else if(serverMessage == "QR")
                        {
                            qrReceived = true;
                        }
                        else if (serverMessage == "Debug")
                        {
                            debugReceived = true;
                        }
                        else if (serverMessage == "Zoom")
                        {
                            zoomReceived = true;
                        }
                        else if(serverMessage == "Panic")
                        {
                            panicReceived = true;
                        }
                    } 				
				} 			
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}  	
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage() {         
		if (socketConnection == null) {             
			return;         
		}  		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				string clientMessage = "This is a message from one of your clients."; 				
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");
                stream.Flush();
            }         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}

    public void SendMessage(string msg)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = msg;
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
                stream.Flush();
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SetIP(string _ip)
    {
        ip = _ip;

        if(writer != null) writer.WriteLine(ip + " " + port);
    }

    public void SetPort(string _port)
    {
        SetPort(int.Parse(_port));
    }

    public void SetPort(int _port)
    {
        port = _port;

        if (writer != null) writer.WriteLine(ip + " " + port);
    }

    public void StartClient()
    {
        ConnectToTcpServer();
        SendMessage("Test Email Client");
    }

    public void SaveNetworkSettings()
    {
        writer = new StreamWriter(path + "/" + netFile);

        writer.WriteLine(ip + " " + port);
        writer.Flush();
        writer.Close();
        writer.Dispose();
    }

    public void LoadNetworkSettings()
    {
        reader = new StreamReader(path + "/" + netFile, true);

        if (reader != null)
        {
            var dataLine = reader.ReadLine();

            if (dataLine != null)
            {
                var dataSplit = dataLine.Split(' ');
                ip = dataSplit[0];
                ipField.text = ip;
                port = int.Parse(dataSplit[1]);
                portField.text = dataSplit[1];
            }

            reader.Close();
            reader.Dispose();
        }
    }
}