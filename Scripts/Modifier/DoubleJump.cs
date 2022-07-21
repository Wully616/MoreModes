using System.Collections;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class DoubleJump : ModifierData {
		
		public static DoubleJump Instance;
		private bool isDoubleJumping;
		public bool IsDoubleJumping => isDoubleJumping;
		
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				local = this;
			}
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			PlayerControl.local.OnJumpButtonEvent += OnJumpButtonEvent;
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			PlayerControl.local.OnJumpButtonEvent -= OnJumpButtonEvent;
		}
		
		private void OnJumpButtonEvent(bool active, EventTime eventTime)
		{
			if(eventTime == EventTime.OnStart) return;
			if(!active) return;
			
			if (Player.local.locomotion != null)
			{
				var lm = Player.local.locomotion;
				if (lm.isGrounded)
				{
					isDoubleJumping = false;
				}
				
				//if the player isnt jumping and not grounded
				if (!isDoubleJumping && !lm.isJumping && !lm.isGrounded)
				{
					Vector3 velocity = lm.rb.velocity;
					velocity.y = 0f;
					lm.rb.velocity = velocity;
					isDoubleJumping = true;
					
					//tell the locomotion its not jumping and is on the ground, so it will execute a normal jump
					lm.isGrounded = true;
					//Then do the double jump
					lm.Jump(true);
					lm.isGrounded = false;
				}
			}
		}

		
		
	}
}