// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Spine.Unity;
// using UnityEngine;

// public class PauseController : MonoBehaviour
// {

// 	SkeletonGraphic BluetoothStatus;
//     // IEnumerator Awake()
//     // {
// 	// 	yield return new WaitForSeconds(2f);
//     //     BLEManager.Instance.Initailize();
// 	// 	// BluetoothStatus = GameObject.Find("/UI/Spine-BluetoothStatus").GetComponent<SkeletonGraphic>();
//     // }

// 	// void Start(){
// 	// 	BLEManager.Instance.setOnBleStatus(OnBleStatus);
// 	// 	// BLEManager.Instance.startScan();
// 	// 	LogManager.Log("PauseController : connected "+BLEManager.Instance.isConnected().ToString());
		
// 	// }

//     private void OnBleStatus(string state)
//     {
//         switch (state)
//         {
//             case "Scanning":
// 				LogManager.Log("PauseController : Scanning");
//                 // BluetoothStatus.AnimationState.SetAnimation(0, "Scan", true);
//                 break;
//             case "OnScanned":
//                 break;
//             case "OnScanStoped":
//                 break;
//             case "OnCommandable":
// 				LogManager.Log("PauseController : Scanning");
//                 break;
//             case "OnConnected":
// 				LogManager.Log("PauseController : Scanning");
// 				BrushMonSceneManager.Instance.LoadScene("Brushing");
//                 break;
//             case "OnDisconnected":
// 				LogManager.Log("PauseController : Scanning");
//                 // BluetoothStatus.AnimationState.SetAnimation(0, "Disconnect", true);
//                 break;
//         }
//     }

// public void test(){

// }
// 	public void onBackPress()
//     {
//         BLEManager.Instance.sendData("q");
//         BLEManager.Instance.disconnect();
//         BrushMonSceneManager.Instance.LoadScene("Welcome");
//     }
// }
