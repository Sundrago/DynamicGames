using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shoot_joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] GameObject joystickUI, joysyick_knob, targetObj;
    [SerializeField] Boundaries boundaries;
    [SerializeField] Shoot_Bullet_Manager bullet_Manager;
    [SerializeField] ParticleSystem ThrustFx;
    [SerializeField] Shoot_GameManager gameManager;

    [SerializeField] float max_radius = 1f;
    [SerializeField] float clamp_max_velocity = 75f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float joystick_inensity = 1;

    [SerializeField] float friction = 0.98f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float velocity = 0.5f;

    public Vector3 vecNormal;
    private Vector3 speed = Vector3.zero;
    private Vector2 initialPoint;

    private ParticleSystem.ShapeModule shape;
    private ParticleSystem.EmissionModule emmision;
    private bool onDrag = false;

    private void Start()
    {
        joystickUI.SetActive(false);
        Application.targetFrameRate = 60;
        shape = ThrustFx.shape;
        emmision = ThrustFx.emission;

        emmision.rateOverTimeMultiplier = 0;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        onDrag = true;
        initialPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        joystickUI.transform.position = initialPoint; // = new Vector2(initialPoint.x, initialPoint.y + joystickUI.GetComponent<RectTransform>().transform.localScale.y / 2f);
        joystickUI.SetActive(true);
        emmision.rateOverTimeMultiplier = 60;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 dragPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        UpdatePointer(dragPoint);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        onDrag = false;
        joystickUI.SetActive(false);
        joysyick_knob.transform.localPosition = Vector2.zero;
        emmision.rateOverTimeMultiplier = 0;
    }

    void LateUpdate()
    {
        if(onDrag) {
            speed += vecNormal * velocity * joystick_inensity;
            shape.position = targetObj.transform.position;
            shape.rotation = new Vector3(targetObj.transform.eulerAngles.z,0f,0f);
        }
            
        speed *= friction;
        speed = new Vector3(Mathf.Clamp(speed.x, -maxSpeed, maxSpeed), Mathf.Clamp(speed.y, -maxSpeed, maxSpeed), 0);
        targetObj.transform.position = targetObj.transform.position + (speed * Time.deltaTime);

        if(targetObj.transform.position.x < boundaries.left.position.x) {
            targetObj.transform.position = new Vector3(boundaries.left.position.x, targetObj.transform.position.y, targetObj.transform.position.z);
        } else if(targetObj.transform.position.x > boundaries.right.position.x) {
            targetObj.transform.position = new Vector3(boundaries.right.position.x, targetObj.transform.position.y, targetObj.transform.position.z);
        } 
        if(targetObj.transform.position.y > boundaries.top.position.y) {
            targetObj.transform.position = new Vector3(targetObj.transform.position.x, boundaries.top.position.y, targetObj.transform.position.z);
        } else if(targetObj.transform.position.y < boundaries.btm.position.y) {
            targetObj.transform.position = new Vector3(targetObj.transform.position.x, boundaries.btm.position.y, targetObj.transform.position.z);
        }
    }

    void UpdatePointer(Vector2 dragPoint) {

        if(gameManager.state != Shoot_GameManager.ShootGameState.playing)
        {
            vecNormal = Vector3.zero;
            return;
        }

        Vector2 vec = new Vector2(dragPoint.x - initialPoint.x, dragPoint.y - initialPoint.y);
        vec = Vector2.ClampMagnitude(vec * clamp_max_velocity, max_radius);
        joysyick_knob.transform.localPosition = vec;
        vecNormal = vec.normalized;
 
        float angle = Mathf.Atan2(vecNormal.y, vecNormal.x) * Mathf.Rad2Deg;
        targetObj.transform.rotation = Quaternion.Lerp(targetObj.transform.rotation, Quaternion.Euler(new Vector3(0f, 0f, angle)), 0.5f);

        joystick_inensity = Vector2.Distance(Vector2.zero, joysyick_knob.transform.localPosition) / 50f;
    }
}