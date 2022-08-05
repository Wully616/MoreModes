using ThunderRoad;
using UnityEngine;

namespace Wully.MoreModes
{
    public class Swimming : ModifierData
    {
        public static Swimming Instance;
        public float swimForce = 1f;
        public override void Init()
        {
            if (Instance != null) return;
            base.Init();
            Instance = this;
            local = this;
        }
        
        public override void Update()
        {
            UpdateSwim();
        }
        public float currentSwimSwingRatio;
        void UpdateSwim()
        {
            if (Player.local && Player.local.creature)
            {
                if (Player.local.creature.waterHandler.inWater)
                {
                    //apply a force from the players hands based on speed?
                    float swimSpeedRatioL = ThunderRoad.Utils.CalculateRatio(Mathf.Abs(PlayerControl.handLeft.GetHandVelocity().y), 0.5f, 2, 0, 1);
                    float swimSpeedRatioR =ThunderRoad.Utils.CalculateRatio(Mathf.Abs(PlayerControl.handRight.GetHandVelocity().y), 0.5f, 2, 0, 1);
                    currentSwimSwingRatio = Mathf.Lerp(currentSwimSwingRatio, (swimSpeedRatioL + swimSpeedRatioR) / 2, Time.deltaTime * 2);
                    var force = swimForce * currentSwimSwingRatio * Time.deltaTime;
                    var vel = Player.local.head.transform.forward * force;
                    Player.local.locomotion.rb.AddForce(vel, ForceMode.VelocityChange);

                }
            }
        }
    }
}
