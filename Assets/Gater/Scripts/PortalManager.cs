using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
	using UnityEditorInternal;
	using UnityEditor;
#endif

[DisallowMultipleComponent]
[ExecuteInEditMode]

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Collider))]

public class PortalManager : MonoBehaviour {
	public bool EnableColliderTrigger = true;
	public bool EnableClipPlaneObjs = true;
	[HideInInspector] public GameObject ClipPlanePosObj;
	private Material GateMaterial;
	private Material ClipPlaneMaterial;
	private Material CloneClipPlaneMaterial;
	public Camera ProjectionCamera;
	public enum ProjectionTypeList{Perspective, Ortographic}; public ProjectionTypeList ProjectionType;
	private Vector2 PreviousProjectionResolution;
	public Vector2 ProjectionResolution = new Vector2(1280, 1024);
	public GameObject SceneTerrain;
	private GameObject RecursionMask;
	[Range(0, 31)] public int RecursionMaskLayer = 1;
	public Material RecursionMaskMaterial;
	private string GateLayer = "TransparentFX";
	public GameObject[] ExcludedWalls;
	public Material SecondGateCustomSkybox;
	public GameObject SecondGate;
	[HideInInspector] public GameObject GateCamObj;

	void OnEnable () {
		//Disable collision of portal with walls
		for (int i = 0; i < ExcludedWalls.Length; i++)
			Physics.IgnoreCollision (transform.GetComponent<Collider> (), ExcludedWalls [i].GetComponent<Collider> (), true);

		//Generate "Portal" and "Clipping plane" materials
		if (!GateMaterial)
			GateMaterial = new Material (Shader.Find ("Gater/UV Remap"));

		string ClipPlaneShaderName = "Custom/StandardClippable";

		if (!ClipPlaneMaterial)
			ClipPlaneMaterial = new Material (Shader.Find (ClipPlaneShaderName));
		if (!CloneClipPlaneMaterial)
			CloneClipPlaneMaterial = new Material (Shader.Find (ClipPlaneShaderName));

		//Apply custom settings to the portal components
		GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		GetComponent<MeshRenderer> ().receiveShadows = false;
		GetComponent<MeshRenderer> ().sharedMaterial = GateMaterial;

		GetComponent<Rigidbody> ().mass = 1;
		GetComponent<Rigidbody> ().drag = 0;
		GetComponent<Rigidbody> ().angularDrag = 0;
		GetComponent<Rigidbody> ().useGravity = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		GetComponent<Rigidbody> ().interpolation = RigidbodyInterpolation.None;
		GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;

		if (GetComponent<MeshCollider> ()) {
			GetComponent<MeshCollider> ().convex = true;
			GetComponent<MeshCollider> ().sharedMaterial = null;
		}
		
		#if UNITY_EDITOR
			//Search trace of required objects for teleport, and fill the relative variables if these are already existing on the current Editor instance
			for (int i = 0; i < transform.GetComponentsInChildren<Transform> ().Length; i++) {
				if (transform.GetComponentsInChildren<Transform> () [i].name == this.gameObject.name + " Camera")
					GateCamObj = transform.GetComponentsInChildren<Transform> () [i].gameObject;
				if (transform.GetComponentsInChildren<Transform> () [i].name == transform.name + " RecursionMask")
					RecursionMask = transform.GetComponentsInChildren<Transform> () [i].gameObject;
				if (transform.GetComponentsInChildren<Transform> () [i].name == transform.name + " ClipPlanePosObj")
					ClipPlanePosObj = transform.GetComponentsInChildren<Transform> () [i].gameObject;
			}
			
			//Enable editor update on a specific function
			EditorApplication.update = null;
			if (!EditorApplication.isPlaying)
				EditorApplication.update = EditorUpdate;
		#endif
	}


	void EditorUpdate () {
		try {
			GenGate ();

			GateCamRepos ();
		} catch { }
	}

	void Update () {
		GenGate ();
	}

	void LateUpdate () {
		GateCamRepos ();
	}

	private Camera CurrentCam;
	private string LastMetdFocusedWindowName;
	private bool MetdActiveSceneView;
	private bool FillMetdActiveSceneView = true;

	//Acquire Scene/Game camera
	Camera GetCurrentCam (Camera InitMetdCurrentCam) {
		Camera LocMetdCurrentCam = InitMetdCurrentCam;
		Camera MetdMainCam = ProjectionCamera;

		#if UNITY_EDITOR
			bool MetdFocusedWindow = SceneView.focusedWindow;
			string MetdFocusedWindowName = MetdFocusedWindow ? SceneView.focusedWindow.titleContent.text : "";
			
			MetdFocusedWindowName = MetdFocusedWindowName == "Scene" || MetdFocusedWindowName == "Game" ? (Screen.width >= 450 ? "Game" : "Scene") : MetdFocusedWindowName;
			
			Camera MetdSceneCam = SceneView.GetAllSceneCameras ().Length > 0 ? SceneView.GetAllSceneCameras () [0] : Camera.current;
			
			if (!FillMetdActiveSceneView && LastMetdFocusedWindowName != MetdFocusedWindowName)
				if (MetdFocusedWindow && (MetdFocusedWindowName == "Scene" || MetdFocusedWindowName == "Game")) {
					FillMetdActiveSceneView = true;
					
					LastMetdFocusedWindowName = MetdFocusedWindowName;
				}
			if (FillMetdActiveSceneView && SceneView.lastActiveSceneView) {
				MetdActiveSceneView = SceneView.FrameLastActiveSceneView ();
				
				FillMetdActiveSceneView = false;
			}
			
			LocMetdCurrentCam = MetdFocusedWindow && !EditorApplication.isPlaying ?
									(MetdFocusedWindowName != "Scene" ?
										(MetdFocusedWindowName != "Game" ?
											(!LocMetdCurrentCam ?
												(MetdActiveSceneView ?
													MetdSceneCam
													: MetdMainCam)
												: LocMetdCurrentCam)
											: MetdMainCam)
										: MetdSceneCam)
									: (MetdActiveSceneView ?
										MetdSceneCam
										: MetdMainCam);
			LocMetdCurrentCam = SecondGate && SecondGate.GetComponent<PortalManager> ().SecondGate && SecondGate.activeSelf ? LocMetdCurrentCam : null;
		#else
			LocMetdCurrentCam = MetdMainCam;
		#endif

		return LocMetdCurrentCam;
	}

	private RenderTexture RenTex;
	private Mesh GateMesh;

	void GenGate () {
		if (this.gameObject.layer != LayerMask.NameToLayer (GateLayer))
			this.gameObject.layer = LayerMask.NameToLayer (GateLayer);

		//Fill "ProjectionCamera" variable, if its value is null
		if (!ProjectionCamera)
			ProjectionCamera = Camera.main;

		//Generate recursion mask, used by portal camera for replace infinite effect with a custom material
		if (!RecursionMask) {
			RecursionMask = new GameObject (transform.name + " RecursionMask");

			RecursionMask.AddComponent<MeshFilter> ();
			RecursionMask.AddComponent<MeshRenderer> ();

			RecursionMask.transform.position = transform.position;
			RecursionMask.transform.rotation = transform.rotation;
			RecursionMask.transform.parent = transform;
			RecursionMask.transform.localPosition = new Vector3 (0, 0, .001f);
			RecursionMask.transform.localScale = Vector3.one;
		}
		if (RecursionMask) {
			RecursionMask.layer = RecursionMaskLayer;

			RecursionMask.GetComponent<MeshFilter> ().sharedMesh = GateMesh;

			RecursionMask.GetComponent<MeshRenderer> ().sharedMaterial = RecursionMaskMaterial;
		}
		
		//Generate position/rotation reference object, used by "Clipping Plane" system for slice objects inside the portal
		if (!ClipPlanePosObj) {
			ClipPlanePosObj = new GameObject (transform.name + " ClipPlanePosObj");

			ClipPlanePosObj.transform.position = transform.position;
			ClipPlanePosObj.transform.rotation = transform.rotation;
			ClipPlanePosObj.transform.parent = transform;
			ClipPlanePosObj.transform.localPosition = new Vector3 (0, 0, .007f);
		}

		//Generate camera for the portal rendering
		if (!GateCamObj) {
			GateCamObj = new GameObject (this.gameObject.name + " Camera");

			GateCamObj.tag = "Untagged";

			GateCamObj.transform.parent = transform;

			GateCamObj.AddComponent<Camera> ();
			GateCamObj.GetComponent<Camera> ().nearClipPlane = .01f;

			GateCamObj.AddComponent<Skybox> ();

			PreviousProjectionResolution = new Vector2 (0, 0);
		}
		if (GateCamObj) {
			if (CurrentCam == ProjectionCamera && GateCamObj.GetComponent<Camera> ().depth != CurrentCam.depth - 1)
				GateCamObj.GetComponent<Camera> ().depth = CurrentCam.depth - 1;

			//Acquire settings from Scene/Game camera, to apply on Portal camera
			if (CurrentCam) {
				GateCamObj.GetComponent<Camera> ().aspect = CurrentCam.aspect;
				GateCamObj.GetComponent<Camera> ().cullingMask = ~(0 << 1);
				GateCamObj.GetComponent<Camera> ().cullingMask &= ~(1 << LayerMask.NameToLayer(GateLayer));
				GateCamObj.GetComponent<Camera> ().cullingMask &= ~(1 << RecursionMaskLayer);
				GateCamObj.GetComponent<Camera> ().orthographic = ProjectionType == ProjectionTypeList.Ortographic ? true : false;
				GateCamObj.GetComponent<Camera> ().fieldOfView = CurrentCam.fieldOfView;
				GateCamObj.GetComponent<Camera> ().farClipPlane = CurrentCam.farClipPlane;
				GateCamObj.GetComponent<Camera> ().renderingPath = CurrentCam.renderingPath;
				GateCamObj.GetComponent<Camera> ().useOcclusionCulling = CurrentCam.useOcclusionCulling;
				GateCamObj.GetComponent<Camera> ().allowHDR = CurrentCam.allowHDR;
			}

            // Commented out because it throws errors
			// SecondGate.GetComponent<PortalManager>().GateCamObj.GetComponent<Skybox> ().material = SecondGateCustomSkybox;
		}

		//Generate render texture for the portal camera
		if (PreviousProjectionResolution.x != ProjectionResolution.x || PreviousProjectionResolution.y != ProjectionResolution.y) {
			if (GateCamObj.GetComponent<Camera> ().targetTexture != null)
				GateCamObj.GetComponent<Camera> ().targetTexture = null;
			if (GateCamObj.GetComponent<Camera> ().targetTexture == null) {
				if (RenTex) {
					#if UNITY_EDITOR
						if (!EditorApplication.isPlaying)
							DestroyImmediate (RenTex, false);
						if (EditorApplication.isPlaying)
							Destroy (RenTex);
					#else
						Destroy(RenTex);
					#endif
				}
				if (!RenTex) {
					RenTex = new RenderTexture (Convert.ToInt32 (ProjectionResolution.x), Convert.ToInt32 (ProjectionResolution.y), 24, RenderTextureFormat.ARGB32);
					RenTex.name = this.gameObject.name + " RenderTexture";

					GateCamObj.GetComponent<Camera> ().targetTexture = RenTex;

					PreviousProjectionResolution = new Vector2 (RenTex.width, RenTex.height);
				}
			}
		}

		CurrentCam = GetCurrentCam (CurrentCam);

		//Reset arrays elements of the required objects for teleport, if Scene/Game camera variable is null and any object is still colliding with the portal
		if (!CurrentCam)
			ResetVars (false);

		//Acquire current portal mesh
		GateMesh = GetComponent<MeshFilter> ().sharedMesh;
		//Apply render texture to the portal material
		GetComponent<MeshRenderer> ().sharedMaterial.SetTexture ("_MainTex", CurrentCam ? SecondGate.GetComponent<PortalManager> ().RenTex : null);

		//Apply current portal mesh to the mesh collider if exist
		if (GetComponent<MeshCollider> ())
			if (GetComponent<MeshCollider> ().sharedMesh != GateMesh)
				GetComponent<MeshCollider> ().sharedMesh = GateMesh;
		//Disable trigger of portal collider
		if (GetComponent<Collider> ().isTrigger != (CurrentCam ? true : false))
			GetComponent<Collider> ().isTrigger = CurrentCam ? (EnableColliderTrigger ? true : false) : false;

		//Check if the walls have "Box Collider" component
		for (int i = 0; i < ExcludedWalls.Length; i++)
			if (!ExcludedWalls [i].GetComponent<BoxCollider> ())
				Debug.LogError ("One excluded wall doesn't have Box Collider component");
	}

	void GateCamRepos () {
		//Move portal camera to position/rotation of Scene/Game camera
		if (CurrentCam) {
			Vector3 GateCamPos = SecondGate.transform.InverseTransformPoint (CurrentCam.transform.position);

			GateCamPos.x = -GateCamPos.x;
			GateCamPos.z = -GateCamPos.z;
			GateCamObj.transform.localPosition = GateCamPos;

			Quaternion GateCamRot = Quaternion.Inverse (SecondGate.transform.rotation) * CurrentCam.transform.rotation;

			GateCamRot = Quaternion.AngleAxis (180.0f, new Vector3 (0, 1, 0)) * GateCamRot;
			GateCamObj.transform.localRotation = GateCamRot;
		}
	}

	class InitMaterialsList { public Material[] Materials; }
	private GameObject[] CollidedObjs = new GameObject[0];
	private string[] CollidedObjsInitName = new string[0];
	private InitMaterialsList[] CollidedObjsInitMaterials = new InitMaterialsList[0];
	private bool[] CollidedObjsAlwaysTeleport = new bool[0];
	private bool[] CollidedObjsFirstTrig = new bool[0];
	private float[] CollidedObjsFirstTrigDist = new float[0];
	private GameObject[] ProxDetCollidedObjs = new GameObject[0];
	private GameObject[] CloneCollidedObjs = new GameObject[0];
	private Vector3[] CollidedObjVelocity = new Vector3[0];
	private bool[] ContinueTriggerEvents = new bool[0];

	void OnTriggerEnter (Collider collision) {
        // Commented out - we teleport manually
        return;

		//Increment and partially fill the arrays elements of required object for teleport
		if (collision.gameObject != this.gameObject && collision.gameObject != SceneTerrain && !collision.GetComponent<PortalManager> () && !collision.name.Contains (collision.gameObject.GetHashCode ().ToString ()) && !collision.name.Contains ("Clone")) {
			Array.Resize (ref CollidedObjs, CollidedObjs.Length + 1);
			Array.Resize (ref CollidedObjsInitName, CollidedObjsInitName.Length + 1);
			Array.Resize (ref CollidedObjsInitMaterials, CollidedObjsInitMaterials.Length + 1);
			Array.Resize (ref CollidedObjsAlwaysTeleport, CollidedObjsAlwaysTeleport.Length + 1);
			Array.Resize (ref CollidedObjsFirstTrig, CollidedObjsFirstTrig.Length + 1);
			Array.Resize (ref CollidedObjsFirstTrigDist, CollidedObjsFirstTrigDist.Length + 1);
			Array.Resize (ref ProxDetCollidedObjs, ProxDetCollidedObjs.Length + 1);
			Array.Resize (ref CloneCollidedObjs, CloneCollidedObjs.Length + 1);
			Array.Resize (ref CollidedObjVelocity, CollidedObjVelocity.Length + 1);
			Array.Resize (ref ContinueTriggerEvents, ContinueTriggerEvents.Length + 1);

			CollidedObjs [CollidedObjs.Length - 1] = collision.gameObject;
			CollidedObjsInitName [CollidedObjsInitName.Length - 1] = collision.gameObject.name;

			if(ExcludedWalls.Length > 0)
				for (int i = 0; i < ExcludedWalls.Length; i++)
					if (ExcludedWalls [i] && ExcludedWalls [i].GetComponent<Collider> ())
						Physics.IgnoreCollision(CollidedObjs [CollidedObjsInitName.Length - 1].GetComponent<Collider> (), ExcludedWalls [i].GetComponent<Collider> (), true);

			if (CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ()) {
				bool StandardObjShader = false;

				if (!StandardObjShader) {
					if (CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterial.shader.name != "Standard") {
						Debug.LogError ("The shader of object material is not 'Standard'");

						#if UNITY_EDITOR
							EditorApplication.isPaused = true;
						#endif
					}
					if (CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterial.shader.name == "Standard")
						StandardObjShader = true;
				}
				if (StandardObjShader) {
					if (CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> () && EnableClipPlaneObjs) {
						CollidedObjsInitMaterials [CollidedObjsInitMaterials.Length - 1] = new InitMaterialsList ();

						CollidedObjsInitMaterials [CollidedObjsInitMaterials.Length - 1].Materials = CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterials;

						CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterial = ClipPlaneMaterial;
						CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterial.CopyPropertiesFromMaterial (CollidedObjsInitMaterials [CollidedObjs.Length - 1].Materials [0]);
					}
				}
			}

			ContinueTriggerEvents [ContinueTriggerEvents.Length - 1] = true;
		}
	}

	private GameObject ObjCollidedCamObj = null;
	private GameObject ObjCloneCollidedCamObj = null;

	void OnTriggerStay (Collider collision) {
        // Commented out - we teleport manually
        return;

        //Change position/rotation of required objects for teleport, and complete the fill of remaining arrays elements
        for (int i = 0; i < CollidedObjs.Length; i++) {
			if (ContinueTriggerEvents [i] && CollidedObjs [i]) {
				if (!ProxDetCollidedObjs [i]) {
					ProxDetCollidedObjs [i] = new GameObject (CollidedObjs [i].name + " Proximity Detector");

					ProxDetCollidedObjs [i].transform.position = transform.position;
					ProxDetCollidedObjs [i].transform.rotation = transform.rotation;
					ProxDetCollidedObjs [i].transform.parent = transform;
				}

				bool FirstPersonCharacter;

				if(CollidedObjs [i].transform.childCount > 0)
					for (int j = 0; j < CollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; j++)
						if (CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].GetComponent<Camera>())
							ObjCollidedCamObj = CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].gameObject;

				if (GateMesh && ProxDetCollidedObjs [i]) {
					FirstPersonCharacter = ObjCollidedCamObj && ObjCollidedCamObj.GetComponent<Camera> () && CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () ? true : false;

					Vector3 ProxDetCollidedObjPos = transform.InverseTransformPoint ((FirstPersonCharacter ? ObjCollidedCamObj.transform.position : CollidedObjs [i].transform.position));
					Vector3 ProxDetCollLimit = new Vector3 (GateMesh.bounds.size.x / 2, GateMesh.bounds.size.y / 2, GateMesh.bounds.size.z / 2);

					ProxDetCollidedObjs [i].transform.localPosition = new Vector3 (ProxDetCollidedObjPos.x > -ProxDetCollLimit.x && ProxDetCollidedObjPos.x < ProxDetCollLimit.x ? ProxDetCollidedObjPos.x : ProxDetCollidedObjs [i].transform.localPosition.x, ProxDetCollidedObjPos.y > -ProxDetCollLimit.y && ProxDetCollidedObjPos.y < ProxDetCollLimit.y ? ProxDetCollidedObjPos.y : ProxDetCollidedObjs [i].transform.localPosition.y, ProxDetCollidedObjs [i].transform.localPosition.z);

					if (!CollidedObjsAlwaysTeleport [i]) {
						if (!CollidedObjsFirstTrig [i]) {
							CollidedObjsFirstTrigDist [i] = Vector3.Dot (CollidedObjs [i].transform.position - ProxDetCollidedObjs [i].transform.position, ProxDetCollidedObjs [i].transform.forward);

							CollidedObjsFirstTrig [i] = true;
						}
						if (CollidedObjsFirstTrig [i] && CollidedObjsFirstTrigDist [i] < 0) {
							CollidedObjsAlwaysTeleport [i] = true;
							
							CollidedObjs [i].name = CollidedObjs [i].name + " " + CollidedObjs [i].GetHashCode ().ToString ();
						}
					}
					if (CollidedObjsAlwaysTeleport [i]) {
						if (!CloneCollidedObjs [i]) {
							CloneCollidedObjs [i] = (GameObject)Instantiate (CollidedObjs [i], SecondGate.transform.position, SecondGate.transform.rotation);
							
							if(CloneCollidedObjs [i].transform.childCount > 0)
								for (int k = 0; k < CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; k++)
									if (CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<Camera>())
										ObjCloneCollidedCamObj = CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].gameObject;
							
							if (CloneCollidedObjs [i].GetComponent<MeshRenderer> ()) {
								if (EnableClipPlaneObjs) {
									CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial = CloneClipPlaneMaterial;
									CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.CopyPropertiesFromMaterial (CollidedObjsInitMaterials [i].Materials [0]);
								}
								if (!EnableClipPlaneObjs) {
									CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterials = CollidedObjsInitMaterials [i].Materials;
									CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterials = CollidedObjsInitMaterials [i].Materials;
								}
							}

							if (ObjCloneCollidedCamObj)
								if (ObjCloneCollidedCamObj.GetComponent<Camera> () && CloneCollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ()) {
									ObjCloneCollidedCamObj.GetComponent<Camera> ().GetComponent<AudioListener> ().enabled = false;
									CloneCollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = false;

									CloneCollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_OriginalCameraPosition = CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_OriginalCameraPosition;
								}

							CloneCollidedObjs [i].name = CollidedObjsInitName [i] + " Clone";

							CloneCollidedObjs [i].transform.position = SecondGate.transform.position;
							CloneCollidedObjs [i].transform.parent = SecondGate.transform;
						}
						if (CloneCollidedObjs [i]) {
							float CollidedObjProxDetDistStay = Vector3.Dot ((FirstPersonCharacter ? ObjCollidedCamObj.transform.position : CollidedObjs [i].transform.position) - ProxDetCollidedObjs [i].transform.position, ProxDetCollidedObjs [i].transform.forward);
							Vector3 CloneCollidedObjLocalPos = transform.InverseTransformPoint (CollidedObjs [i].transform.position);

							CloneCollidedObjLocalPos.x = -CloneCollidedObjLocalPos.x;
							CloneCollidedObjLocalPos.z = FirstPersonCharacter ? -CloneCollidedObjLocalPos.z - .007f : -CloneCollidedObjLocalPos.z;

							CloneCollidedObjs [i].transform.localPosition = CloneCollidedObjLocalPos;

							Quaternion CloneCollidedObjLocalRot = Quaternion.Inverse (transform.rotation) * (CollidedObjs [i].transform.rotation);

							CloneCollidedObjLocalRot = Quaternion.AngleAxis (180.0f, new Vector3 (0, -1, 0)) * CloneCollidedObjLocalRot;

							CloneCollidedObjs [i].transform.localRotation = CloneCollidedObjLocalRot;

							if (ObjCollidedCamObj && ObjCloneCollidedCamObj) {
								if (!ObjCloneCollidedCamObj.GetComponent<Skybox> ())
									ObjCloneCollidedCamObj.AddComponent<Skybox> ();
								if (ObjCloneCollidedCamObj.GetComponent<Skybox> ()) {
									ObjCloneCollidedCamObj.GetComponent<Skybox> ().material = SecondGateCustomSkybox;

									if (!SecondGateCustomSkybox) {
										#if UNITY_EDITOR
										if (!EditorApplication.isPlaying)
											DestroyImmediate (ObjCloneCollidedCamObj.GetComponent<Skybox> (), false);
										if (EditorApplication.isPlaying)
											Destroy (ObjCloneCollidedCamObj.GetComponent<Skybox> ());
										#else
										Destroy(ObjCloneCollidedCamObj.GetComponent<Skybox> ());
										#endif
									}
								}

								float DistAmount = -.014f;
								
								ObjCollidedCamObj.GetComponent<Camera> ().enabled = CollidedObjProxDetDistStay >= DistAmount ? false : true;
								ObjCloneCollidedCamObj.GetComponent<Camera> ().enabled = CollidedObjProxDetDistStay >= DistAmount ? true : false;
								ProjectionCamera = CollidedObjProxDetDistStay >= DistAmount ? ObjCloneCollidedCamObj.GetComponent<Camera>() : ObjCollidedCamObj.GetComponent<Camera>();

								ObjCloneCollidedCamObj.transform.localPosition = ObjCollidedCamObj.transform.localPosition;
								ObjCloneCollidedCamObj.transform.localRotation = ObjCollidedCamObj.transform.localRotation;

								if (CurrentCam == ProjectionCamera && CurrentCam.nearClipPlane > .01f) {
									Debug.LogError ("The nearClipPlane of Main Camera is not equal to 0.01");

									#if UNITY_EDITOR
										EditorApplication.isPaused = true;
									#endif
								}
							}

							if (CollidedObjs [i].GetComponent<MeshRenderer> () && CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial == ClipPlaneMaterial) {
								Vector3 DirectionVector = Vector3.forward;

								CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.SetVector ("_planePos", ClipPlanePosObj.transform.position);
								CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.SetVector ("_planeNorm", Quaternion.Euler (transform.eulerAngles) * -DirectionVector);

								CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.SetVector ("_planePos", SecondGate.GetComponent<PortalManager> ().ClipPlanePosObj.transform.position);
								CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.SetVector ("_planeNorm", Quaternion.Euler (SecondGate.transform.eulerAngles) * -DirectionVector);
							}
						}
					}
				}
			}
		}
	}

	void OnTriggerExit (Collider collision) {
        // Commented out - we teleport manually
        return;

        //Destroy required objects for teleport, reset relative arrays, and move original collided object to the its final position/rotation
        for (int i = 0; i < CloneCollidedObjs.Length; i++) {
			if (ContinueTriggerEvents [i] && CollidedObjs [i] && CollidedObjs [i].GetHashCode ().ToString () == collision.gameObject.GetHashCode ().ToString () && CloneCollidedObjs [i]) {
				if (CollidedObjVelocity [i] == new Vector3 (0, 0, 0))
					CollidedObjVelocity [i] = CollidedObjs [i].GetComponent<Rigidbody> () ? CollidedObjs [i].GetComponent<Rigidbody> ().velocity.magnitude * -SecondGate.transform.forward : new Vector3 (0, 0, 0);

				float CollObjProxDetDistExit = Vector3.Dot (CollidedObjs [i].transform.position - ProxDetCollidedObjs [i].transform.position, ProxDetCollidedObjs [i].transform.forward);
				
				if(CollidedObjs [i].transform.childCount > 0)
					for (int j = 0; j < CollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; j++)
						if (CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].GetComponent<Camera>())
							ObjCollidedCamObj = CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].gameObject;
				if(CloneCollidedObjs [i].transform.childCount > 0)
					for (int k = 0; k < CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; k++)
						if (CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<Camera>())
							ObjCloneCollidedCamObj = CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].gameObject;
				
				if (CollObjProxDetDistExit > 0) {
					CollidedObjs [i].transform.position = CloneCollidedObjs [i].transform.position;
					CollidedObjs [i].transform.rotation = CloneCollidedObjs [i].transform.rotation;

					if (ObjCollidedCamObj && ObjCloneCollidedCamObj)
						if (ObjCloneCollidedCamObj.GetComponent<Camera> () && CloneCollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ()) {
							if (SecondGateCustomSkybox && ObjCloneCollidedCamObj.GetComponent<Skybox> ())
								if(!ObjCollidedCamObj.GetComponent<Skybox> ())
									ObjCollidedCamObj.AddComponent<Skybox> ();
							if(ObjCollidedCamObj.GetComponent<Skybox> ())		
								ObjCollidedCamObj.GetComponent<Skybox> ().material = SecondGateCustomSkybox;
							
							ObjCollidedCamObj.GetComponent<Camera> ().enabled = true;

							CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_MouseLook.Init (CollidedObjs [i].transform, CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_Camera.transform);
						}

					if (CollidedObjVelocity [i] != new Vector3 (0, 0, 0))
						CollidedObjs [i].GetComponent<Rigidbody> ().velocity = CollidedObjVelocity [i];

					if(ExcludedWalls.Length > 0)
						for (int j = 0; j < ExcludedWalls.Length; j++)
							if (ExcludedWalls [j] && ExcludedWalls [j].GetComponent<Collider> ())
								Physics.IgnoreCollision(CollidedObjs [i].GetComponent<Collider> (), ExcludedWalls [j].GetComponent<Collider> (), false);
				}

				CollidedObjs [i].name = CollidedObjsInitName [i];

				if (CollidedObjs [i].GetComponent<MeshRenderer> () && EnableClipPlaneObjs) {
					CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterials = CollidedObjsInitMaterials [i].Materials;

					CollidedObjsInitMaterials [i].Materials = null;
				}

				CollidedObjs [i] = null;
				CollidedObjsInitName [i] = "";
				CollidedObjsAlwaysTeleport [i] = false;
				CollidedObjsFirstTrig [i] = false;
				CollidedObjsFirstTrigDist [i] = 0;
				Destroy (ProxDetCollidedObjs [i]);
				Destroy (CloneCollidedObjs [i]);
				CollidedObjVelocity [i] = new Vector3 (0, 0, 0);
				ContinueTriggerEvents [i] = false;
			}
		}

		ResetVars (true);
	}

	void ResetVars (bool TriggerExit) {
		bool SetVars = false;

		if (CollidedObjs.Length > 0) {
			if (!TriggerExit) {
				for (int i = 0; i < CollidedObjs.Length; i++) {
					if (CloneCollidedObjs [i])
						Destroy (CloneCollidedObjs [i]);
					
					if (ProxDetCollidedObjs [i])
						Destroy (ProxDetCollidedObjs [i]);
					
					if (CollidedObjs [i] && CollidedObjs [i].transform.childCount > 0 && CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ()) {
						Camera CollObjCam = null;

						for (int j = 0; j < CollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; j++)
							if (CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].GetComponent<Camera>())
								CollObjCam = CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].GetComponent<Camera> ();

						if (CollObjCam && !CollObjCam.enabled)
							CollObjCam.enabled = true;
					}
				}

				SetVars = true;
			}
			if (TriggerExit) {
				int ElementsChecked = 0;

				for (int i = 0; i < CollidedObjs.Length; i++)
					if (!CollidedObjs [i])
						ElementsChecked += 1;

				if (ElementsChecked == CollidedObjs.Length)
					SetVars = true;

				ElementsChecked = 0;
			}
		}

		if (SetVars) {
			CollidedObjs = new GameObject[0];
			CollidedObjsInitName = new string[0];
			CollidedObjsInitMaterials = new InitMaterialsList[0];
			CollidedObjsAlwaysTeleport = new bool[0];
			CollidedObjsFirstTrig = new bool[0];
			CollidedObjsFirstTrigDist = new float[0];
			ProxDetCollidedObjs = new GameObject[0];
			CloneCollidedObjs = new GameObject[0];
			CollidedObjVelocity = new Vector3[0];
			ContinueTriggerEvents = new bool[0];
		}
	}
}