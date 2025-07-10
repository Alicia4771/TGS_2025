using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Text;

public class M5BluetoothReceiver : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;

    private double angX = 200;
    private double accMagnitude = 200;

    void Start()
    {
        serialPort = new SerialPort("/dev/cu.hukkin", 115200);
        // /dev/cu.hukkin
        // ブルートゥース用シリアルポート

        // /dev/cu.usbserial-9552C3038B
        // 有線用シリアルポート
        serialPort.ReadTimeout = 100;


        try
        {
            serialPort.Open();
            //StartCoroutine(ReadLoop());
        }
        catch (System.Exception e)
        {
            Debug.LogError("シリアルポートを開けませんでした: " + e.Message);
        }


        //StartCoroutine(ReadLoop());
    }



    //    System.Collections.IEnumerator ReadLoop()
    //    {
    //        while (true)
    //        {
    //            if (serialPort != null && serialPort.IsOpen)
    //            {
    //                try
    //                {
    //                    string line = serialPort.ReadExisting();
    ////                    これは今届いている全データを即座に読み取る方法です。
    ////ReadLine()より待ち時間がないので、タイムラグの大半がこれで解消されます。

    //                    string[] atai = line.Split(',');

    //                    //string hosiiX =

    //                    double angX = 0;

    //                    try
    //                    {
    //                        angX = double.Parse(atai[4]);
    //                    }
    //                    catch
    //                    {
    //                        Debug.Log("変換失敗");
    //                    }

    //                    if (!string.IsNullOrEmpty(line))
    //                    {
    //                        Debug.Log("受信: " + line);
    //                        //Debug.Log("Xの傾き：" + atai[4]);
    //                        Debug.Log("Xの傾き(double)：" + angX);
    //                    }
    //                }
    //                catch (System.TimeoutException)
    //                {
    //                    // タイムアウトは無視（Bluetooth接続維持のため）
    //                }
    //                catch (System.Exception e)
    //                {
    //                    Debug.LogWarning("読み取りエラー: " + e.Message);
    //                }
    //            }

    //            yield return new WaitForSeconds(0.2f); // 適度に待つ
    //        }
    //    }




    private int count_emp = 0;
    private int count_non = 0;
    private int count_else = 0;
    private int count_data = 0;
    private int count_faul = 0;



    //StringBuilder buffer = new StringBuilder();

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            //string data = serialPort.ReadExisting();
            string[] dataList;

            try
            {
                string data = serialPort.ReadExisting();

                if (data == "")
                {
                    count_emp++;
                    //Debug.Log("受信データ: empty");
                }
                else
                {
                    count_data++;
                    //Debug.Log("受信データ: " + data);


                    try
                    {
                        dataList = data.Split(",");

                        double data_angX = double.Parse(dataList[0]);
                        double data_accMagnitude = double.Parse(dataList[1]);

                        Debug.Log("angX:" + data_angX + ", accMagnitude:" + data_accMagnitude);

                        setAngX(data_angX);
                        setAccMagnitude(data_accMagnitude);
                    }
                    catch
                    {
                        count_faul++;
                        //Debug.Log("なんか失敗");
                    }

                }
            }
            catch
            {
                count_non++;
                //Debug.Log("受信データ: non");
            }
            //string data = serialPort.ReadExisting();
            //buffer.Append(data);

            //while (buffer.ToString().Contains("\n"))
            //{
            //    int index = buffer.ToString().IndexOf("\n");
            //    string line = buffer.ToString().Substring(0, index).Trim();
            //    buffer.Remove(0, index + 1);

            //    Debug.Log("受信データ: " + line);
            //}


        }
        else {
            count_else++;
            //Debug.Log("受信データ: else");
        }

        //Debug.Log("emp:" + count_emp + ", non:" + count_non + ", else:" + count_else + ", data:" + count_data + ", faul:" + count_faul);

        //float frameTime = Time.deltaTime;
        //Debug.Log("フレームの長さ：" + frameTime);
    }



    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }


    // ゲッターメソッド
    /**
     * M5stickから受け取ったX軸の傾き
     * @return double
     */
    public double getAngX() {
        return this.angX;
    }

    /**
     * M5stickから受け取った加速度
     * @return double
     */
    public double getAccMagnitude() {
        return this.accMagnitude;
    }
}
