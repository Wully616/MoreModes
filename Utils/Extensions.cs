using System;
using System.IO;
using System.Reflection;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wully.Utils {
	public static class Extensions {

		public static string GetModPath( this Object thisObject ) {
			Type type = thisObject.GetType();
			string assemblyFullName = type.Assembly.GetName().Name;
			return FileManager.GetFullPath(FileManager.Type.JSONCatalog,
				FileManager.Source.Mods, assemblyFullName);
		}

		public static bool GetOptionAsBool( this Level level, string optionId ) {
			if (string.IsNullOrEmpty(optionId)) {
				Debug.LogWarning("Option id is null - make sure SetId() is called on LevelModuleOptional");
				return false;
			}
			//if the config option is there,  enable/disable based on its value
			if ( level.options != null && level.options.TryGetValue(optionId, out double num) ) {
				return num > 0; //0 is off, 1 is on
			}

			return false;
		}

		public static bool GetOptionAsBool(this Level level, string optionId, bool defaultValue) {
			//if the config option is there,  enable/disable based on its value
			if (level.options != null && level.options.TryGetValue(optionId, out double num)) {
				defaultValue = num > 0; //0 is off, 1 is on
			}

			return defaultValue;
		}

		public static string GetManifest(Type type) {
			string assemblyFullName = type.Assembly.GetName().Name;
			string manifestName = assemblyFullName + "\\manifest.json";
			string manifest = File.ReadAllText(FileManager.GetFullPath(FileManager.Type.JSONCatalog,
				FileManager.Source.Mods, manifestName));
			return manifest;
		}

		public static bool IsNull(this Object gameObject) {
			return gameObject is null;
		}

		public static bool IsNotNull(this Object gameObject) {
			return !IsNull(gameObject);
		}

		public static bool ObjectEqual(this Object gameObject, Object otherObject) {
			return ReferenceEquals(gameObject, otherObject);
		}

		public static T Read<T>(this object source, string fieldName) {
			return (T) source.GetType()
				.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(source);
		}

		public static void Set<T>(this object source, string fieldName, T val) {
			source.GetType()
				.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
				.SetValue(source, val);
		}

		internal static void Raise(this object source, string eventName, object eventArgs) {
			var eventDelegate = (MulticastDelegate) source.GetType()
				.GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(source);
			if (eventDelegate != null) {
				eventDelegate.DynamicInvoke(eventArgs);
			}
		}

		public static void Enable(this EffectInstance instance) {
			for (int i = instance.effects.Count - 1; i >= 0; i--) {
				instance.effects[i].gameObject.SetActive(true);
			}
		}

		public static void Disable(this EffectInstance instance) {
			for (int i = instance.effects.Count - 1; i >= 0; i--) {
				instance.effects[i].gameObject.SetActive(false);
			}
		}

		public static void SetRotation(this EffectInstance instance, Quaternion rotation) {
			for (int i = instance.effects.Count - 1; i >= 0; i--) {
				instance.effects[i].transform.rotation = rotation;
			}
		}

		public static void SetPosition(this EffectInstance instance, Vector3 position) {
			for (int i = instance.effects.Count - 1; i >= 0; i--) {
				instance.effects[i].transform.position = position;
			}
		}

		public static RagdollPart GetRagdollPartByName(string name) {
			if (Player.local && Player.local.creature) {
				foreach (RagdollPart ragdollPart in Player.local.creature.ragdoll.parts) {
					if (ragdollPart.name.Equals(name)) {
						return ragdollPart;
					}
				}
			}

			Debug.Log("Couldn't find ragdoll part: " + name);
			return null;
		}
	}
}