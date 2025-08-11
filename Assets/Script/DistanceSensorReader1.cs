using UnityEngine;
using System.IO.Ports;
using TMPro;

public class DistanceSensorReader1 : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/cu.usbserial-0001"; // Macの場合
    public int baudRate = 115200; // SchooMyの設定に合わせる

    [Header("UI")]
    public TextMeshProUGUI distanceText;

    private SerialPort serial;
    // 他スクリプトから参照できる
    public static float distance = 0f;

    void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.ReadTimeout = 50;
        serial.Open();
    }

    void Update()
    {
        if (serial != null && serial.IsOpen)
        {
            try
            {
                string line = serial.ReadLine();
                if (float.TryParse(line, out float value))
                {
                    distance = value;
                    // UIに表示（小数点1桁まで）
                    distanceText.text = $"{distance:F0}";

                }
            }
            catch { /* タイムアウトは無視 */ }
        }
    }

    void OnDestroy()
    {
        if (serial != null && serial.IsOpen)
        {
            serial.Close();
        }
    }
}
