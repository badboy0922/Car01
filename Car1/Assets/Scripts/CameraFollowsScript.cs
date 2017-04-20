using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机的跟随 
/// 可以显得更加真实
/// </summary>
public class CameraFollowsScript : MonoBehaviour {
	public GameObject car;
	// 水平方向上摄像头和车的距离
	public float distance = 8f;
	// 垂直方向上车和摄像头的距离
	public float heightDiff = 4f;

	// 角度缓冲系数 当车转向的时候  让摄像头的角度发生改变
	public float angleDamping = 1.5f;
	// 摄像头的高度的缓冲系数 车在水平高度上发生变化时候 摄像机的跟随 
	public float heightDamping = 1f;
	// 摄像头初始视野范围
	public float cameraScope = 60f;
	// 当加速的时候 视野范围的缩放比例
	public float zoomScale = 1.1f;
	// 记录车的方向
	private float carDirection;

	void Update () {
		CamereFollow ();
	}
	void CamereFollow () {
		// 得到摄像机自身的高度
		float cameraHight = transform.position.y;
		// 得到车的高度
		float carHight = car.transform.position.y + heightDiff;
		// 在车的高度和摄像机的高度之间去一个插值
		float lerpHight = Mathf.Lerp (cameraHight, carHight, heightDamping * Time.deltaTime);

		// 得到摄像机在y轴上的角
		float cameraAngle = transform.eulerAngles.y;
		// 得到车在y轴上的角 
		float carAngle = carDirection;
		// 得到两个角之间的插值
		float lerpAngle = Mathf.LerpAngle (cameraAngle, carAngle, angleDamping * Time.deltaTime);
		// 将角度的变化用四元数表示
		Quaternion changeRotation = Quaternion.Euler (0, lerpAngle, 0);
		// 让摄像机的位置等于车的位置
		transform.position = car.transform.position;
		// 让摄像头和车沿着四元数拉开一定得距离
		transform.position -= changeRotation * Vector3.forward * distance;
		// 用一个坐标来存储摄像头的位置
		Vector3 temp = transform.position;
		// 让摄像机的高度在车的基础上上升一个距离 (y轴方向)
		temp.y = lerpHight;
		// 让摄像头的位置等于改变后的位置
		transform.position = temp;
		// 改变摄像头的朝向
		transform.LookAt (car.transform);
	}
	void FixedUpdate () {

		// 得到车的速度
		Vector3 v = car.GetComponent<Rigidbody>().velocity;
		// 车当前的朝向
		//Vector3 faceDirection = car.transform.forward;
		// 速度和车的方向都是向量 前进的时候 两个向量的点积大于零  后退的时候 夹角为钝角 点积小于零
		//float dot = Vector3.Dot(v, faceDirection);
		// 转换成本地坐标
		Vector3 localV = car.transform.InverseTransformDirection(v);
		// 车当前的方向
		float currentAngleY = car.transform.eulerAngles.y;
		if (localV.z < -0.001) {
			carDirection = currentAngleY + 180;
		} else {
			carDirection = currentAngleY;
		}
		// 得到速度的大小
		float speed = v.magnitude;
		// 摄像机的视野变化
		GetComponent<Camera>().fieldOfView = cameraScope + speed * zoomScale;
	}
}