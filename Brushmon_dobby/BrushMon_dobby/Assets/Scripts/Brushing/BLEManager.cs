using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BLEManager : MonoSingleton<BLEManager>
{
    public BluetoothStatus currentBluetoothStatus = BluetoothStatus.NONE;

    // public string deviceNamePrefix = "BMT100";
    private BleNordicScript bleNordicScript;
    private BleScanner bleScanner;
    private string connectedDeviceAddress;
    private Action<int> OnValidateToothBehavior;
    private Action<int> OnKeyStateBehavior;

    // 블루투스 연결 끊기 완료 체크, 완료되면 true 반환
    public bool IsEndDisconnect { get { return bleNordicScript.IsEndDisconnect; } }

    bool isConnectPeripheral = false;
    public bool IsConnectPeripheral { get { return isConnectPeripheral; } }

    void Start()
    {
        initializeBLEScript();
        initializeBLEScan();
    }
    
    public void sendDataDelay(string data, float delay){
        StartCoroutine(delayCommandSend(data, delay));
    }
    IEnumerator delayCommandSend(string data, float delay){
        yield return new WaitForSeconds(delay);
        BLEManager.Instance.sendData(data);
    }
    private void initializeBLEScript()
    {
        gameObject.AddComponent<BleNordicScript>();
        bleNordicScript = GetComponent<BleNordicScript>();
        bleNordicScript.setToothBrushing(false);
        bleNordicScript.setToothMatchRegion(1);
    }

    private void initializeBLEScan()
    {
        gameObject.AddComponent<BleScanner>();
        bleScanner = GetComponent<BleScanner>();
    }

    public void sendData(string data)
    {
        try{
            bleNordicScript.send(data);
        } catch{

        }
    }

    public void startScan()
    {
        bleScanner.startScan(ConfigurationData.Instance.GetValue<string>("BleDeviceName", "BMT100"), onScanned);
    }

    private void onScanned(string address)
    {
        isConnectPeripheral = true;
        //연결 끊김 콜백 함수 등록
        BluetoothLEHardwareInterface.SetDisconnectPeripheral(DisconnectPeripheral);

        connectedDeviceAddress = address;
        bleNordicScript.connect(address);
    }

    // 블루투스 연결 끊김 콜백 함수
    void DisconnectPeripheral(string msg)
    {
        isConnectPeripheral = false;
        LogManager.Log("DisconnectPeripheral : " + msg);
    }

    public void StopScan()
    {
        bleScanner.stopScan();
    }

    public void disconnect()
    {
        bleScanner.stopScan();
        bleNordicScript.disconnect();
    }

    public bool isConnected(){
        return bleNordicScript.Connected;
    }

    public void setToothMatchRegion(int region)
    {
        bleNordicScript.setToothMatchRegion(region);
    }

    public int[] getBrushingResult(){
       return bleNordicScript.getBrushingResult();
    }

    public void setToothBrushing(bool isToothBrushing)
    {
        bleNordicScript.setToothBrushing(isToothBrushing);
    }

    internal void setOnValidateToothBehavior(Action<int> onValidateToothBehavior)
    {
        this.OnValidateToothBehavior = onValidateToothBehavior;
        bleNordicScript.setOnValidateToothBehavior(OnValidateToothBehavior);
    }

    internal void setOnKeyStateBehavior(Action<int> onKeyStateBehavior)
    {
        this.OnKeyStateBehavior = onKeyStateBehavior;
        bleNordicScript.setOnKeyStateBehavior(OnKeyStateBehavior);
    }

    public void setOnBleStatus(Action<string> OnBleStatus)
    {
        bleNordicScript.setOnBleStatus(OnBleStatus);
        bleScanner.setOnBleStatus(OnBleStatus);
    }
}
