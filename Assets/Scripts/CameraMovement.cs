using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // カメラの移動速度
    public float verticalSpeed = 3f; // 上下移動の速度

    void Update()
    {
        // WASDキーの入力を取得して前後左右に移動する
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 上下移動の入力を取得する
        float upInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        float downInput = Input.GetKey(KeyCode.LeftShift) ? -1f : 0f;

        // 移動方向に応じて位置を更新する
        Vector3 moveDirection = new Vector3(horizontalInput, downInput + upInput, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}
