using System;
using System.Threading.Tasks;
using UnityEngine;

public class jumpSample : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private moveSample moveSample;
    private bool isJumping = false;

    // ジャンプの設定値
    public float jumpPowerY = 4f;
    public float jumpPowerZ = 2f;
    public float gravity = 9.8f;


    private Vector3 velocity;

    void Start()
    {
        // 初期化処理
        isJumping = false;
    }



    void Update()
    {

        if (isJumping)
        {
            // 斜め上前に移動
            transform.position += velocity * Time.deltaTime;

            // 重力を加える
            velocity.y -= gravity * Time.deltaTime;

            // 地面に到達（Y <= 0）で着地
            if (transform.position.y <= 0f)
            {
                Vector3 pos = transform.position;
                pos.y = 0f;
                transform.position = pos;

                isJumping = false;
                moveSample.SetJumpFlag(false); // ジャンプ終了を通知
            }
        }
    }

    public async void StartJump()
    {
        //if (this.isJumping)
        //{
        //    // 既にジャンプ中の場合は何もしない
        //    //return false;
        //}

        this.isJumping = true;

        // ジャンプ開始前に少し待機（２秒）
        await Task.Delay(2000);





        this.isJumping = false;
        moveSample.SetJumpFlag(false);

        //return true;
    }



    public bool GetIsJumping()
    {
        return this.isJumping;
    }
}
