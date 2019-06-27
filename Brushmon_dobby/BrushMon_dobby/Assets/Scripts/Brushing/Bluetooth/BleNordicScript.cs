using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BleNordicScript : MonoBehaviour {
    private bool _connecting = false;
    private string _connectedID = null;
    private string _serviceUUID = "0001";
    private string _readCharacteristicUUID = "0003";
    private string _writeCharacteristicUUID = "0002";
    private float _subscribingTimeout = 0f;
    private bool _readFound = false;
    private bool _writeFound = false;
    private BrushAlgorithm brushAlgorithm;

    private int _toothMatchRegion = 1;
    private bool _isToothBrushing = false;

    private Action<string> OnBleStatus;
    private Action<int> OnValidateToothBehavior;
    private Action<int> OnKeyStateBehavior;

    private string _connectedDeviceAddress;
    bool _connected = false;

    bool isEndDisconnect = false;
    public bool IsEndDisconnect { get { return isEndDisconnect; } }

    public bool Connected {
        get { return _connected; }
        set {
            _connected = value;

            if (_connected) {
                _connecting = false;
            } else {
                _connectedID = null;
            }
        }
    }

    bool isCommandable = false;

    public void Start () {
        Connected = false;
        brushAlgorithm = new BrushAlgorithm ();
        isCommandable = false;
    }

    void disconnect (Action<string> action) {
        BluetoothLEHardwareInterface.DisconnectPeripheral (_connectedDeviceAddress, action);
    }

    public void setOnBleStatus (Action<string> OnBleStatus) {
        this.OnBleStatus = OnBleStatus;
    }

    private void callOnBleStatus (string message) {
        if (OnBleStatus != null)
            OnBleStatus (message);
    }

    internal void setOnValidateToothBehavior (Action<int> onValidateToothBehavior) {
        this.OnValidateToothBehavior = onValidateToothBehavior;
    }

    internal void setOnKeyStateBehavior (Action<int> onKeyStateBehavior) {
        this.OnKeyStateBehavior = onKeyStateBehavior;
    }

    internal void setToothMatchRegion (int region) {
        this._toothMatchRegion = region;
    }

    internal void setToothBrushing (bool toothBrushing) {
        this._isToothBrushing = toothBrushing;
    }

    public void send (string data) {
        if (data.Length > 0) {
            LogManager.Log ("send : " + data);
            byte[] bytes = ASCIIEncoding.UTF8.GetBytes (data);
            if (bytes.Length > 0)
                SendBytes (bytes);
        }
    }

    public void connect (string targetAddress) {
        _connectedDeviceAddress = targetAddress;
        _readFound = false;
        _writeFound = false;
        BluetoothLEHardwareInterface.ConnectToPeripheral (_connectedDeviceAddress, (address) => {
                isCommandable = false;
                callOnBleStatus ("OnConnected");
            },
            (address, serviceUUID) => {
            },
            (address, serviceUUID, characteristicUUID) => {
                if (IsEqual (serviceUUID, _serviceUUID)) {
                    _connectedID = address;

                    Connected = true;

                    if (IsEqual (characteristicUUID, _readCharacteristicUUID)) {
                        _readFound = true;
                    } else if (IsEqual (characteristicUUID, _writeCharacteristicUUID)) {
                        _writeFound = true;
                    }
                }
            }, (address) => {
            });
    }

    public void disconnect () {
        StartCoroutine (Disconnect ());
    }

    IEnumerator Disconnect()
    {
        BluetoothLEHardwareInterface.StopScan();

        if (Connected)
        {
            isEndDisconnect = false;
            BluetoothLEHardwareInterface.DisconnectAll();
            BluetoothLEHardwareInterface.DeInitialize(EndDisconnect);
            yield return null;
        }
        else
        {
            isEndDisconnect = true;
            BluetoothLEHardwareInterface.DisconnectAll();
            BluetoothLEHardwareInterface.FinishDeInitialize();
        }
        isCommandable = false;
        Connected = false;
    }

    // 블루투스 연결끊기 완료 콜백함수
    void EndDisconnect()
    {
        BluetoothLEHardwareInterface.FinishDeInitialize();
        isEndDisconnect = true;
    }

    public int[] getBrushingResult () {
        return brushAlgorithm.computeresult ();
    }

    // Update is called once per frame
    void Update () {
        if (_readFound && _writeFound) {
            _readFound = false;
            _writeFound = false;

            _subscribingTimeout = 1f;
        }

        if (_subscribingTimeout > 0f) {
            _subscribingTimeout -= Time.deltaTime;
            if (_subscribingTimeout <= 0f) {
                _subscribingTimeout = 0f;
                BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_connectedID, FullUUID (_serviceUUID), FullUUID (_readCharacteristicUUID), (deviceAddress, notification) => {
                }, (deviceAddress2, characteristic, data) => {

                    BluetoothLEHardwareInterface.Log (string.Format ("data length: {0}", data.Length));
                    if (deviceAddress2.CompareTo (_connectedID) == 0) {
                        // BluetoothLEHardwareInterface.Log(string.Format("data length: {0}", data.Length));
                        if (data.Length == 13 && data[0] == 0x64) {
                            callOnBleStatus ("OnCommandable");
                            callOnBleStatus ("DeviceVibration:" + data[3]);
                        } else {
                            if (data[0] == 0x73 && !isCommandable) {
                                send ("g");
                                isCommandable = true;
                            }
                            if (data[7] == 1) {
                                OnKeyStateBehavior (data[7]);
                            }

                            if (_isToothBrushing) {
                                int result = brushAlgorithm.determineCurResult (data, _toothMatchRegion);

                                if (result == 1) {
                                    OnValidateToothBehavior (result);
                                }
                            }

                        }
                    }

                });
            }
        }
    }

    string FullUUID (string uuid) {
        return "6E40" + uuid + "-B5A3-F393-E0A9-E50E24DCCA9E";
    }

    bool IsEqual (string uuid1, string uuid2) {
        if (uuid1.Length == 4)
            uuid1 = FullUUID (uuid1);
        if (uuid2.Length == 4)
            uuid2 = FullUUID (uuid2);

        return (uuid1.ToUpper ().CompareTo (uuid2.ToUpper ()) == 0);
    }

    void SendByte (byte value) {
        byte[] data = new byte[] { value };
        BluetoothLEHardwareInterface.WriteCharacteristic (_connectedID, FullUUID (_serviceUUID), FullUUID (_writeCharacteristicUUID), data, data.Length, true, (characteristicUUID) => {
            BluetoothLEHardwareInterface.Log ("Write Succeeded");
        });
    }

    void SendBytes (byte[] data) {
        BluetoothLEHardwareInterface.WriteCharacteristic (_connectedID, FullUUID (_serviceUUID), FullUUID (_writeCharacteristicUUID), data, data.Length, true, (characteristicUUID) => {
            BluetoothLEHardwareInterface.Log ("Write Succeeded");
        });
    }
}