using UnityEngine;
using UnityEngine.SceneManagement;

public class EasyCourseIntroduction : MonoBehaviour
{
    private Vector3 start_position;
    private float start_position_x;
    private float start_position_y;

    private Quaternion start_rotation;
    private float start_rotation_x;
    private float start_rotation_y;
    private float start_rotation_z;


    private float force = 0.3f;         // 進む速さ
    private Vector3 streatVec;

    [SerializeField, Tooltip("何秒で前を向くか")]
    private float rotation_time = 5;

    [Header("シーン名")]
    public string sceneName = "nomal";

    private float y_rotation_fin = -170;    // どこまで回転させる

    private double uchiggawa_time = 2;      // 何秒後に内側に移動するか
    private float x_force = 0.3f;           // 内側に移動する時の速さ
    private float x_limit = 1;              // どこまでx座標を動かすか

    private double sageru_time = 3;      // 何秒後にカメラを少し下げるか
    private float y_force = 0.02f;      // カメラを下げる速さ
    private float y_limit = 2;          // どこまでy座標を下げるか

    private double stop_time = 8;      // 何秒後に進むのをやめるか
    private double senter_time = 9;    // 何秒後に正面に振り返るか
    private float kaiten_time = 2;      // 何秒かけて振り返るか
    private float kaiten_limit = 15;    // どこまで振り返るか
    private float sagaru_z = 0.26f;        // 正面に振り返っている間に後ろに下がる量
    private float sagaru_x = 0.04f;

    private double change_time = 12;

    private float y_mawasu;                 // カメラを回す量
    private float y_mawasu_delta;

    private float y_kaiten_syoumenn;        // 正面を向くために回転する量
    private float y_kaiten_syoumenn_delta;

    private double time;


    void Start()
    {
        start_position = this.transform.position;
        start_position_x = start_position.x;
        start_position_y = start_position.y;

        start_rotation = transform.rotation;
        start_rotation_x = start_rotation.x;
        start_rotation_y = -70;
        start_rotation_z = start_rotation.z;

        y_mawasu = y_rotation_fin - start_rotation_y;
        y_kaiten_syoumenn = kaiten_limit - y_rotation_fin;

        time = 0;
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        // 移動
        if (time <= stop_time)
        {
            if (time > uchiggawa_time)
            {
                float x_idou = transform.position.x - x_force;
                if (x_idou < x_limit) x_idou = x_limit;

                if (time > sageru_time)
                {
                    float y_idou = transform.position.y - y_force;
                    if (y_idou < y_limit) y_idou = y_limit;

                    streatVec = new Vector3(x_idou, y_idou, (transform.position.z - force));
                }
                else
                {
                    streatVec = new Vector3(x_idou, start_position_y, (transform.position.z - force));
                }
            }
            else
            {
                streatVec = new Vector3(start_position_x, start_position_y, (transform.position.z - force));
            }

            transform.position = streatVec;     // カメラを移動

            // 向き回転
            if (time <= rotation_time)
            {
                y_mawasu_delta = start_rotation_y + ((y_mawasu / rotation_time) * (float)time);
                transform.rotation = Quaternion.Euler(start_rotation.x, y_mawasu_delta, start_rotation.z);
            }
        }

        if (time > senter_time)
        {
            if (time <= senter_time + kaiten_time)
            {
                float tmp_time = (float)(time - senter_time);

                // 後ろに下がる
                float x_idou = transform.position.x - ((sagaru_x / kaiten_time) * tmp_time);
                float z_idou = transform.position.z - ((sagaru_z / kaiten_time) * tmp_time);
                streatVec = new Vector3(x_idou, transform.position.y, z_idou);
                transform.position = streatVec;


                // 正面を向く
                y_kaiten_syoumenn_delta = y_rotation_fin + ((y_kaiten_syoumenn / kaiten_time) * tmp_time);
                transform.rotation = Quaternion.Euler(start_rotation.x, y_kaiten_syoumenn_delta, start_rotation.z);
            }
        }

        if (time > change_time)
        {
            // シーンを切り替える
            SceneManager.LoadScene(sceneName);
        }

    }
}
