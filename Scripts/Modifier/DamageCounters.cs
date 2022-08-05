using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class DamageCounters : ModifierData {
		public static DamageCounters Instance;

		public float counterStayTime = 1;
		public override void Init()
		{
			if (Instance != null) return;
			
			base.Init();
			local = Instance = this;
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			EventManager.onCreatureKill += OnCreatureKill;
			EventManager.onCreatureHit += OnCreatureHit;

		}
		
		protected override void OnDisable()
		{
			base.OnDisable();
			EventManager.onCreatureKill -= OnCreatureKill;
			EventManager.onCreatureHit -= OnCreatureHit;
		}
		
		
		private void OnCreatureHit(Creature creature, CollisionInstance collisionInstance)
		{
			if(creature == Player.currentCreature || !collisionInstance.IsDoneByPlayer() ) return;
			ShowDamage(collisionInstance);
		}
		private void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime) {
			if ( eventTime == EventTime.OnStart  || player || !collisionInstance.IsDoneByPlayer() )
				return;
			ShowDamage(collisionInstance);
		}

	
		private void ShowDamage(CollisionInstance collisionInstance)
		{
			if (collisionInstance?.damageStruct.damage > 0)
			{
				var holder = new GameObject();
				var text = holder.AddComponent<TextMesh>();
				var damage = (int)Mathf.Clamp(collisionInstance.damageStruct.damage, 1, 999999);
				text.text = damage.ToString();
				text.anchor = TextAnchor.MiddleCenter;
				text.transform.position = collisionInstance.contactPoint;
				text.characterSize = 0.03f;
				text.fontSize = 100;
				text.color = Color.Lerp(Color.yellow, Color.red, damage / 50f);
				Level.current.StartCoroutine(MoveText(text, -collisionInstance.contactNormal));
			}
		}

		private IEnumerator MoveText(TextMesh textMesh, Vector3 direction)
		{
			var textMeshTransform = textMesh.transform;
			Vector3 startingPos  = textMeshTransform.position;
			Vector3 finalPos = textMeshTransform.position + (direction * 2);
			float elapsedTime = 0;
			while (elapsedTime < counterStayTime)
			{
				textMeshTransform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / counterStayTime));
				textMeshTransform.rotation = Quaternion.LookRotation(textMeshTransform.position - Player.local.head.transform.position);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			GameObject.Destroy(textMesh.gameObject);
		}
	}
}