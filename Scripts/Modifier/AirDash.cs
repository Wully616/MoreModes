using ThunderRoad;
using UnityEngine;

namespace Wully.MoreModes
{
    public class AirDash : ModifierData
    {
        public float dashForce = 10f;
        public static AirDash Instance;
        private bool isAirDashing;
        
        public bool IsAirDashing => isAirDashing;
        
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

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerControl.local.OnJumpButtonEvent -= OnJumpButtonEvent;
        }

        private void OnJumpButtonEvent(bool active, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            if (!active) return;

            if (Player.local.locomotion != null)
            {
                var lm = Player.local.locomotion;
                if (lm.isGrounded)
                {
                    isAirDashing = false;
                }

                //if the player isnt jumping and not grounded
                if (!lm.isJumping && !lm.isGrounded)
                {
                    //check if doublejump is enabled, we only want to airdash after the double jump
                    if(DoubleJump.Instance.IsEnabled && !DoubleJump.Instance.IsDoubleJumping ) return;
                    
                    //Zero off velocity
                    Vector3 velocity = lm.rb.velocity;
                    velocity.y = 0f;
                    lm.rb.velocity = velocity;
                    isAirDashing = true;

                    //Apply force in look direction
                    var force = Player.local.head.transform.forward * dashForce;
                    Player.local.locomotion.rb.AddForce(force, ForceMode.VelocityChange);
                }
            }
        }
    }
}
