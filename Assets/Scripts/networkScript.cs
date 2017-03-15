using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class networkScript : MonoBehaviour {
    public gameScript myGame;
    public string ipaddress;

    private int connectionId;
    private int channelId;
    private int socketId;
    private int socketPort = 7777;
    private string[] splitMessage;

    int recHostId;
    int recConnectionId;
    int recChannelId;
    byte[] recBuffer = new byte[1024];
    int bufferSize = 1024;
    int dataSize;
    string message;
    byte error;

    NetworkEventType recNetworkEvent;
    Stream stream;
    BinaryFormatter formatter;

    private void openSocket()
    {
        //Oeffnet einen Socket der zum Senden und Empfangen verwendet wird
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Reliable);

        int maxConnections = 10;
        HostTopology topology = new HostTopology(config, maxConnections);

        //TODO: Das hier wieder rausnehmen, ist bei lokalen Tests jedoch notwendig
        socketPort = Random.Range(7000, 7900);

        socketId = NetworkTransport.AddHost(topology, socketPort);
        Debug.Log("Socket Open. SocketId is: " + socketId);
    }

    //Verbindet ueber den erstellten Socket zum Server
    private void connect()
    {
        if (ipaddress == null)
        {
            ipaddress = "127.0.0.1";
        }
        connectionId = NetworkTransport.Connect(socketId, ipaddress, 777, 0, out error);
        Debug.Log("Connected to server. ConnectionId: " + connectionId);
    }

    private void disconnect()
    {
        NetworkTransport.Disconnect(socketId, connectionId, out error);
        Debug.Log("disconnected from server.");
    }

    public void sendMessage(string message)
    {
        Debug.Log("sende Nachricht: " + message);
        byte[] buffer = new byte[1024];
        stream = new MemoryStream(buffer);
        formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);

        NetworkTransport.Send(socketId, connectionId, channelId, buffer, bufferSize, out error);
    }

    // Use this for initialization
    void Start () {
        openSocket();
        connect();
    }

    private void checkMessages()
    {
        recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

        if (error < 0)
        {
            Debug.Log("Error: " + error);
            return;
        }

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("incoming connection event received");
                //Wenn wir verbunden sind fragen wir den Server nach unserer Position in der Liste
                sendMessage("1;");
                break;
            case NetworkEventType.DataEvent:
                stream = new MemoryStream(recBuffer);
                formatter = new BinaryFormatter();
                message = formatter.Deserialize(stream) as string;
                Debug.Log("incoming message event received: " + message);
                dissectMessage(message);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("remote client event disconnected");
                break;
        }
    }

    private void dissectMessage(string recMessage)
    {
        //Nimmt die Nachricht auseinander und gibt sie an das Spiel
        splitMessage = recMessage.Split(';');
        myGame.processMessage(splitMessage);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("b"))
        {
            disconnect();
        }
        checkMessages();
    }
}
