using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HtmlAgilityPack;
using static LyricResponse;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

public class CubeActions : MonoBehaviour
{
    AudioSource audio;
    WebUtils webUtils;
    public Text lyrics;
	public GameObject sceneController;

    // Start is called before the first frame update
    void Start()
    {
        audio = sceneController.GetComponent<AudioSource>();
        webUtils = new WebUtils();

        AudioClip lyric = Resources.Load<AudioClip>("Music/" + Properties.selectedSong);

        Debug.Log("Music/" + Properties.selectedSong);
        Debug.Log(lyric);
        audio.clip = lyric;

        audio.Play();
    }
    void OnMouseUp()
    {
        Debug.Log("hi");
    }
    // Update is called once per frame
    void Update()
    {
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
            if (audio.isPlaying)
            {
                audio.Pause();
            }
            else
            {
                audio.Play();
            }
        }
    }

   
    bool isValidReg(string str)
    {
        Regex rgx = new Regex(@"^\[[0-9]{2}:[0-9]{2}\.[0-9]{2}\].*$");
        return rgx.IsMatch(str);
    }
}
