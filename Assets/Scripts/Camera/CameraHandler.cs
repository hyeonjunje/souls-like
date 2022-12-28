using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    private InputController inputController;
    private Player player;

    public Transform targetTransform;
    public Transform cameraTransform;
    public Transform cameraPivotTransform;
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    public LayerMask ignoreLayers;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public static CameraHandler instance;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float targetPosition;
    private float defaultPosition;
    private float lookAnlge;
    private float pivotAngle;
    public float minimunPivot = -35f;
    public float maximumPivot = 35f;
    public float minimumPivotInLock = -10f;
    public float maximumPivorInLock = 10f;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    public Transform currentLockOnTarget;

    public List<LivingEntity> availableTargets = new List<LivingEntity>();
    public Transform nearestLockOnTarget;
    public Transform leftLockTarget;
    public Transform rightLockTarget;
    public float maximumLockOnDistance = 30f;

    public Transform lockOnMark;
    private Transform currentTargetHpCanvas = null;


    private void Awake()
    {
        instance = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;

        targetTransform = FindObjectOfType<PlayerController>().transform;
        inputController = FindObjectOfType<InputController>();
        player = FindObjectOfType<Player>();
    }


    public void FollowTarget(float delta)
    {
        /// Recommend using SmoothDamp instead of Lerp. Looks much more natural as it's factoring in velocity
        Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity,  delta * followSpeed);
        myTransform.position = targetPosition;

        HandleCameraCollisions(delta);
    }


    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        // 록온상태가 아닐 때
        if (inputController.lockOnFlag == false && currentLockOnTarget == null)
        {
            lookAnlge = transform.eulerAngles.y;
            pivotAngle = cameraPivotTransform.localEulerAngles.x;
            if (pivotAngle > 180)
                pivotAngle -= 360;

            lookAnlge += (mouseXInput * lookSpeed) * delta;
            pivotAngle -= (mouseYInput * pivotSpeed) * delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimunPivot, maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAnlge;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
        // 록온 상태일 때
        else
        {
            float velocity = 0;

            Vector3 dir = currentLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;

            dir = currentLockOnTarget.position - cameraPivotTransform.position;
            dir.Normalize();

            targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;

            if (eulerAngle.x > 180f)
                eulerAngle.x -= 360f;

            eulerAngle.x = Mathf.Clamp(eulerAngle.x, minimumPivotInLock, maximumPivorInLock);
            eulerAngle.y = 0;

            cameraPivotTransform.localEulerAngles = eulerAngle;

            lockOnMark.LookAt(cameraTransform);
            if (currentTargetHpCanvas != null)
                currentTargetHpCanvas.LookAt(cameraTransform);
        }
    }


    private void HandleCameraCollisions(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffset);
        }

        if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        cameraTransformPosition.z = targetPosition;
        cameraTransform.localPosition = cameraTransformPosition;
    }

    public void HandleLockOn()
    {
        availableTargets = new List<LivingEntity>();

        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

        for(int i = 0; i < colliders.Length; i++)
        {
            LivingEntity character = colliders[i].GetComponent<LivingEntity>();

            if(character != null && character.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                RaycastHit hit;

                if(character.transform.root.gameObject.layer != LayerMask.NameToLayer("Player") 
                    && viewableAngle > -50 && viewableAngle < 50 
                    && distanceFromTarget <= maximumLockOnDistance)
                {
                    if(Physics.Linecast(character.lockOnTransform.position, player.lockOnTransform.position, out hit))
                    {
                        if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {

                        }
                        else
                            availableTargets.Add(character);
                    }
                }
            }
        }

        for (int k = 0; k < availableTargets.Count; k++)
        {
            float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

            if(distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = availableTargets[k].lockOnTransform;
            }

            if(inputController.lockOnFlag)
            {
                Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position);
                if (relativeEnemyPosition.x > 0f && Mathf.Abs(relativeEnemyPosition.x) < shortestDistanceOfLeftTarget)
                {
                    shortestDistanceOfLeftTarget = Mathf.Abs(relativeEnemyPosition.x);
                    leftLockTarget = availableTargets[k].lockOnTransform;
                }

                if(relativeEnemyPosition.x < 0f && Mathf.Abs(relativeEnemyPosition.x) < shortestDistanceOfRightTarget)
                {
                    shortestDistanceOfRightTarget = Mathf.Abs(relativeEnemyPosition.x);
                    rightLockTarget = availableTargets[k].lockOnTransform;
                }
            }
        }
    }


    public void StartLockOn()
    {
        lockOnMark.gameObject.SetActive(true);

        cameraPivotTransform.position += Vector3.up * 0.5f;

        Debug.Log(currentLockOnTarget.name);

        lockOnMark.SetParent(currentLockOnTarget, false);

        Enemy enemy = currentLockOnTarget.GetComponentInParent<Enemy>();
        if(enemy.enemyType == Define.EEnemyType.Common)
        {
            enemy.hpCanvas.SetActive(true);
            currentTargetHpCanvas = enemy.hpCanvas.transform;
        }
    }


    public void EndLockOn()
    {
        lockOnMark.gameObject.SetActive(false);
        lockOnMark.SetParent(myTransform, false);

        cameraPivotTransform.position += Vector3.down * 0.5f;

        Enemy enemy = currentLockOnTarget.GetComponentInParent<Enemy>();
        if (enemy.enemyType == Define.EEnemyType.Common)
        {
            enemy.hpCanvas.SetActive(false);
            currentTargetHpCanvas = null;
        }
    }


    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }
}
