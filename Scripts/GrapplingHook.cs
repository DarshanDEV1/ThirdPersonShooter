using UnityEngine;
using System.Collections;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public enum State
{
    Normal,
    Hookshot,
    HookShotThrown
}

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform debugPoint;
    [SerializeField] State state;
    [SerializeField] CharacterController characterController;
    [SerializeField] Vector3 hookShotPosition;
    [SerializeField] float hookCoolDown = 5f;
    [SerializeField] bool hookCool = true;
    [SerializeField] Image ColorFill;
    [SerializeField] Transform hookShotTransform;
    [SerializeField] float hookSize;

    private void Awake()
    {
        state = State.Normal;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        hookShotTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCameraStatus();
        HookCoolDownTimer();

        switch (state)
        {
            default:
            case State.Normal:
                HookShot();
                break;
            case State.HookShotThrown:
                HandleHookShot();
                break;
            case State.Hookshot:
                HookMovement();
                break;
        }
    }

    private void HookShot()
    {
        if (Input.GetKeyDown(KeyCode.E) && hookCool)
        {
            Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit raycastHit, 80f);
            //HIT
            debugPoint.position = raycastHit.point;
            hookShotPosition = raycastHit.point;
            Debug.DrawRay(cam.transform.position, hookShotPosition, Color.yellow);
            hookSize = 0f;
            hookShotTransform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
            hookShotTransform.gameObject.SetActive(true);

            state = State.HookShotThrown;

            hookCoolDown = 0;
            hookCool = false;
            ColorFill.fillAmount = hookCoolDown;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            state = State.Normal;
        }
    }

    private void HookMovement()
    {
        Vector3 hook = (hookShotPosition - transform.position).normalized;
        float speed = Vector3.Distance(transform.position, hookShotPosition);
        float speedMultiplier = 2f;

        characterController.Move(hook * speed * speedMultiplier * Time.deltaTime);

        float reachedDistance = 10f;
        if (Vector3.Distance(hookShotPosition, transform.position) < reachedDistance)
        {
            //Do Something
            characterController.Move(Vector3.up.normalized * Time.deltaTime);
            hookShotTransform.gameObject.SetActive(false);
            state = State.Normal;
        }
    }

    private void HookCoolDownTimer()
    {
        if(!hookCool)
        {
            hookCoolDown += Time.deltaTime;
            ColorFill.fillAmount = hookCoolDown / 5;
            if(hookCoolDown >= 5)
            {
                hookCool = true;
            }
        }
    }

    private void HandleHookShot()
    {
        hookShotTransform.LookAt(hookShotPosition);

        float throwSpeed = Vector3.Distance(transform.position, hookShotPosition);
        float speedMultiplier = 2f;
        hookSize += throwSpeed * speedMultiplier * Time.deltaTime;
        hookShotTransform.localScale = new Vector3(0.2f, 0.2f, hookSize);

        if(hookSize >= Vector3.Distance(transform.position, hookShotPosition))
        {
            state = State.Hookshot;
        }
    }

    private void UpdateCameraStatus()
    {
        cam.transform.rotation = Camera.main.transform.rotation;
    }
}