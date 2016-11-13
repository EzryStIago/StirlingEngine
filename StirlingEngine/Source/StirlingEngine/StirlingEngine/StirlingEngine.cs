using UnityEngine;         // Always needed
using Verse;               // RimWorld universal objects are here (like 'Building')
using RimWorld;


namespace StirlingEngine
{
    public class Building_StirlingEngine : Building_TempControl
    {
        //Power generation calculations aided by Jean-Pierre Van Dormael's worksheet available at "www.moteurstirling.com/pdf/calcul_uk.xls"
        private const float conductivity = 0.5f;
        private const float powerSlope = 3.9f;
        private const float powerYIntercept = 53.9f;

        //private const float HeatOutputMultiplier = 1.25f;

        //private const float EfficiencyLossPerDegreeDifference = 0.0076923077f;

        public override void TickRare()
        {
            CompProperties_Power props = this.compPowerTrader.Props;

            if (this.compPowerTrader.PowerOn)
            {
                IntVec3 room1 = base.Position + IntVec3.South.RotatedBy(base.Rotation);
                IntVec3 room2 = base.Position + IntVec3.North.RotatedBy(base.Rotation);
                bool flag = false; //True iff both sides are passable && there is a temp difference between them
                if (!room2.Impassable() && !room1.Impassable())
                {
                    float temperature2 = room2.GetTemperature();
                    float temperature1 = room1.GetTemperature();

                    float dTemp = temperature2 - temperature1; //difference in temperatures

                    flag = !Mathf.Approximately(dTemp, 0f);
                    if (flag)
                    {
                        float rate = conductivity * dTemp;
                        if (rate > 12f) //never as effective as a vent (rate = 14f)
                            rate = 12f;
                        GenTemperature.EqualizeTemperaturesThroughBuilding(this, rate);

                        this.compPowerTrader.PowerOutput = powerSlope * dTemp + powerYIntercept;
                    }
                    else
                    {
                        this.compPowerTrader.PowerOutput = 0;
                    }
                }
            }
        }
    }
}


