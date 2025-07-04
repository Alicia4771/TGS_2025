using System;
using UnityEngine;

public class moveSample : MonoBehaviour
{
    [SerializeField, Tooltip("M5StickのBluetooth受信クラス")]
    [Header("M5StickのBluetooth受信クラス")]


    private M5BluetoothReceiver M5;

    [SerializeField, Tooltip("ジャンプのクラス")]
    [Header("ジャンプのクラス")]
    private jumpSample junpMove;

    private double X_now = 200;

    private double[] X_list = new double[20];

    private double X_data_now = 200;
    private double X_data_befo = 200;

    [SerializeField, Tooltip("進む倍率")]
    [Header("進む倍率")]
    private double move_ratio = 1.0;


    // 追加兼岩
    private double X_data_befo1 = 200;


    private Animator animator;





    // 繰り上がり
    private int count_over = 5;

    [SerializeField]
    private double stillness = 0.5;

    private double flont_start_position = 0;
    private double flont_stop_position = 0;
    private double back_start_position = 0;
    private double back_stop_position = 0;

    //private bool streat;

    private int streat;
    private int[] streat_list = new int[40];


    private bool jump_flag = false;         // ジャンプ中かどうかのフラグ
    [SerializeField, Tooltip("ジャンプの制限値(どこまでをジャンプとみなすか)")]
    [Header("ジャンプの制限値")]
    private double jump_limit = 3.0;

    private double streat_distance; // 前に進んだ距離
    // (1:前に進んでいる, 0:静止している, -1:後ろに戻っている)





    private Rigidbody rb;
    public float moveSpeed = 2f; // キャラの移動スピード



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // X_list配列の初期化
        for (int i = 0; i < X_list.Length; i++) X_list[i] = 200;

        // 初期値の設定
        X_data_now = 200;
        X_data_befo = 200;
        count_over = 5;

        flont_start_position = 0;
        flont_stop_position = 0;
        back_start_position = 0;
        back_stop_position = 0;

        streat_distance = 0;

        streat = 0;
        for (int i = 0; i < streat_list.Length; i++) streat_list[i] = 0;

        jump_flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        // デバッグのため、ひとまずコメントアウト
        if (!jump_flag)
        {
            if (M5.getAccMagnitude() > jump_limit)
            {
                Debug.Log("ジャンプ開始");
                junpMove.StartJump();
                jump_flag = true;
            }
            else if (M5.getAngX() <= 180 && this.X_data_now != M5.getAngX())
            {
                move();
            }
        }
        else
        {
            Debug.Log("ジャンプ中");
            // ジャンプ後、加速度が落ち着いたらジャンプ終了とみなす
            if (M5.getAccMagnitude() < 1.0)  // ←この値はチューニングして
            {
                Debug.Log("ジャンプ終了");
                jump_flag = false;
            }
        }
    }


    private void move()
    {
        // 値を一つずらす
        for (int i = X_list.Length - 1; i > 0; i--) X_list[i] = X_list[i - 1];
        X_data_befo = X_data_now;

        for (int i = streat_list.Length - 1; i > 0; i--) streat_list[i] = streat_list[i - 1];

        // M5Stickから値の取得
        X_data_now = M5.getAngX();

        // +,-の入れ替わり時の調整
        if (X_data_befo <= 180)
        {
            if (Math.Abs(X_data_now) + Math.Abs(X_data_befo) >= 200)
            {
                if (X_data_befo > 0 && X_data_now < 0) count_over++;
                if (X_data_befo < 0 && X_data_now > 0) count_over--;
            }
        }

        X_list[0] = X_data_now + 360 * count_over;
        //        X_now = X_data_now + 360 * count_over;

        double a = inclination();

        if (Math.Abs(a) < stillness)
        {
            // 変動が小さい時は静止しているとみなす
            Debug.Log("静止");
            streat = 0;

            // どの程度を静止と見なすかは今後調整
        }
        else
        {
            // 前に進んでいるか後ろに戻っているかを判定
            if (a > 0)
            {
                Debug.Log("進");
                streat = 1;

                // 実際に進む処理
                //rb.linearVelocity = Vector3.forward * moveSpeed;
                transform.position += Vector3.forward * moveSpeed * Time.deltaTime * (float)move_ratio;


                animator.Play("アーマチュア|nobiru");




            }
            else
            {
                Debug.Log("戻");
                streat = -1;

                animator.Play("アーマチュア|tijimu");

            }

            streat_list[0] = streat;

            int streat_sum_new = 0;
            int streat_sum_before = 0;
            int streat_new = 0;
            int streat_before = 0;
            for (int i = 0; i < (streat_list.Length / 2); i++) streat_sum_new += streat_list[i];
            for (int i = (streat_list.Length / 2); i < streat_list.Length; i++) streat_sum_before += streat_list[i];

            if (Math.Abs(streat_sum_before) < 2) streat_before = 0;
            else if (streat_sum_before > 0) streat_before = 1;
            else if (streat_sum_before < 0) streat_before = -1;

            if (Math.Abs(streat_sum_new) < 2) streat_new = 0;
            else if (streat_sum_new > 0) streat_new = 1;
            else if (streat_sum_new < 0) streat_new = -1;

            if ((streat_before == 0 || streat_before == -1) && streat_new == 1)
            {
                flont_start_position = X_list[0];
            }
            else if (streat_before == 1 && (streat_new == 0 || streat_new == -1))
            {
                flont_stop_position = X_list[0];
                streat_distance = flont_stop_position - flont_start_position;   // 実際に進んだ距離
                streat_distance *= move_ratio;      // 進む倍率を掛ける
                Debug.Log("前に進んだ距離: " + streat_distance + "度 (倍率：" + move_ratio + ")");
            }

        }

        //        if (Math.Abs(X_now - X_befo1) < 10) {
        //            // 変動が小さい時は静止しているとみなす
        //            Debug.Log("静止");
        //        } else {
        //            if (inclination > 0) {
        //                Debug.Log("進");
        //            } else {
        //                Debug.Log("戻");
        //            }
        //        }
    }

    // X_listを１次式で近似してその傾きを返す
    private double inclination()
    {
        double x2 = 0;
        double xy = 0;

        for (int i = 0; i < X_list.Length; i++)
        {
            x2 += i * i;
            xy += i * (X_list[(X_list.Length - 1) - i] - X_list[X_list.Length - 1]);
        }

        return (xy / x2);
    }



    /**  
     * 前に進んでいるか後ろに戻っているかを取得
     * @return int 1:前に進んでいる, 0:静止している, -1:後ろに戻っている
     */
    public int GetStreat()
    {
        return this.streat;
    }

    public void SetJumpFlag(bool jump_flag)
    {
        this.jump_flag = jump_flag;
    }
}