using System.Collections;
using System.ComponentModel;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Rendering;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class StatusBar : ModifierData {
		public static StatusBar Instance;
		
		private GameObject statusBar;
		public Transform redT;
		public Transform blueT;
		public Transform blackT;
		public Transform detectionT;
		

		private Mesh cubeMesh;
		public Material red;
		public Material blue;
		public Material black;
		public Material green;

		public float height = 0.25f;
		private static readonly int ColorProp = Shader.PropertyToID("_Color");
		
		public override void Init()
		{
			if (Instance != null) return;
			
			base.Init();
			local = Instance = this;
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubeMesh = go.GetComponent<MeshFilter>().sharedMesh;
			red = new Material(Shader.Find("Sprites/Default"));
			red.SetColor(ColorProp, Color.red);
			blue = new Material(Shader.Find("Sprites/Default"));
			blue.SetColor(ColorProp, Color.blue);
			black = new Material(Shader.Find("Sprites/Default"));
			black.SetColor(ColorProp, Color.black);
			green = new Material(Shader.Find("Sprites/Default"));
			green.SetColor(ColorProp, Color.green);
			GameObject.Destroy(go);
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();

			CreateBar();
		}
		
		protected override void OnDisable()
		{
			base.OnDisable();
			
			GameObject.Destroy(statusBar);
		}

		private void CreateBar()
		{
			statusBar = new GameObject("statusBar");
			redT = new GameObject("red").transform;
			redT.SetParent(statusBar.transform);
			redT.localPosition = new Vector3(0, 0.03f, 0);
			redT.localScale = new Vector3(1, 0.05f, 0.02f);
			
			blueT = new GameObject("blue").transform;
			blueT.SetParent(statusBar.transform);
			blueT.localPosition = new Vector3(0, -0.03f, 0);
			blueT.localScale = new Vector3(1, 0.05f, 0.02f);

			detectionT = new GameObject("detection").transform;
			detectionT.SetParent(statusBar.transform);
			detectionT.localPosition = new Vector3(0, -0.09f, 0);
			detectionT.localScale = new Vector3(1, 0.05f, 0.02f);
			
			blackT = new GameObject("black").transform;
			blackT.SetParent(statusBar.transform);
			blackT.localPosition = new Vector3(0, 0, 0.03f);
			blackT.localScale = new Vector3(1.05f, 0.25f, 0.01f);
			

		}
		
		


		public override void Update()
		{
			base.Update();
			int allActiveCount = Creature.allActive.Count;
			for (var i = 0; i < allActiveCount; i++)
			{
				var creature = Creature.allActive[i];
				if (creature.isPlayer) continue;

				DrawCreatureStatusBar(creature);
			}

		}

		void DrawCreatureStatusBar(Creature creature)
		{
			var head = creature.animator.GetBoneTransform(HumanBodyBones.Head);
				
			Vector3 headPosition = head.position;
			headPosition.y += height;
			var rotation = Quaternion.LookRotation(headPosition - Player.local.head.transform.position);
			statusBar.transform.SetPositionAndRotation(headPosition, rotation);
			
			UpdateBarTransform(redT, Mathf.Clamp01(creature.currentHealth / creature.maxHealth));
			UpdateBarTransform(blueT, Mathf.Clamp01(creature.mana.currentMana / creature.mana.maxMana));
			
			var moduleDetection = creature.brain.instance.GetModule<BrainModuleDetection>();
			UpdateBarTransform(detectionT, Mathf.Clamp01(moduleDetection.alertednessLevel / moduleDetection.detectAlertednessThreshold));
			
			DrawBar(this.redT, red);
			DrawBar(this.blueT, blue);
			DrawBar(this.detectionT, moduleDetection.alertednessIncreased ? red : green);
			DrawBar(this.blackT, black);
		}
		private void UpdateBarTransform(Transform t, float ratio)
		{
			var redScale = t.localScale;
			redScale.x = ratio;
			t.localScale = redScale;
			var redPos = t.localPosition;
			redPos.x = (ratio - 1f) * 0.5f;
			t.localPosition = redPos;
		}

		void DrawBar(Transform source, Material mat)
		{
			var matrix = Matrix4x4.TRS(source.position, source.rotation, source.lossyScale);
			Graphics.DrawMesh(cubeMesh, matrix,  mat, 0 );
		}

	}
}