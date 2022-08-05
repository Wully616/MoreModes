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
            Debug.Log($"AirDashEnabled");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerControl.local.OnJumpButtonEvent -= OnJumpButtonEvent;
            Debug.Log($"AirDashDisabled");
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
                if (!lm.isJumping && !lm.isGrounded && !isAirDashing)
                {
                    //check if doublejump is enabled, and its not double jumped yet, call that first
                    //We do this because depending on the order air dash and double jump are enabled, one will get the event before the other.
                    //We need to guarantee double jump happens first
                    //Then next time the player presses the jump button, they will air dash
                    Debug.Log($"air dashing");
                    if (DoubleJump.Instance.IsEnabled)
                    {
                        //we just double jumped this frame, so wait until the next time the player presses jump
                        if (!DoubleJump.Instance.IsDoubleJumping)
                        {
                            Debug.Log($"Not airdashing because we haven't double jumped yet");
                            return;
                        }
                        
                    };
                    
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
