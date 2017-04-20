using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制车移动的脚本 
/// 前轮转向
/// 后轮移动
/// </summary>
public class CarMoveControllerScript : MonoBehaviour {
	
	// 声明四个轮子
	public WheelCollider wheelFL; // 前左 front left
	public WheelCollider wheelFR; // 前右 front right
	public WheelCollider wheelRL; // 后左 rear left
	public WheelCollider wheelRR; // 后右 reat right
	// 声明一个力矩
	public float MAXTORQUE;
	// 低速前轮转向的最大角度
	public float MaxAngleAtLowSpeed;
	// 高速前轮转向的最大角度
	public float MaxAngleAtHigSpeed;
	//相对于转向角的最大速度
	private float MAX_VELOCITY_FOR_STEER = 45;
	// 最大速度
	public float highSpeed;
	// 当前速度
	public float currentSpeed;

	// 记录四个轮子的位置信息
	public Transform wheelFLTransform;
	public Transform wheelFRTransform;
	public Transform wheelRLTransform;
	public Transform wheelRRTransform;

	// 前轮最开始的角度
	private float initRotationFL = 0f;
	private float initRotationFR = 0f;

	void Start () {
		
	}
	void Update () {
		CarWheelMove ();
	}
	void FixedUpdate () {
		CarMove ();
	}
	// 利用力矩让车动起来
	void CarMove () {
		float ver = Input.GetAxis ("Vertical");
		float hor = Input.GetAxis ("Horizontal");
		// 控制前进
		// 车速度的大小　
		currentSpeed = GetComponent<Rigidbody> ().velocity.magnitude;
		if (currentSpeed < highSpeed) {
			wheelRL.motorTorque = MAXTORQUE * ver;
			wheelRR.motorTorque = MAXTORQUE * ver;
		} else {
			wheelRL.motorTorque = 0;
			wheelRR.motorTorque = 0;
		}


		float speedFactor = GetComponent<Rigidbody> ().velocity.magnitude / MAX_VELOCITY_FOR_STEER;
		float steerFactor = Mathf.Lerp (MaxAngleAtLowSpeed, MaxAngleAtHigSpeed, speedFactor);
		// 控制方向的偏移角度
		wheelFL.steerAngle = steerFactor * hor;
		wheelFR.steerAngle = steerFactor * hor;
	}
	void CarWheelMove () {
		// 前左轮转动
		initRotationFL += wheelFL.rpm * 360 * Time.deltaTime / 60;
		initRotationFL = Mathf.Repeat (initRotationFL, 360);
		AngelMove (wheelFLTransform, initRotationFL, wheelFL.steerAngle);
		// 前右轮转动
		initRotationFR += wheelFR.rpm * 360 * Time.deltaTime / 60;
		initRotationFR = Mathf.Repeat (initRotationFL, 360);
		AngelMove (wheelFRTransform, initRotationFL, wheelFR.steerAngle);


		// wheelFLTransform.Rotate (wheelFL.rpm * 360 * Time.deltaTime / 60, 0, 0);
		// wheelFRTransform.Rotate (wheelFR.rpm * 360 * Time.deltaTime / 60, 0, 0);
		wheelRLTransform.Rotate (wheelRL.rpm * 360 * Time.deltaTime / 60, 0, 0);
		wheelRRTransform.Rotate (wheelRR.rpm * 360 * Time.deltaTime / 60, 0, 0);

	}
	void AngelMove(Transform trans, float angle, float yAngle) {
		Vector3 a = new Vector3 (angle, yAngle, 0);
		trans.localRotation = Quaternion.Euler (a);
	}

}
