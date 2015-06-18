using UnityEngine;
using System.Collections;
using Parse;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public string LastGameIP;
	public int port = 5566;

	public bool isConnected = false;
	TcpClient client;
	StreamWriter streamWriter;

	public bool usingProxy;
	public string proxyHost;
	public int proxyPort;

	private TcpListener tcpListener;
	private Thread listenThread;
	public int listenPort = 5567;

	public delegate void NetworkCommandEventHandler(JSONObject command);
	public NetworkCommandEventHandler networkCommandEvent;


	// Use this for initialization
	void Start () {
		
		var query = ParseObject.GetQuery("GlassGame").OrderByDescending("createdAt").Limit(1);
		query.FirstAsync().ContinueWith(t =>
		                                {
			ParseObject obj = t.Result;
			
			Debug.Log("Insert Parse ip:"+obj["ip"]);
			Debug.Log("Parse Date:"+obj.CreatedAt);

			LastGameIP = obj["ip"].ToString();
			ConnectTo(LastGameIP);
		});


		this.tcpListener = new TcpListener(IPAddress.Any, listenPort);
		this.listenThread = new Thread(new ThreadStart(ListenForClients));
		this.listenThread.Start();
	}


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
			commandStr = commandStr.Split(splitArray)[1];
			JSONObject obj = new JSONObject(commandStr.Trim());
			commands.Add(obj);
			//Debug.Log ("commandStr:"+commandStr);
		}
	}

	private List<JSONObject> commands = new List<JSONObject>();
	char[] splitArray = "\t".ToCharArray();

	void Update ()
	{
		JSONObject[] copyCommand = commands.ToArray ();
		commands.Clear ();

		foreach (JSONObject obj in copyCommand) 
		{
			if(networkCommandEvent!=null)
			{
				networkCommandEvent(obj);
			}
		}
	}

	void ConnectTo(string ip)
	{
		if (usingProxy) 
		{
			Debug.Log ("Connecting to " + ip);

			client = new TcpClient ();
			client.BeginConnect (proxyHost, proxyPort, new System.AsyncCallback (ProcessDnsInformation), client);
		}
		else
		{
			Debug.Log ("Connecting to " + ip);
			
			client = new TcpClient ();
			client.BeginConnect (ip, port, new System.AsyncCallback (ProcessDnsInformation), client);
			
		}

	}

	/*Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
	clientThread.Start(client);*/
	void ProcessDnsInformation(System.IAsyncResult result)
	{
		if (result.IsCompleted) 
		{
			Debug.Log("[Connect Success] : "+LastGameIP);
			isConnected = true;
			streamWriter = new StreamWriter(client.GetStream());

			if(usingProxy)
			{
				Send("{\"command\":\"ProxyToTarget\",\"targetID\":\"GlassGameServer\",\"id\":\"GlassGameClient\"}");
			}


			Thread clientThread = new Thread(new ParameterizedThreadStart(ReadData));
			clientThread.Start(client);
		}
	}

	public void ReadData(object client)
	{
		StreamReader reader = new StreamReader(((TcpClient)client).GetStream());

		while(true)
		{

			//Debug.Log("Try reading");

			string message = reader.ReadLine();

			if (message.Length > 0)
				Debug.Log (message);

		}
	}

	public void Send(string str)
	{
		if (isConnected) 
		{
			streamWriter.WriteLine(str);
			streamWriter.Flush();
		}
	}
}
