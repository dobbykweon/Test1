using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BMode
{
    None,
    //GetAddress,
    SetVibration,
    //NoneBrushDataUpload,
}

public class UIConnectingBrush : MonoBehaviour
{
    [SerializeField] GameObject objConnecting;
    [SerializeField] GameObject objSetVibration;

    [SerializeField] Image imgVibration;

    //[SerializeField] List<Text> txtVibPower;

    BMode mode = BMode.None;
    Action<int> setVibration;

    //ProfileVo currentProfileVo;

    bool isConnect = false;
    int vibration = 1;
    int firstVib = 1;

    private void Start()
    {
        //currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo>("CurrentProfile");
    }

    public void InitSetVibration(Action<int> action, int _vibration)
    {
        setVibration = action;
        vibration = _vibration;
        firstVib = _vibration;
        mode = BMode.SetVibration;

        imgVibration.fillAmount = (float)vibration / 4f;

        StartCoroutine(SetBLEManager());
    }


    IEnumerator SetBLEManager()
    {
        yield return new WaitForSeconds(0.1f);
        BLEManager.Instance.setOnBleStatus(OnBleStatus);
        BLEManager.Instance.setOnKeyStateBehavior(OnKeyStateBehavior);
        yield return new WaitForSeconds(0.2f);

        BLEManager.Instance.startScan();
    }

    private void OnBleStatus(string state)
    {
        LogManager.Log("OnBleStatus : " + state);
        //BMUtil.Instance.OpenToast(state);

        switch (state)
        {
            case "Scanning":
                objConnecting.SetActive(true);
                break;
            case "OnScanned":
                //if (mode == BMode.SetVibration)
                //{
                //    objConnecting.SetActive(false);
                //    objSetVibration.SetActive(true);
                //}
                //else if (mode == BMode.NoneBrushDataUpload)
                //{
                //    objConnecting.SetActive(false);
                //    objNoneBrushDataUpload.SetActive(true);
                //}
                break;

            case "OnConnected":
                if (mode == BMode.SetVibration)
                {
                    objConnecting.SetActive(false);
                    objSetVibration.SetActive(true);
                }
                break;

            case "OnScanStoped":
                break;

            case "OnCommandable":

                isConnect = true;

                if (mode == BMode.SetVibration)
                {
                    //BLEManager.Instance.sendDataDelay("s", 0.2f);
                    BLEManager.Instance.sendData("s");
                    BLEManager.Instance.setToothBrushing(true);
                    BLEManager.Instance.sendData(vibration.ToString());
                }
                break;

            case "OnDisconnected":
                BLEManager.Instance.setToothBrushing(false);
                break;
        }
    }

    void Disconnect()
    {
        BLEManager.Instance.disconnect();
    }

    internal void OnKeyStateBehavior(int key)
    {
        if (mode == BMode.SetVibration && isConnect == true)
        {
            vibration++;
            if (vibration > 4) vibration = 0;
            imgVibration.fillAmount = (float)vibration / 4f;

            //if (vibration == 0)
            //{
            //    for (int i = 0; i < txtVibPower.Count; i++)
            //    {
            //        txtVibPower[i].color = new Color(txtVibPower[i].color.r, txtVibPower[i].color.g, txtVibPower[i].color.b, 0.2f);
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < txtVibPower.Count; i++)
            //    {
            //        txtVibPower[i].color = new Color(txtVibPower[i].color.r, txtVibPower[i].color.g, txtVibPower[i].color.b, 1);
            //    }
            //}

            StartCoroutine(SetVibration());
        }
    }

    IEnumerator SetVibration()
    {
        BLEManager.Instance.setToothBrushing(false);

        yield return new WaitForSeconds(0.1f);

        BLEManager.Instance.sendDataDelay("s", 0.2f);
        BLEManager.Instance.setToothBrushing(true);
        BLEManager.Instance.sendData(vibration.ToString());
    }

    public void BtnSetVibration()
    {
        BMUtil.Instance.OpenLoading();
        setVibration(vibration);
        Disconnect();
        StartCoroutine(Close());
    }

    IEnumerator Close()
    {
        while (BLEManager.Instance.IsEndDisconnect == false)
        {
            yield return null;
        }

        BMUtil.Instance.CloseLoading();
        Destroy(gameObject);
    }

    public void BtnCancel()
    {
        if (BMUtil.Instance.IsLoading() == true) return;

        LogManager.Log("BtnCancel : " + BLEManager.Instance.isConnected());

        //if (!BLEManager.Instance.isConnected())
        //    BLEManager.Instance.StopScan();

        BMUtil.Instance.OpenLoading();

        if (mode == BMode.SetVibration)
        {
            BLEManager.Instance.sendData(firstVib.ToString());
            setVibration(firstVib);
        }

        Disconnect();
        StartCoroutine(Close());
    }

    private IEnumerator CloseDataUpload(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LogManager.Log("Escape");
            BtnCancel();
        }
    }
}