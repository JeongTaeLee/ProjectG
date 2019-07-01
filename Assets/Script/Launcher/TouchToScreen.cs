﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Photon.Pun;

public class TouchToScreen : MonoBehaviourPunCallbacks
{
    //
    public GameObject Character2D = null;

    public float Character2DHeight = 1f;
    public float Character2DSpeed = 1f;

    private Vector3 Character2DOriginPos;
    private float Character2DSin = 0f;

    //
    public GameObject Logo = null;

    public float LogoHeight = 1f;
    public float LogoSpeed = 1f;

    private Vector3 LogoOriginPos;
    private float LogoSin = Mathf.PI;

    // 
    public GameObject TouchToScreenText = null;

    void Start()
    {
        Character2DOriginPos = Character2D.transform.position;
        LogoOriginPos = Logo.transform.position;

        TouchToScreenText.SetActive(true);
    }

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = "1.0";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버에 연결되었습니다.");
        PhotonNetwork.JoinLobby();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            TouchToScreenText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Connecting...";

            Connect();
        }

        if (Character2DSin >= (Mathf.PI * 2))
            Character2DSin = 0f;

        float y = Character2DOriginPos.y + (Mathf.Sin(Character2DSin) * Character2DHeight);
        Character2D.transform.position = new Vector3(Character2DOriginPos.x, y, 0f);

        Character2DSin += Time.deltaTime * Character2DSpeed;

        if (LogoSin >= (Mathf.PI * 2))
            LogoSin = 0f;

        y = LogoOriginPos.y + (Mathf.Sin(LogoSin) * LogoHeight);
        Logo.transform.position = new Vector3(LogoOriginPos.x, y, 0f);

        LogoSin += Time.deltaTime * LogoSpeed;
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Launcher", LoadSceneMode.Single);
    }
}
