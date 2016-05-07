using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSpaceWars.Ships
{
    class TargetDrone
    {
        private string name;
        private int hull;
        private int shields;
        private bool destroyed;
        double[] location;

        public TargetDrone(string name, double[] location)
        {
            this.name = name;
            this.location = location;
            hull = 50;
            shields = 50;
        }

        public List<string> InflictDamage(int damage)
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
