using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BleScanner : MonoBehaviour {

    public int LimitRSSI = -80;
    public bool _isScanning = false;
    private Action<string> onBleStatus;

    void Start ()
    {
        initailize (OnScannerInitialized);
    }

    void OnScannerInitialized()
    {
        //블루투스 
        LogManager.Log("Bluetooth OnInitialized Success");
    }

    internal void startScan (string deviceNamePrefix, Action<string> Callback) {
        callOnBleStatus ("Scanning");
        _isScanning = true;
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, null, (address, name, rssi, bytes) => {
            if (_isScanning && rssi > LimitRSSI && (name.IndexOf(deviceNamePrefix) > -1 && name.IndexOf("LIFEPLUS") == -1))
            {
                stopScan();
                Callback(address);
                callOnBleStatus("OnScanned");
            }
        }, true);
    }

    internal void stopScan () {
        if (_isScanning) {
            _isScanning = false;
            BluetoothLEHardwareInterface.StopScan ();
        }
    }

    public void initailize (Action onInitailized) {
        BluetoothLEHardwareInterface.Initialize (true, false, () => {
                if (onInitailized != null)
                    onInitailized ();
            },
            (error) => {

                BluetoothLEHardwareInterface.Log ("Error: " + error);
                if (error.Contains ("Bluetooth LE Not Enabled"))
                    BluetoothLEHardwareInterface.BluetoothEnable (true);
            });
    }

    internal void setOnBleStatus (Action<string> onBleStatus) {
        this.onBleStatus = onBleStatus;
    }

    private void callOnBleStatus (string message) {
        if (onBleStatus != null)
            onBleStatus (message);
    }
}