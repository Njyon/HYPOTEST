using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : Singelton<CameraController>
{
	GameCharacter gameCharacter;
    List<GameCharacter> targets = new List<GameCharacter>();

    [Header("DefaultValues")]
	[SerializeField] Vector3 offset;
	public float moveSpeedx = 5f;
    public float moveSpeedy = 5f;
    public float lookAhead = 2f;
    public float speed = 5f;
    public float damping = 0.01f;
    public Vector2 clampY = new Vector2(-5, 5);
    public Vector2 clampX = new Vector2(-5, 5);
	[SerializeField] float defaultZoomSpeed = 1f;

    [Header("MultiTargetValues")]
	[SerializeField] Vector3 multiOffset;
    [SerializeField] float multiInterpSpeed = 1f;
	[SerializeField] float smoothTime = 0.5f;  // Die Zeit, die die Kamera braucht, um sich zu interpolieren
    [SerializeField] float minFoV = 5f;  // Die minimale Zoomstufe der Kamera
    [SerializeField] float maxFoV = 20f;  // Die maximale Zoomstufe der Kamera
    [SerializeField] float zoomLimiter = 50f;  // Die maximale Entfernung zwischen den Zielen, die die Kamera verfolgt
    [SerializeField] float minDistance = 10f; // Mindestentfernung der Kamera zu den Targets
    [SerializeField] float multiZoomSpeed = 2f; // zoom speed interpotlation

    [Header("Effects")]
    [SerializeField] int TestDebugCameraShakeIndex = 0;
    [SerializeField] List<CameraShakeScriptableObject> cameraShakes = new List<CameraShakeScriptableObject>();

    Vector3 velocity;
    Camera cam;
    float zoomTarget;
    CameraStateMachine stateMachine;
    Vector3 cameraTargetPosition;
    Vector3 finalCameraPosition;
    Vector3 cameraEffectOffset;
    CameraEffectComponent cameraEffectComponent;
    float defaultZoom;

    public List<GameCharacter> Targets { get { return targets; } }
    public CameraStateMachine StateMachine { get { return stateMachine; } }
    public Camera Camera { get { return cam; } }   
    public Vector3 Offset { get { return offset; } }
    public Vector3 MultiOffset { get { return multiOffset; } }
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    public float FinalFoV { get { return zoomTarget; } set { zoomTarget = value; } }
    public float MinDistance { get { return minDistance; } }    
    public float ZoomLimiter { get { return zoomLimiter; } }
    public float MaxFoV { get { return maxFoV; } }
    public float MinFOV { get { return minFoV; } }
    public float SmoothTime { get { return smoothTime; } }
    public float MoveSpeedx { get { return moveSpeedx; } }
    public float MoveSpeedy { get { return moveSpeedy; } }
    public float LookAhead { get { return lookAhead; } }
    public float Speed { get { return speed; } }
    public float Damping { get { return damping; } }
    public float MultiZoomSpeed { get { return multiZoomSpeed; } }
    public float DefaultZoomSpeed { get { return defaultZoomSpeed; } }
    public float MultiInterpSpeed { get { return multiInterpSpeed; } }
    public Vector2 ClampX { get { return clampX; } }
    public Vector2 ClampY { get { return clampY; } }
    public Vector3 CameraTargetPosition { get { return cameraTargetPosition; } set { cameraTargetPosition = value; } }
    public GameCharacter GameCharacter { get { return gameCharacter; } }
    public Vector3 FinalCameraPosition { get { return finalCameraPosition; } set { finalCameraPosition = value; } }
    public Vector3 CameraEffectOffset { get { return cameraEffectOffset; } set { cameraEffectOffset = value; } }
	public CameraEffectComponent CameraEffectComponent { get { return cameraEffectComponent; } }
    public float DefaultZoom { get { return defaultZoom; } }

	[HideInInspector] public Vector3 velocityVelx = Vector3.zero;
	[HideInInspector] public Vector3 velocityVely = Vector3.zero;

	[MyBox.ButtonMethod()]
	private void DefaultCameraShake()
    {
       if (cameraShakes.Count > TestDebugCameraShakeIndex) 
           CameraEffectComponent.AddCameraEffect(new CameraEffectShake(this, cameraShakes[TestDebugCameraShakeIndex]));
    }

	private void Start()
    {
        cam = GetComponent<Camera>();
        cameraEffectComponent = new CameraEffectComponent(this);
        defaultZoom = cam.fieldOfView;
        FinalFoV = defaultZoom;
        CameraEffectComponent.Init();
    }

    public void OnPosses(GameObject newTarget)
    {
        gameCharacter = newTarget.GetComponent<GameCharacter>();
        gameCharacter.onGameCharacterGotArroged += AddGameCharacterToTargets;
        gameCharacter.onGameCharacterStoppedBeingArroged += RemoveGameCharacterFromTargets;

		targets.Add(gameCharacter);
        transform.position = newTarget.transform.position + offset;

        stateMachine = gameObject.AddComponent<CameraStateMachine>();
        cameraTargetPosition = transform.position; 
    }


	void Update()
	{
		
	}

	void LateUpdate()
    {
		CameraEffectComponent.Update(Time.deltaTime);
        transform.position = FinalCameraPosition + CameraEffectOffset;
        cam.fieldOfView = FinalFoV;
		CameraEffectOffset = Vector3.zero;
	}

	void OnDestroy()
	{
        if (gameCharacter != null)
        {
		    gameCharacter.onGameCharacterGotArroged -= AddGameCharacterToTargets;
		    gameCharacter.onGameCharacterStoppedBeingArroged -= RemoveGameCharacterFromTargets;
        }

	}

	public void ShakeCamerea(int index)
    {
		if (cameraShakes.Count > index)
			CameraEffectComponent.AddCameraEffect(new CameraEffectShake(this, cameraShakes[index]));
	}

    void AddGameCharacterToTargets(GameCharacter aggroedGameCharacter)
    {
        targets.Add(aggroedGameCharacter);
    }

    void RemoveGameCharacterFromTargets(GameCharacter removedAggroGameChracter)
    {
        targets.Remove(removedAggroGameChracter);
    }
}
