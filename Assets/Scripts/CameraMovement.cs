using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // �J�����̈ړ����x
    public float verticalSpeed = 3f; // �㉺�ړ��̑��x

    void Update()
    {
        // WASD�L�[�̓��͂��擾���đO�㍶�E�Ɉړ�����
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �㉺�ړ��̓��͂��擾����
        float upInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        float downInput = Input.GetKey(KeyCode.LeftShift) ? -1f : 0f;

        // �ړ������ɉ����Ĉʒu���X�V����
        Vector3 moveDirection = new Vector3(horizontalInput, downInput + upInput, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}
