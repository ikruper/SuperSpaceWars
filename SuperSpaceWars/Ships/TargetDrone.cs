using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSpaceWars.Modules;

namespace SuperSpaceWars.Ships
{
    class TargetDrone : Ship
    {

        public TargetDrone(string name, double[] location)
        {
            this.name = name;
            this.location = location;
            course = new int[2] { 0, 0 };
            hull = 50;
            shields = 50;
            armament = null;
        }

        public override List<Cannon> GetReadyCannons()
        {
            return new List<Cannon>();
        }

        public override List<Cannon> GetCanAttackCannons(Ship enemy_ship)
        {
            return new List<Cannon>();
        }

        public override List<string> FireCannon(Ship enemy_ship, Cannon own_cannon, int hit_threshold)
        {
            return new List<string>();
        }

        public override int CalculateHitThreshold(Ship enemy_ship, Cannon enemy_cannon)
        {
            double own_x = location[0];
            double own_y = location[1];
            double own_z = location[2];
            double enemy_x = enemy_ship.GetLocation()[0];
            double enemy_y = enemy_ship.GetLocation()[1];
            double enemy_z = enemy_ship.GetLocation()[2];
            double range_to_target = Math.Sqrt(Math.Pow(own_x - enemy_x, 2) + Math.Pow(own_y - enemy_y, 2) + Math.Pow(own_z - enemy_z, 2));
            int hit_threshold = Convert.ToInt32((range_to_target * 100) / enemy_cannon.GetMaxRange());
            if (hit_threshold > 100)
                hit_threshold = 100;
            if (hit_threshold < 0)
                hit_threshold = 0;
            return hit_threshold;
        }

        public override List<string> InflictDamage(int damage)
        {
            List<string> report = new List<string>();

            if (damage == 0)
                report.Add("Shot missed");
            else
                report.Add("Shot hit");

            if (shields > 0 && shields - damage > 0)
                shields -= damage;
            if (shields > 0 && shields - damage <= 0)
            {
                report.Add("Enemy shields down");
                damage -= shields;
                shields = 0;
            }
            if (shields == 0 && hull - damage > 0)
                hull -= damage;
            if (shields == 0 && hull - damage <= 0)
            {
                report.Add("Enemy ship destroyed");
                destroyed = true;
            }
            return report;
        }
    }
}
