//using UnityEngine;
//using System;
//using System.Diagnostics;
//using UnityEngine.Windows;
//using UnityEngine.UI; // ← 追加

//public class PlayerMoveFromSensor : MonoBehaviour
//{
//    Rigidbody rb;
//    [SerializeField, Tooltip("加える力")]
//    private float force = 10f;

//    ForceMode mode = ForceMode.Acceleration;

//    [SerializeField, Tooltip("止まったとき、毎秒どれだけ速度を落とすか")]
//    private float brake = 12f;

//    private bool move_flag;


//    [SerializeField, Tooltip("進む時の基本量")]
//    private float move_distance;

//    [SerializeField, Tooltip("進む時にかける倍率")]
//    public static float move_diameter = 1.0f;

//    //[SerializeField, Tooltip("障害物に当たった時にかける倍率")]
//    private float collision_diameter;
//    private bool collision_flag;
//    private double collision_start_time;
//    private double debuff_time;

//    private float distance_value;

//    private float[] distance_value_history;
//    private const int DISTANCE_VALUE_HOW = 20;  // 距離センサーからの値を記憶しておく個数
//    private bool is_distance_array_full;

//    [SerializeField, Tooltip("動いていないとみなす閾値")]
//    private float immovable_th = 10;

//    private float start_distance;       // 進め始めた時の地点
//    [SerializeField, Tooltip("基準となる開始地点の距離センサーの値")]
//    private float start_line_distance;  // 基準となる開始地点の距離
//    private bool start_line_distance_isSet;

//    // playerの状態（-1:戻, 0：止, 1：進）
//    private int situation;
//    private int situation_before;

//    [SerializeField, Tooltip("スタートラインから、最低どのくらい引くと溜め判定にするか")]
//    private int charge_th = 20;

//    private int charge_count;

//    // ジャンプ関係
//    private bool jamp_flag;     // ジャンプ中かどうか

//    private Vector3 jamp_start_pos;

//    private float t;   // 時刻
//    [SerializeField, Tooltip("ジャンプする時の角度[度]")]
//    private int angle_degree;   // なす角[°]
//    private double angle_rad;   // なす角[rad]
//    private float cos;
//    private float sin;
//    private float v0;   // 初速度（溜めた量）
//    private float a;    // 加速度（溜め開放後に伸ばした量）
//    private const float g = (float)9.8;   // 重力加速度

//    [SerializeField, Tooltip("初速度調整用の値")]
//    private float v0_adjustment;
//    [SerializeField, Tooltip("加速度調整用の値")]
//    private float a_adjustment;

//    [SerializeField, Tooltip("チャージカウントの最大値")]
//    private int charge_count_max;

//    [SerializeField, Tooltip("キャラの最大速度")]
//    private float maxSpeed = 5f; // ← インスペクタから調整可能にする

//    [SerializeField, Tooltip("チャージゲージ UI")]
//    private Slider chargeSlider; // ← インスペクタから割り当てる

//    // チュートリアル用のフラグ、チャージとジャンプ
//    public static bool Charged = false;
//    public static bool Jumped = false;


//    // パーティクルシステムへの参照
//    public ParticleSystem windParticles;

//    // 風の効果音
//    public AudioSource windAudioSource; // AudioSourceコンポーネントへの参照
//    public AudioClip windSoundEffect;   // 再生したいオーディオクリップ

//    // ジャンプの効果音
//    public AudioSource jumpAudioSource; // AudioSourceコンポーネントへの参照
//    public AudioClip jumpSoundEffect;   // 再生したいオーディオクリップ

//    // 溜めゲージの効果音
//    public AudioSource chargeAudioSource; // AudioSourceコンポーネントへの参照
//    public AudioClip chargeSoundEffect;   // 再生したいオーディオクリップ


//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//    }

//    void Start()
//    {
//        // ゲーム開始時にパーティクルを停止
//        if (windParticles != null)
//        {
//            windParticles.Stop();
//        }
//        distance_value_history = new float[DISTANCE_VALUE_HOW];
//        for (int i = 0; i < DISTANCE_VALUE_HOW; i++) distance_value_history[i] = -1;

//        collision_diameter = 1;

//        // スタートラインの基準値を指定した値にするか、スタート時の平均値にするか
//        if (start_line_distance != null && start_line_distance >= 0)
//        {
//            start_line_distance_isSet = true;
//        }
//        else
//        {
//            start_line_distance_isSet = false;
//        }

//        is_distance_array_full = false;
//        start_distance = 0;

//        situation = 0;
//        situation_before = 0;

//        charge_count = 0;

//        jamp_flag = false;
//        collision_flag = false;
//        move_flag = false;

//        // ゲージの最大値をチャージの最大に合わせる
//        if (chargeSlider != null)
//        {
//            chargeSlider.maxValue = charge_count_max;
//            chargeSlider.value = 0;
//        }

//        Charged = false;
//        Jumped = false;
//    }


//    void Update()
//    {
//        // 距離センサーからの値を取得
//        distance_value = DistanceSensorReader1.distance;



//        // `distance_value_history`の値を１つずらして最新の値をセットする
//        for (int i = DISTANCE_VALUE_HOW - 1; i > 0; i--) distance_value_history[i] = distance_value_history[i - 1];
//        distance_value_history[0] = distance_value;

//        if (!is_distance_array_full)
//        {
//            if (distance_value_history[DISTANCE_VALUE_HOW - 1] > 0)
//            {
//                is_distance_array_full = true;  // 配列の中身が全て埋まった

//                // スタートの基準値が指定値で指定されていない
//                if (!start_line_distance_isSet)
//                {
//                    // スタート位置を決定する
//                    for (int i = 0; i < DISTANCE_VALUE_HOW; i++) start_line_distance += distance_value_history[i];
//                    start_line_distance /= DISTANCE_VALUE_HOW;

//                    start_distance = start_line_distance;
//                }
//            }
//        }
//        else
//        {
//            if (jamp_flag)  // ジャンプ中
//            {
//                //UnityEngine.Debug.Log("ジャンプフラグON");
//                t += (float)Time.deltaTime;

//                float x = (v0 * cos) * t;
//                float y = ((v0 * sin) * t) - (g * (t * t) / 2);


//                Vector3 new_pos = new Vector3(jamp_start_pos.x, jamp_start_pos.y + y, jamp_start_pos.z + x);

//                // ジャンプ前の高さに戻ってきたらジャンプ終了
//                if (jamp_start_pos.y + y < jamp_start_pos.y)
//                {
//                    new_pos = new Vector3(jamp_start_pos.x, jamp_start_pos.y, jamp_start_pos.z + x);
//                    jamp_flag = false;
//                }

//                this.transform.position = new_pos;
//                //UnityEngine.Debug.Log("ジャンプしたよ");
//            }
//            else
//            {
//                // ↓ ジャンプ中じゃない

//                situation_before = situation;

//                // 溜め判定
//                if (distance_value > start_line_distance + charge_th)
//                {
//                    if (charge_count >= charge_count_max)
//                    {
//                        charge_count = charge_count_max;
//                    }
//                    else
//                    {
//                        charge_count++;
//                        // 毎フレームゲージを更新
//                        if (chargeSlider != null)
//                        {
//                            chargeSlider.value = charge_count;
//                        }

//                        // チュートリアル用のフラグ関数
//                        SomeChargeDetectMethod();
//                        // 効果音を再生
//                        if (chargeAudioSource != null && chargeSoundEffect != null && !chargeAudioSource.isPlaying)
//                        {
//                            chargeAudioSource.PlayOneShot(chargeSoundEffect);
//                        }
//                    }
//                    //UnityEngine.Debug.Log("ため開始");
//                    //UnityEngine.Debug.Log("charge_count: " + charge_count);
//                    // 停止しているとき、停止
//                    windParticles.Stop();
//                }
//                else
//                {
//                    // ↓ 溜め中じゃない
//                    double inclination_value = inclination();

//                    if (Math.Abs(inclination_value) < immovable_th)
//                    {
//                        // 停止しているとき、停止
//                        windParticles.Stop();
//                        // 動いていない
//                        situation = 0;
//                        move_flag = false;
//                    }
//                    else
//                    {
//                        if (inclination_value < 0)
//                        {
//                            // 進んでいる
//                            situation = 1;

//                            // 溜め開放時以外は、そのまま動かす
//                            if (charge_count <= 0)
//                            {
//                                move_flag = true;

//                                //move();
//                            }
//                        }
//                        else
//                        {
//                            // 停止しているとき、停止
//                            windParticles.Stop();
//                            // 戻っている
//                            situation = -1;
//                            move_flag = false;
//                        }
//                    }

//                    // 進→(止or戻)
//                    if (situation_before == 1 && situation <= 0)
//                    {
//                        if (charge_count > 0)
//                        {
//                            jamp();
//                            //UnityEngine.Debug.Log("jumpスレッド呼び出し");
//                        }
//                    }

//                    // (止or戻)->進
//                    if (situation_before <= 0 && situation == 1)
//                    {
//                        start_distance = distance_value;
//                    }
//                }
//            }

//            if (collision_flag)
//            {
//                if (Time.time > collision_start_time + debuff_time)
//                {
//                    collision_diameter = 1;
//                    collision_flag = false;
//                }
//            }
//        }


//    }

//    private void FixedUpdate()
//    {
//        if (move_flag)
//        {
//            // 動いているとき、再生
//            windParticles.Play();
//            if (windAudioSource != null && windSoundEffect != null && !windAudioSource.isPlaying)
//            {
//                windAudioSource.PlayOneShot(windSoundEffect);
//            }
//            //UnityEngine.Debug.Log("風吹く");
//            Vector3 dir = transform.forward; // ← 常にキャラの向きで進む
//            rb.AddForce(dir * force * move_diameter * collision_diameter, mode);
//            //UnityEngine.Debug.Log("進む");
//        }
//        else
//        {
//            var v = rb.linearVelocity;   // ← linearVelocity じゃなく velocity
//            // 例: Z軸だけ素早く減衰（Yは重力を維持）
//            v.z = Mathf.MoveTowards(v.z, 0f, brake * Time.fixedDeltaTime);
//            rb.linearVelocity = v;
//        }

//        // ★ ここで速度制限
//        if (rb.linearVelocity.magnitude > maxSpeed)
//        {
//            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
//        }
//    }




//    private double inclination()
//    {
//        double x2 = 0;
//        double xy = 0;

//        for (int i = 0; i < DISTANCE_VALUE_HOW; i++)
//        {
//            x2 += i * i;
//            xy += i * (distance_value_history[(DISTANCE_VALUE_HOW - 1) - i] - distance_value_history[DISTANCE_VALUE_HOW - 1]);
//        }

//        return (xy / x2);
//    }



//    private void move()
//    {
//        float moved_value = move_distance * move_diameter * collision_diameter;

//        Vector3 now_pos = this.transform.position;
//        Vector3 new_pos = new Vector3(now_pos.x, now_pos.y, now_pos.z + moved_value);

//        this.transform.position = new_pos;
//    }

//    private void jamp()
//    {
//        tutorialJamp();
//        // 動いているとき、再生
//        windParticles.Play();
//        jamp_flag = true;


//        t = 0;
//        angle_rad = angle_degree * Math.PI / 180.0;
//        cos = (float)Math.Cos(angle_rad);
//        sin = (float)Math.Sin(angle_rad);
//        v0 = (charge_count * v0_adjustment) + ((start_distance - distance_value) * a_adjustment);
//        a = (start_distance - distance_value) * a_adjustment;

//        jamp_start_pos = this.transform.position;

//        // 音楽を止める
//        chargeAudioSource.Stop();

//        // 効果音を再生
//        if (jumpAudioSource != null && jumpSoundEffect != null)
//        {
//            jumpAudioSource.PlayOneShot(jumpSoundEffect);
//        }

//        // `charge_count` をリセット
//        charge_count = 0;
//        chargeSlider.value = 0;

//        Charged = false;


//    }


//    public void collision(double time, double slow)
//    {
//        collision_start_time = Time.time;
//        collision_diameter = (float)slow;
//        debuff_time = time;
//        collision_flag = true;
//        //UnityEngine.Debug.Log("遅くなる");
//    }

//    // チュートリアルでのチャージのフラグ
//    private void SomeChargeDetectMethod()
//    {
//        Charged = true;
//    }

//    // チュートリアルでのジャンプのフラグ
//    private void tutorialJamp()
//    {
//        // 既存ジャンプ処理
//        Jumped = true;
//    }
//}
















using UnityEngine;
using System;
using System.Diagnostics;
using UnityEngine.Windows;
using UnityEngine.UI; // ← 追加

public class PlayerMoveFromSensor : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField, Tooltip("加える力")]
    private float force = 10f;

    ForceMode mode = ForceMode.Acceleration;

    [SerializeField, Tooltip("止まったとき、毎秒どれだけ速度を落とすか")]
    private float brake = 12f;

    private bool move_flag;


    [SerializeField, Tooltip("進む時の基本量")]
    private float move_distance;

    [SerializeField, Tooltip("進む時にかける倍率")]
    public static float move_diameter = 1.0f;

    //[SerializeField, Tooltip("障害物に当たった時にかける倍率")]
    private float collision_diameter;
    private bool collision_flag;
    private double collision_start_time;
    private double debuff_time;

    private float distance_value;

    private float[] distance_value_history;
    private const int DISTANCE_VALUE_HOW = 20;  // 距離センサーからの値を記憶しておく個数
    private bool is_distance_array_full;

    [SerializeField, Tooltip("動いていないとみなす閾値")]
    private float immovable_th = 10;

    private float start_distance;       // 進め始めた時の地点
    [SerializeField, Tooltip("基準となる開始地点の距離センサーの値")]
    private float start_line_distance;  // 基準となる開始地点の距離
    private bool start_line_distance_isSet;

    // playerの状態（-1:戻, 0：止, 1：進）
    private int situation;
    private int situation_before;

    [SerializeField, Tooltip("スタートラインから、最低どのくらい引くと溜め判定にするか")]
    private int charge_th = 20;

    private int charge_count;

    // ジャンプ関係
    private bool jamp_flag;     // ジャンプ中かどうか

    private Vector3 jamp_start_pos;

    private float t;   // 時刻
    private float v_y;
    private float v_z;

    private float a;    // 加速度（溜め開放後に伸ばした量）
    private const float g = (float)9.8;   // 重力加速度

    [SerializeField, Tooltip("溜めた量にかける値")]
    private float v0_adjustment;
    [SerializeField, Tooltip("伸ばした量にかける値")]
    private float a_adjustment;

    [SerializeField, Tooltip("チャージカウントの最大値")]
    private int charge_count_max;

    [SerializeField, Tooltip("キャラの最大速度")]
    private float maxSpeed = 5f; // ← インスペクタから調整可能にする

    [SerializeField, Tooltip("チャージゲージ UI")]
    private Slider chargeSlider; // ← インスペクタから割り当てる

    // チュートリアル用のフラグ、チャージとジャンプ
    public static bool Charged = false;
    public static bool Jumped = false;


    // パーティクルシステムへの参照
    //public ParticleSystem windParticles;

    // 風の効果音
    public AudioSource windAudioSource; // AudioSourceコンポーネントへの参照
    public AudioClip windSoundEffect;   // 再生したいオーディオクリップ

    // ジャンプの効果音
    public AudioSource jumpAudioSource; // AudioSourceコンポーネントへの参照
    public AudioClip jumpSoundEffect;   // 再生したいオーディオクリップ

    // 溜めゲージの効果音
    public AudioSource chargeAudioSource; // AudioSourceコンポーネントへの参照
    public AudioClip chargeSoundEffect;   // 再生したいオーディオクリップ


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 物理演算による回転を固定
        rb.freezeRotation = true;
    }

    void Start()
    {
        // ゲーム開始時にパーティクルを停止
        //if (windParticles != null)
        //{
        //    windParticles.Stop();
        //}
        distance_value_history = new float[DISTANCE_VALUE_HOW];
        for (int i = 0; i < DISTANCE_VALUE_HOW; i++) distance_value_history[i] = -1;

        collision_diameter = 1;

        // スタートラインの基準値を指定した値にするか、スタート時の平均値にするか
        if (start_line_distance != null && start_line_distance >= 0)
        {
            start_line_distance_isSet = true;
        }
        else
        {
            start_line_distance_isSet = false;
        }

        is_distance_array_full = false;
        start_distance = 0;
        if (!start_line_distance_isSet) start_line_distance = 0;


        situation = 0;
        situation_before = 0;

        charge_count = 0;

        jamp_flag = false;
        collision_flag = false;
        move_flag = false;

        // ゲージの最大値をチャージの最大に合わせる
        if (chargeSlider != null)
        {
            chargeSlider.maxValue = charge_count_max;
            chargeSlider.value = 0;
        }

        Charged = false;
        Jumped = false;
    }


    void Update()
    {
        // 距離センサーからの値を取得
        distance_value = DistanceSensorReader1.distance;



        // `distance_value_history`の値を１つずらして最新の値をセットする
        for (int i = DISTANCE_VALUE_HOW - 1; i > 0; i--) distance_value_history[i] = distance_value_history[i - 1];
        distance_value_history[0] = distance_value;

        if (!is_distance_array_full)
        {
            if (distance_value_history[DISTANCE_VALUE_HOW - 1] > 0)
            {
                is_distance_array_full = true;  // 配列の中身が全て埋まった

                // スタートの基準値が指定値で指定されていない
                if (!start_line_distance_isSet)
                {
                    // スタート位置を決定する
                    for (int i = 0; i < DISTANCE_VALUE_HOW; i++) start_line_distance += distance_value_history[i];
                    start_line_distance /= DISTANCE_VALUE_HOW;

                    start_distance = start_line_distance;
                }
            }
        }
        else
        {
            if (jamp_flag)  // ジャンプ中
            {
                //UnityEngine.Debug.Log("ジャンプフラグON");
                t += (float)Time.deltaTime;

                //float x = (v0 * cos) * t;
                // float y = ((v0 * sin) * t) - (g * (t * t) / 2);
                float z = v_z * t;
                float y = (v_y * t) - (g * (t * t) / 2);


                Vector3 new_pos = new Vector3(jamp_start_pos.x, jamp_start_pos.y + y, jamp_start_pos.z + z);

                // ジャンプ前の高さに戻ってきたらジャンプ終了
                if (jamp_start_pos.y + y < jamp_start_pos.y)
                {
                    new_pos = new Vector3(jamp_start_pos.x, jamp_start_pos.y, jamp_start_pos.z + z);
                    jamp_flag = false;
                }

                this.transform.position = new_pos;
                //UnityEngine.Debug.Log("ジャンプしたよ");
            }
            else
            {
                // ↓ ジャンプ中じゃない

                situation_before = situation;

                // 溜め判定
                if (distance_value > start_line_distance + charge_th)
                {
                    if (charge_count >= charge_count_max)
                    {
                        charge_count = charge_count_max;
                    }
                    else
                    {
                        charge_count++;
                        // 毎フレームゲージを更新
                        if (chargeSlider != null)
                        {
                            chargeSlider.value = charge_count;
                        }

                        // チュートリアル用のフラグ関数
                        SomeChargeDetectMethod();
                        // 効果音を再生
                        if (chargeAudioSource != null && chargeSoundEffect != null && !chargeAudioSource.isPlaying)
                        {
                            chargeAudioSource.PlayOneShot(chargeSoundEffect);
                        }
                    }
                    //UnityEngine.Debug.Log("ため開始");
                    //UnityEngine.Debug.Log("charge_count: " + charge_count);
                    // 停止しているとき、停止
                    //windParticles.Stop();
                }
                else
                {
                    // ↓ 溜め中じゃない
                    double inclination_value = inclination();

                    if (Math.Abs(inclination_value) < immovable_th)
                    {
                        // 停止しているとき、停止
                        //windParticles.Stop();
                        // 動いていない
                        situation = 0;
                        move_flag = false;
                    }
                    else
                    {
                        if (inclination_value < 0)
                        {
                            // 進んでいる
                            situation = 1;

                            // 溜め開放時以外は、そのまま動かす
                            if (charge_count <= 0)
                            {
                                move_flag = true;

                                //move();
                            }
                        }
                        else
                        {
                            // 停止しているとき、停止
                            //windParticles.Stop();
                            // 戻っている
                            situation = -1;
                            move_flag = false;
                        }
                    }

                    // 進→(止or戻)
                    if (situation_before == 1 && situation <= 0)
                    {
                        if (charge_count > 0)
                        {
                            jamp();
                            //UnityEngine.Debug.Log("jumpスレッド呼び出し");
                        }
                    }

                    // (止or戻)->進
                    if (situation_before <= 0 && situation == 1)
                    {
                        start_distance = distance_value;
                    }
                }
            }

            if (collision_flag)
            {
                if (Time.time > collision_start_time + debuff_time)
                {
                    collision_diameter = 1;
                    collision_flag = false;
                }
            }
        }


    }

    private void FixedUpdate()
    {
        if (move_flag)
        {
            // 動いているとき、再生
            //windParticles.Play();
            if (windAudioSource != null && windSoundEffect != null && !windAudioSource.isPlaying)
            {
                windAudioSource.PlayOneShot(windSoundEffect);
            }
            //UnityEngine.Debug.Log("風吹く");
            Vector3 dir = transform.forward; // ← 常にキャラの向きで進む
            rb.AddForce(dir * force * move_diameter * collision_diameter, mode);
            //UnityEngine.Debug.Log("進む");
        }
        else
        {
            var v = rb.linearVelocity;   // ← linearVelocity じゃなく velocity
            // 例: Z軸だけ素早く減衰（Yは重力を維持）
            v.z = Mathf.MoveTowards(v.z, 0f, brake * Time.fixedDeltaTime);
            rb.linearVelocity = v;
        }

        // ★ ここで速度制限
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }




    private double inclination()
    {
        double x2 = 0;
        double xy = 0;

        for (int i = 0; i < DISTANCE_VALUE_HOW; i++)
        {
            x2 += i * i;
            xy += i * (distance_value_history[(DISTANCE_VALUE_HOW - 1) - i] - distance_value_history[DISTANCE_VALUE_HOW - 1]);
        }

        return (xy / x2);
    }



    private void move()
    {
        float moved_value = move_distance * move_diameter * collision_diameter;

        Vector3 now_pos = this.transform.position;
        Vector3 new_pos = new Vector3(now_pos.x, now_pos.y, now_pos.z + moved_value);

        this.transform.position = new_pos;
    }

    private void jamp()
    {
        tutorialJamp();
        // 動いているとき、再生
        //windParticles.Play();
        jamp_flag = true;


        t = 0;
        v_y = charge_count * v0_adjustment;
        //v_z = (start_distance - distance_value) * a_adjustment;
        v_z = Math.Abs(start_distance - distance_value) * a_adjustment;

        jamp_start_pos = this.transform.position;

        // 音楽を止める
        chargeAudioSource.Stop();

        // 効果音を再生
        if (jumpAudioSource != null && jumpSoundEffect != null)
        {
            jumpAudioSource.PlayOneShot(jumpSoundEffect);
        }

        // `charge_count` をリセット
        charge_count = 0;
        chargeSlider.value = 0;

        Charged = false;


    }


    public void collision(double time, double slow)
    {
        collision_start_time = Time.time;
        collision_diameter = (float)slow;
        debuff_time = time;
        collision_flag = true;
        //UnityEngine.Debug.Log("遅くなる");
    }

    // チュートリアルでのチャージのフラグ
    private void SomeChargeDetectMethod()
    {
        Charged = true;
    }

    // チュートリアルでのジャンプのフラグ
    private void tutorialJamp()
    {
        // 既存ジャンプ処理
        Jumped = true;
    }
}
