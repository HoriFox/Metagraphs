using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace nm
{
    public class FreeCamera : MonoBehaviour
    {
        [Range(0f, 100f)]
        public float moveSpeed = 5f;
        [Range(0f, 100f)]
        public float sprintSpeed = 15f;
        [Range(0f, 1f)]
        public float mouseTurnSpeed = 0.5f;
        [Range(0f, 1f)]
        public float smoothness = 0.36f;

        [HideInInspector] public bool m_inputCaptured;
        [HideInInspector] public bool m_rotateAroud;
        float m_yaw, m_pitch, speed, forward, right, up;

        public Vector3 offset;
        public float zoom = 0.2f;
        public float zoomMax = 10;
        public float zoomMin = 3;

        public GameObject menu;
        [HideInInspector] public string selectedObject = null;
        public GameObject rotAObject;
        public GameObject scrollBarRotA;
        public Transform CenterRotMarker;

        private Scrollbar speedRotate;
        private Vector3 targetPosition;
        private Quaternion standartZero = Quaternion.Euler(Vector3.zero);
        private StructureModule structureM;
        private ChangeModule changeM;
        private Quaternion rotation;
        private EditorMenu editorMenu;

        private void Start()
        {
            editorMenu = menu.GetComponent<EditorMenu>();
            structureM = StructureModule.GetInit();
            changeM = ChangeModule.GetInit();
            offset = new Vector3(offset.x, offset.y, Mathf.Abs(zoomMin));

            speedRotate = scrollBarRotA.GetComponent<Scrollbar>();
        }

        public void UpdateMouseSetting()
        {
            mouseTurnSpeed = editorMenu.mouseSensitivity;
            smoothness = editorMenu.smoothingMotion;
        }

        // Вернули управление.
        void CaptureInput()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            m_inputCaptured = true;

            m_yaw = transform.eulerAngles.y;
            m_pitch = transform.eulerAngles.x;
        }

        // Забрали управление.
        void ReleaseInput()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            m_inputCaptured = false;
        }

        void OnApplicationFocus(bool focus)
        {
            if (m_inputCaptured && !focus)
                ReleaseInput();
        }

        public void SetAlignment()
        {
            Vector3 currentRotation = Camera.main.transform.localRotation.eulerAngles;
            Camera.main.transform.localRotation = Quaternion.Euler(Vector3.up);
        }

        void Update()
        {
            if (!m_inputCaptured && !m_rotateAroud)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.collider.transform;

                    if (Input.GetMouseButtonDown(0))
                    {
                        // Если луч указывает на UI элемент, то показания луча недействительны.
                        if (EventSystem.current.IsPointerOverGameObject()) return;
                        // Если элемент существует
                        if (structureM.IsExistNode(objectHit.name))
                        {
                            changeM.ResetChange();
                            selectedObject = objectHit.name;
                            changeM.SetChangeMenu(selectedObject);
                        }
                    }
                }
            }

            if (!editorMenu.menuActive)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (!m_rotateAroud)
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
                }

                // При нажатии показываем маркер центра.
                if (Input.GetKeyDown(EditorMenu.keys[7]))
                {
                    CenterRotMarker.gameObject.SetActive(true);
                }

                // При вращении или удерживании кнопки можно настроить дальность.
                if (m_rotateAroud || Input.GetKey(EditorMenu.keys[7]))
                {
                    CenterRotMarker.position = targetPosition;
                    CenterRotMarker.rotation = standartZero;

                    if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    {
                        offset.z += zoom;
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                    {
                        offset.z -= zoom;
                    }
                    offset.z = Mathf.Clamp(offset.z, Mathf.Abs(zoomMin), float.MaxValue);
                }

                // В момент отпускания мы убираем метку центра.
                if (Input.GetKeyUp(EditorMenu.keys[7]))
                {
                    CenterRotMarker.gameObject.SetActive(false);
                    if (!m_inputCaptured)
                    {
                        m_rotateAroud = (m_rotateAroud) ? false : true;
                        rotAObject.SetActive(m_rotateAroud);
                    }
                }
            }

            //Debug.DrawLine(transform.position, targetPosition);

            if (m_rotateAroud)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * 100 * speedRotate.value);
                transform.position = transform.localRotation * -offset + targetPosition;
            }
            else
            {
                targetPosition = transform.position + transform.localRotation * offset;
            }

            if (!m_inputCaptured)
                return;

            // Вращение камеры
            m_yaw = (m_yaw + mouseTurnSpeed * 10 * Input.GetAxis("Mouse X")) % 360f;
            m_pitch = (m_pitch - mouseTurnSpeed * 10 * Input.GetAxis("Mouse Y")) % 360f;
            rotation = Quaternion.AngleAxis(m_yaw, Vector3.up) * Quaternion.AngleAxis(m_pitch, Vector3.right);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothness);

            // Перемещение камеры
            speed = Time.deltaTime * (Input.GetKey(EditorMenu.keys[6]) ? sprintSpeed : moveSpeed);
            right = speed * ((Input.GetKey(EditorMenu.keys[1]) ? 1f : 0f) - (Input.GetKey(EditorMenu.keys[0]) ? 1f : 0f));
            forward = speed * ((Input.GetKey(EditorMenu.keys[2]) ? 1f : 0f) - (Input.GetKey(EditorMenu.keys[3]) ? 1f : 0f));

            up = speed * ((Input.GetKey(EditorMenu.keys[4]) ? 1f : 0f) - (Input.GetKey(EditorMenu.keys[5]) ? 1f : 0f));
            transform.position += transform.forward * forward + transform.right * right + Vector3.up * up;
        }
    }
}
