using EasyButtons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameCharacter gameCharacter;
    List<Transform> targets = new List<Transform>();
    [Header("Overall Values")]
    [SerializeField] Vector3 offset;
    [SerializeField] float StateToStateInterpolationSpeed = 100f;

    [Header("DefaultValues")]
    public float moveSpeedx = 5f;
    public float moveSpeedy = 5f;
    public float lookAhead = 2f;
    public float speed = 5f;
    public float damping = 0.01f;
    public Vector2 clampY = new Vector2(-5, 5);
    public Vector2 clampX = new Vector2(-5, 5);

    [Header("MultiTargetValues")]
    [SerializeField] float smoothTime = 0.5f;  // Die Zeit, die die Kamera braucht, um sich zu interpolieren
    [SerializeField] float minZoom = 5f;  // Die minimale Zoomstufe der Kamera
    [SerializeField] float maxZoom = 20f;  // Die maximale Zoomstufe der Kamera
    [SerializeField] float zoomLimiter = 50f;  // Die maximale Entfernung zwischen den Zielen, die die Kamera verfolgt
    [SerializeField] float minDistance = 10f; // Mindestentfernung der Kamera zu den Targets

    Vector3 velocity;
    Camera cam;
    float zoomVelocity;
    CameraStateMachine stateMachine;
    Vector3 prevTargetPos;
    Vector3 mainTargetVelocity;
    Vector3 cameraTargetPosition;
    Vector3 finalCameraPosition;
    Vector3 cameraEffectOffset;
    CameraEffectComponent cameraEffectComponent;

    public List<Transform> Targets { get { return targets; } }
    public CameraStateMachine StateMachine { get { return stateMachine; } }
    public Camera Camera { get { return cam; } }   
    public Vector3 Offset { get { return offset; } }
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    public float ZoomVelocity { get { return zoomVelocity; } set { zoomVelocity = value; } }
    public float MinDistance { get { return minDistance; } }    
    public float ZoomLimiter { get { return zoomLimiter; } }
    public float MaxZoom { get { return maxZoom; } }
    public float MinZoom { get { return minZoom; } }
    public float SmoothTime { get { return smoothTime; } }
    public Vector3 MainTargetVelocity { get { return mainTargetVelocity; } }
    public float MoveSpeedx { get { return moveSpeedx; } }
    public float MoveSpeedy { get { return moveSpeedy; } }
    public float LookAhead { get { return lookAhead; } }
    public float Speed { get { return speed; } }
    public float Damping { get { return damping; } }
    public Vector2 ClampX { get { return clampX; } }
    public Vector2 ClampY { get { return clampY; } }
    public Vector3 CameraTargetPosition { get { return cameraTargetPosition; } set { cameraTargetPosition = value; } }
    public GameCharacter GameCharacter { get { return gameCharacter; } }
    public Vector3 FinalCameraPosition { get { return finalCameraPosition; } set { finalCameraPosition = value; } }
    public Vector3 CameraEffectOffset { get { return cameraEffectOffset; } set { cameraEffectOffset = value; } }
	public CameraEffectComponent CameraEffectComponent { get { return cameraEffectComponent; } }

	public Vector3 velocityVelx = Vector3.zero;
	public Vector3 velocityVely = Vector3.zero;

	[Button("DefaultCameraShake")]
	private void DefaultCameraShake()
    {
        CameraEffectComponent.AddCameraEffect(new CameraEffectShake(this));
    }

	private void Start()
    {
        cam = GetComponent<Camera>();
        cameraEffectComponent = new CameraEffectComponent(this);
        CameraEffectComponent.Init();
    }

    public void OnPosses(GameObject newTarget)
    {
        gameCharacter = newTarget.GetComponent<GameCharacter>();

        targets.Add(newTarget.transform);
        transform.position = newTarget.transform.position + offset;

        stateMachine = gameObject.AddComponent<CameraStateMachine>();
        prevTargetPos = newTarget.transform.position;
        cameraTargetPosition = transform.position; 
    }


	private void Update()
	{
        if (Targets.Count > 0 && Targets[0] != null)
        {
            Vector3 currentPosition = Targets[0].position;
            mainTargetVelocity = (currentPosition - prevTargetPos) / Time.deltaTime;
            prevTargetPos = currentPosition;
        }
    }

    private void LateUpdate()
    {
		CameraEffectComponent.Update(Time.deltaTime);
        transform.position = FinalCameraPosition + CameraEffectOffset;
        CameraEffectOffset = Vector3.zero;
	}
}
