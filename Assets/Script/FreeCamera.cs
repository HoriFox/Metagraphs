﻿using UnityEngine;

public class FreeCamera : MonoBehaviour {
    //public bool enableInputCapture = true;
    //public bool holdRightMouseCapture = false;

    public GameObject croshair;

    public float mouseTurnSpeed = 5f;
	public float moveSpeed = 5f;
	public float sprintSpeed = 15f;

	bool	m_inputCaptured;
	float	m_yaw, m_pitch, rotStrafe, rotFwd, speed, forward, right, up;

    //void Awake() {
    //	enabled = enableInputCapture;
    //}

    //void OnValidate() {
    //	if(Application.isPlaying)
    //		enabled = enableInputCapture;
    //}

    void CaptureInput() {
        croshair.gameObject.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

		m_inputCaptured = true;

		m_yaw = transform.eulerAngles.y;
		m_pitch = transform.eulerAngles.x;
	}

	void ReleaseInput() {
        croshair.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

		m_inputCaptured = false;
	}

	void OnApplicationFocus(bool focus) {
		if(m_inputCaptured && !focus)
			ReleaseInput();
	}

	void Update() {
        if (Input.GetMouseButtonDown(1))
        {
            if (!m_inputCaptured)
            {
                CaptureInput();
            }
            else if (m_inputCaptured)
            {
                ReleaseInput();
            }
		}

		if(!m_inputCaptured)
			return;

		rotStrafe = Input.GetAxis("Mouse X");
		rotFwd = Input.GetAxis("Mouse Y");

        // Вращение камеры
		m_yaw = (m_yaw + mouseTurnSpeed * rotStrafe) % 360f;
		m_pitch = (m_pitch - mouseTurnSpeed * rotFwd) % 360f;
		transform.rotation = Quaternion.AngleAxis(m_yaw, Vector3.up) * Quaternion.AngleAxis(m_pitch, Vector3.right);

        // Перемещение камеры
        float speed = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);
        float forward = speed * Input.GetAxis("Vertical");
        float right = speed * Input.GetAxis("Horizontal");
        float up = speed * ((Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f));
		transform.position += transform.forward * forward + transform.right * right + Vector3.up * up;
	}
}
