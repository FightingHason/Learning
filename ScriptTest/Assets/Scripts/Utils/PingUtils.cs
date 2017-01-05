using UnityEngine;
using System.Collections;

public class PingUtils : MonoBehaviour {

    public string ip = "172.19.1.48";
    public int mix = 10;
    public int max = 100;
    public int maxNet = 500;

    private string item = "|";
    private int pingBackMS = 0;
    private int needToDrawNum = 0;
    private string needToDrawContent = "";
    private Ping ping;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        StartCoroutine("PingIP");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (pingBackMS <= 0) return;

        GUI.color = Color.green;
        GUI.Label(new Rect(0,0,10000,100),"Ping:"+needToDrawContent+" "+pingBackMS);
        GUI.Label(new Rect(0, 100, 10000, 100), "Ping Num:" + needToDrawNum);
    }

    /// <summary>
    /// Ping ip
    /// </summary>
    /// <returns></returns>
    public IEnumerator PingIP()
    {
        ping = new Ping(ip);
        while (!ping.isDone)
        {
            yield return null;
        }

        pingBackMS = ping.time;
        needToDrawNum = getDrawNum(pingBackMS);
        int i = 0;
        needToDrawContent = "";
        for (i = 0; i < needToDrawNum; ++i)
        {
            needToDrawContent += item;
        }

        StartCoroutine("PingIP");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pingBackMS"></param>
    /// <returns></returns>
    private int getDrawNum(int pingBackMS)
    {
        int need = 0;
        if (pingBackMS >= maxNet)
        {
            need = mix;
        }
        else
        {
            need = max*(maxNet-pingBackMS)/maxNet;
        }

        if (need < mix) need = mix;
        
        return need;
    }

}
