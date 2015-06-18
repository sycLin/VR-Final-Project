using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Parse;

public class NetworkManager : MonoBehaviour {
	
	private TcpListener tcpListener;
	private Thread listenThread;
	static private int serverPort = 5566;

	//player
	public GameObject targetObject;
	public ArrayList commandList;
	public PlayerController playerController;
	public BulletShooter shooter;

	public bool UsingProxy;
	public string ProxyURL;
	public int ProxyPort;
	public TcpClient proxySocket;

	public bool blinkable = false;

	//public CNj
	//public BotControlScript botControllScript;

	// Use this for initialization
	void Start () {
		//init
		commandList = new ArrayList ();

		if (UsingProxy) 
		{
			proxySocket = new TcpClient ();
			proxySocket.BeginConnect (ProxyURL, ProxyPort, new System.AsyncCallback (ProxySuccess),proxySocket);
		}
		else
		{
			this.tcpListener = new TcpListener(IPAddress.Any, serverPort);
			this.listenThread = new Thread(new ThreadStart(ListenForClients));
			this.listenThread.Start();
		}

		string localIP = LocalIPAddress ();

		Debug.Log ("Server Start on:"+localIP);


		ParseObject testObject = new ParseObject("GlassGame");
		testObject["ip"] = localIP;
		testObject.SaveAsync().ContinueWith(temp=>
		                                    {

			var query = ParseObject.GetQuery("GlassGame").OrderByDescending("createdAt").Limit(1);
			query.FirstAsync().ContinueWith(t =>
			                                {
				ParseObject obj = t.Result;

				Debug.Log("Insert Parse ip:"+obj["ip"]);
				Debug.Log("Parse Date:"+obj.CreatedAt);
			});

		});

	}

	void ProxySuccess(System.IAsyncResult result)
	{
		if (result.IsCompleted) 
		{
			Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
			clientThread.Start(proxySocket);

			proxyWriter = new StreamWriter(proxySocket.GetStream());
			proxyWriter.WriteLine("{\"command\":\"CreateProxyServer\",\"id\":\"GlassGameServer\"}");
			proxyWriter.Flush();
			Debug.Log ("Proxy Connect");
		}
	}
	StreamWriter proxyWriter;
	private void ListenForClients()
	{
		this.tcpListener.Start();
		
		while (true)
		{
			Debug.Log ("Waiting for Client");

			//blocks until a client has connected to the server
			TcpClient client = this.tcpListener.AcceptTcpClient();
			
			//create a thread to handle communication 
			//with connected client
			Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
			clientThread.Start(client);
			Debug.Log ("Client Connect");
		}
	}

	private void HandleClientComm(object client)
	{
		TcpClient tcpClient = (TcpClient)client;
		StreamReader clientStream = new StreamReader(tcpClient.GetStream());

		while (true)
		{
			string commandStr = clientStream.ReadLine();

			commandStr = commandStr.Replace("'","\"");
			//Debug.Log("reading:"+commandStr.Length +" data:"+commandStr);
			
			JSONObject commandJsonObject = new JSONObject(commandStr);
			commandList.Add(commandJsonObject);


		}
	}

	char[] splitArray = "\t".ToCharArray();

	void parseCommand(){
		//check
		if (commandList.Count == 0) {
			return;
		}

		ArrayList tempCommandList = (ArrayList)commandList.Clone();
		commandList.RemoveRange (0, tempCommandList.Count);

		foreach (JSONObject commandObject in tempCommandList) {
			//Debug.Log("Command Object:"+commandObject.ToString());
			//check
			if(commandObject == null) continue;
			else if(commandObject["command"] == null) continue;

			//get command
			string commandStr = commandObject["command"].str;

			//check
			if(commandStr == null) {
				continue;
			}

			//Debug.Log(commandObject);

			switch (commandStr){
				case "move":
					//check
					if(commandObject!=null){

						float x = 0.0f;
						if((commandObject["x"]!=null) && (commandObject["x"].n != null)){
							x = (float)commandObject["x"].n;
						}
							
						float y = 0.0f;
						if((commandObject["y"]!=null) && (commandObject["y"].n != null)){
							y = (float)commandObject["y"].n;
						}


					playerController.joyStickInput = new Vector2(x,-y);

						//targetObject.transform.position -= targetObject.transform.forward * y * playerController.moveSpeed * Time.deltaTime;
						//targetObject.transform.position += targetObject.transform.right * x * playerController.moveSpeed * Time.deltaTime;
						//}
					}
					break;
				case "singleTap":
				case "acceleration":
				case "microphone":

					
					playerController.shoot();
					//Debug.Log("Single Tap!");
					break;

				case "blink":
				if(blinkable)
				{
					playerController.shoot();
				}
				//Debug.Log("Single Tap!");
				break;

				case "changeBlink":
				if(commandObject["state"]!=null)
				{
					blinkable = commandObject["state"].b;
				}

				//Debug.Log("blinkable:"+blinkable);
				break;

			case "doubleTap":
					//Debug.Log("Double Tap!");

					
					//Application.Quit();

					break;
				case"changeWeapon":

					string direction = commandObject["direction"].str;
					
				if(direction == "previous")
				{
					playerController.weaponManager.ChangeWeaponToPrevous();
				}
				else
				{
					playerController.weaponManager.ChangeWeaponToNext();
				}

				break;

				case "swipe":
					Debug.Log("Swipe!");
					break;
				case "longPress":
					Debug.Log("Long Press!");
					break;			
				case "gyro":

				Quaternion q;
				q.x = (float)commandObject["x"].n;
				q.y = (float)commandObject["y"].n;
				q.z = (float)commandObject["z"].n;
				q.w = (float)commandObject["w"].n;

				//Debug.Log("gyro"+q);

				shooter.setGunGyro(new Quaternion(q.x,q.y,-q.z,-q.w));
//				Debug.Log("Accelerometer: x = "+commandObject["accX"].n+" y = "+commandObject["accY"].n+" z = "+commandObject["accZ"].n);

				break;
				case "accelerometer":
					//Debug.Log("Accelerometer: x = "+commandObject["x"]+" y = "+commandObject["y"]+" z = "+commandObject["z"]);
					break;
				case "changeAimMode":

			

				string mode = commandObject["mode"].str;
				Debug.Log("changeAimMode:"+mode);
				if(mode.Equals("phoneGun"))
				{
					shooter.aimType = BulletShooter.AimType.phoneGun;
					playerController.ClearCurrentOffestX();
				}
				else if(mode.Equals("viewportCenter"))
				{
					shooter.aimType = BulletShooter.AimType.viewportCenter;
					playerController.ClearCurrentOffestX();
				}

				
			
				break;
			case "rotate":
				if(commandObject!=null){
					
					float x = 0.0f;
					if((commandObject["x"]!=null) && (commandObject["x"].n != null)){
						x = (float)commandObject["x"].n;
					}
					
					float y = 0.0f;
					if((commandObject["y"]!=null) && (commandObject["y"].n != null)){
						y = (float)commandObject["y"].n;
					}
					
					
					playerController.joyStickInputRight = new Vector2(x,-y);



					//targetObject.transform.position -= targetObject.transform.forward * y * playerController.moveSpeed * Time.deltaTime;
					//targetObject.transform.position += targetObject.transform.right * x * playerController.moveSpeed * Time.deltaTime;
					//}
				}
				break;
			case "clearOffsetX":

				playerController.ClearCurrentOffestX();

				break;

			case "headGesture":

				bool enable = commandObject["value"].b;
				tilt.enabled = enable;
				
				break;

			case "onTrack":
				
				bool ontrack = commandObject["value"].b;
				playerController.UserControl = !ontrack;
				
				break;

			case "exit":

				Debug.Log("quit");
				Application.Quit();

				
				break;

				default:
					Debug.Log("No such command:"+commandStr);
					break;
			}
		}

		//clear commands


		//commandList.Clear();

	}
	public TiltListener tilt;

	private bool isInitQuaternion = false;
	public Quaternion initQuaternion;

	// Update is called once per frame
	void Update () {
		if (targetObject != null) {
			//parse command 
			parseCommand();
		}
	}

	//get ip address
	public string LocalIPAddress()
	{
		/*
		string strHostName = "";
		strHostName = System.Net.Dns.GetHostName();
		
		IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
		
		IPAddress[] addr = ipEntry.AddressList;

		Debug.Log ("ip:"+Network.player.ipAddress);
		return addr[addr.Length-1].ToString(); 
		*/

		return Network.player.ipAddress;
	}
}
