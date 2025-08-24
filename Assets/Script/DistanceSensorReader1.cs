//using UnityEngine;
//using System.IO.Ports;
//using TMPro;

//public class DistanceSensorReader1 : MonoBehaviour
//{
//    [Header("Serial Settings")]
//    public string portName = "/dev/cu.usbserial-0001"; // Macの場合
//    public int baudRate = 115200; // SchooMyの設定に合わせる

//    //[Header("UI")]
//    //public TextMeshProUGUI distanceText;

//    private SerialPort serial;
//    // 他スクリプトから参照できる
//    public static float distance = 0f;

//    void Start()
//    {
//        serial = new SerialPort(portName, baudRate);
//        serial.ReadTimeout = 50;
//        serial.Open();
//    }

//    void Update()
//    {
//        if (serial != null && serial.IsOpen)
//        {
//            try
//            {
//                string line = serial.ReadLine();
//                if (float.TryParse(line, out float value))
//                {
//                    distance = value;
//                    // UIに表示（小数点1桁まで）
//                    //distanceText.text = $"{distance:F0}";

//                }
//            }
//            catch { /* タイムアウトは無視 */ }
//        }
//    }

//    void OnDestroy()
//    {
//        if (serial != null && serial.IsOpen)
//        {
//            serial.Close();
//        }
//    }
//}






using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class DistanceSensorReader1 : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/cu.usbserial-0001"; // Macの場合
    public int baudRate = 115200;

    public static float distance = 0f; // 他スクリプトから参照可能

    private SerialPort serial;
    private Thread readThread;
    private bool running = false;
    private object lockObj = new object(); // スレッド安全用

    void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.ReadTimeout = 50;

        try
        {
            serial.Open();
        }
        catch (System.Exception e)
        {
            Debug.LogError("SerialPort Open Failed: " + e.Message);
            return;
        }

        running = true;
        readThread = new Thread(ReadSerialLoop);
        readThread.Start();
    }

    void ReadSerialLoop()
    {
        while (running && serial != null && serial.IsOpen)
        {
            try
            {
                string line = serial.ReadLine();
                if (float.TryParse(line, out float value))
                {
                    lock (lockObj)
                    {
                        distance = value; // スレッドセーフに更新
                    }
                }
            }
            catch (System.TimeoutException)
            {
                // タイムアウトは無視
            }
            catch (System.Exception e)
            {
                Debug.LogError("Serial Read Error: " + e.Message);
            }
        }
    }

    public static float GetDistance()
    {
        // メインスレッドで安全に読み取りたい場合
        lock (typeof(DistanceSensorReader1))
        {
            return distance;
        }
    }

    void OnDestroy()
    {
        running = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(); // スレッド終了待ち
        }

        if (serial != null && serial.IsOpen)
        {
            serial.Close();
        }
    }
}
