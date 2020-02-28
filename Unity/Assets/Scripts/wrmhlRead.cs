using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script is used to read all the data coming from the device. For instance,
If arduino send ->
								{"1",
								"2",
								"3",}
readQueue() will return ->
								"1", for the first call
								"2", for the second call
								"3", for the thirst call

This is the perfect script for integration that need to avoid data loose.
If you need speed and low latency take a look to wrmhlReadLatest.
*/

public class wrmhlRead : MonoBehaviour {
    public GameObject mainBall;
    GameObject heartBall;
    wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen your computer and hardware.

	[Tooltip("SerialPort of your device.")]
	public string portName = "COM5";

	[Tooltip("Baudrate")]
	public int baudRate = 250000;


	[Tooltip("Timeout")]
	public int ReadTimeout = 20;

	[Tooltip("QueueLenght")]
	public int QueueLenght = 1;

	void Start () {
		myDevice.set (portName, baudRate, ReadTimeout, QueueLenght); // This method set the communication with the following vars;
		//                              Serial Port, Baud Rates, Read Timeout and QueueLenght.
		myDevice.connect (); // This method open the Serial communication with the vars previously given.
        Vector3 center = transform.position;
        Vector3 heartPos = center;
        heartBall = Instantiate(mainBall, heartPos, transform.rotation);
        var heartRenderer = heartBall.GetComponent<Renderer>();
        heartRenderer.material.SetColor("_Color", Color.red);
    }

	// Update is called once per frame
	void Update () {
        //	print (myDevice.readQueue () ); // myDevice.read() return the data coming from the device using thread.
        print(myDevice.readQueue());

        if (myDevice.readQueue() == "High")
        {
            Vector3 lerpScale = new Vector3(heartBall.transform.localScale.x, 1.5f, heartBall.transform.localScale.z);
            heartBall.transform.localScale = lerpScale;
        }
        else
        {
            Vector3 lerpScale = new Vector3(heartBall.transform.localScale.x, 0.01f, heartBall.transform.localScale.z);
            heartBall.transform.localScale = Vector3.Lerp(heartBall.transform.localScale, lerpScale, Time.deltaTime);
        }

    }

	void OnApplicationQuit() { // close the Thread and Serial Port
		myDevice.close();
	}
}
