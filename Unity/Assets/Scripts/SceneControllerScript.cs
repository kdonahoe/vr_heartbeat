using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SceneControllerScript : MonoBehaviour
{
    public GameObject canvas;
    AudioSource audioSource;
    int numCubes = 256;
    public float[] spectrum = new float[6];

    public GameObject ballPrefab, ground;
    public GameObject mainBall;
    GameObject[] balls = new GameObject[512];
    GameObject heartBall;
    public float scaler;

    public Color[] colors = new Color[512];

    public float circleSize;
    public int preFabScale;
    int counter = 0;

    public GameObject particlePrefab;
    public int particleCount;
    public float particleMinSize;
    public float particleMaxSize;
    private bool alreadyExploded;
    public TextMeshProUGUI lyrics;
    Vector3 offset;
    public float lyricsDepth;
    public float lyricsHeight;

    public List<GameObject> cubeList = new List<GameObject>();

    public int num_bpm = 0;
    public bool breatheIn = true;
    public bool breatheWait = false;
    public bool breatheOut = false;
    void Start()
    {
        offset = canvas.transform.position - Camera.main.transform.position;
        audioSource = GetComponent<AudioSource>();
		AudioClip lyric = Resources.Load<AudioClip>("Music/" + Properties.selectedSong);
		audioSource.clip = lyric;
        audioSource.Play();
        Vector3 center = transform.position;

        Vector3 heartPos = center;
        heartBall = Instantiate(mainBall, heartPos, transform.rotation);
        var heartRenderer = heartBall.GetComponent<Renderer>();
        heartRenderer.material.SetColor("_Color", Color.red);

        for (int i = 0; i < 10; i++)
        {
            float ang = (360f/6f) * i;
            Vector3 pos = circleUp(center, 6.5f, ang);
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            GameObject cubeInstance = Instantiate(ballPrefab, pos, rot);

            var cubeRenderer = cubeInstance.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.5f, 0f, 0.1f, 0.5f, 1f));
           // cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.5f, 0.7f, 1f, 1f, 0.5f, 1f));


            colors[i] = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            //cubeRenderer.material.SetColor("_Color", colors[i]);

            balls[i] = cubeInstance;
            cubeList.Add(cubeInstance);
        }

        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            //cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }
    }
    Vector3 circleUp(Vector3 center, float radius, float ang)
    {
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.z + 2;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
    Vector3 velocity;
    // Update is called once per frame
    void Update()
    {
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
        //canvas.transform.LookAt(Camera.main.transform, Vector3.up);
        //canvas.transform.rotation.eulerAngles = new Vector3(0, 0, 0);
        Vector3 newV = new Vector3(Camera.main.transform.position.x + lyricsDepth * Camera.main.transform.forward.x, lyricsHeight, Camera.main.transform.position.z + lyricsDepth * Camera.main.transform.forward.z);
        canvas.transform.position = Vector3.SmoothDamp(canvas.transform.position, newV, ref velocity, 1f * Time.deltaTime);
        //canvas.transform.position = Vector3.SmoothDamp(canvas.transform.position, Camera.main.transform.position + 20f * Camera.main.transform.forward + new Vector3(-Camera.main.transform.position.x, 2f,0), ref velocity, 1f * Time.deltaTime);
        if (counter % 5  == 0)
        {
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            float average = getAvg(spectrum);
            
            if(spectrum[0] > 0)
            {
                heartBall.transform.localScale = new Vector3(heartBall.transform.localScale.x, (spectrum[0] * 100 * scaler), heartBall.transform.localScale.z);
            }

            if(heartBall.transform.localScale.y > 0.0116)
            {
                num_bpm += 1;
            }

          //  if((num_bpm % 4 == 0 || num_bpm % 17 == 0) && breatheIn)
         //   {
            //    balls[0].transform.localScale = new Vector3(heartBall.transform.localScale.x + 1, heartBall.transform.localScale.y + 1, heartBall.transform.localScale.z + 1);
              //  breatheOut = true;
              //  breatheIn = false;
         //   }
         //   if(num_bpm%10 == 0 && breatheOut)
          //  {
              //  balls[0].transform.localScale = new Vector3(heartBall.transform.localScale.x - 2, heartBall.transform.localScale.y -2, heartBall.transform.localScale.z -2);
            //    breatheOut = true;
          //  }
          //  if (num_bpm % 17 == 0 && breatheOut)
          //  {
         //       breatheIn = true;
         ////       breatheOut = false;
        //    }
            //for (int j = 0; j < numCubes; j++)
            //{
            //    balls[j].transform.localScale = new Vector3(balls[j].transform.localScale.x, (spectrum[j] * 1000 * scaler), balls[j].transform.localScale.z);
            //     if(spectrum[j]>0)
            //    {
            //        print("yeet");
            //    }
            //    if (j > 0)
            //    {
            //        var cubeRenderer = balls[j].GetComponent<Renderer>();
            //        //cubeRenderer.material.SetColor("_Color", colors[j]);
            //    }


            //}
        }
        counter++;
        //    numRotation++;

        //  if(numRotation % 60 == 0) //every 20 times
        //   {
        // rotateColors(colors);
        // }
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                Interact(hit);
        }
#else
        if (Input.touchCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                Interact(hit);
        }
#endif

    }

    void Interact(RaycastHit hit)
    {
        if (hit.collider.CompareTag("User"))
        {
            //songLyrics = webUtils.getTopLyrics(Properties.songsList[0]);
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.Play();
            }
        }
    }

    float getAvg(float[] spectrum)
    {
        float num = 0;
        for (int j = 0; j < numCubes; j++)
        {
            num += spectrum[j];
        }
        float avg = num / numCubes;
        return (avg);
    }
    void rotateColors(Color[] colors)
    {
        Color temp = colors[0];
        for (int i = 0; i < numCubes - 1; i++)
        {
            colors[i] = colors[i + 1];
        }
        colors[numCubes - 1] = temp;
    }

  
    bool isValidReg(string str)
    {
        Regex rgx = new Regex(@"^\[[0-9]{2}:[0-9]{2}\.[0-9]{2}\].*$");
        return rgx.IsMatch(str);
    }
}
