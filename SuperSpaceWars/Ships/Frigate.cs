using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSpaceWars.Modules;

namespace SuperSpaceWars.Ships
{
    class Frigate
    {
        private string name;
        private int hull;
        private int shields;
        private double[] location;
        private int[] course;
        private bool destroyed;
        private List<Cannon> armament;

        public Frigate(string name, double[] location, int[] course)
        {
            this.name = name;
            this.location = location;
            this.course = course;
            hull = 100;
            shields = 50;
            destroyed = false;
            armament = new List<Cannon>()
            {
                new Cannon("top_155mm", 25, 20, 16, 30, new int[,] { { -180, 180 }, { 10, 170 } }, 0.80),
                new Cannon("bottom_155mm", 25, 20, 16, 30, new int[,] { { -180, 180 }, { -170, -10 } }, 0.80)
            };
        }

        public string GetName()
        {
            return name;
        }

        public int GetHull()
        {
            return hull;
        }

        public int GetShields()
        {
            return shields;
        }

        public double[] GetLocation()
        {
            return location;
        }

        public int[] GetCourse()
        {
            return course;
        }

        public List<Cannon> GetReadyCannons()
        {
            List<Cannon> ready_cannons = new List<Cannon>();
            foreach (Cannon cannon in armament)
            {
                if (cannon.IsReady())
                    ready_cannons.Add(cannon);
            }
            return ready_cannons;
        }

        public List<Cannon> GetCanAttackCannons(Frigate enemy_frigate)
        {
            double[] relative_bearing = new double[2];
            double own_x = location[0];
            double own_y = location[1];
            double own_z = location[2];
            double enemy_x = enemy_frigate.GetLocation()[0];
            double enemy_y = enemy_frigate.GetLocation()[1];
            double enemy_z = enemy_frigate.GetLocation()[2];
            double range_to_target = Math.Sqrt(Math.Pow(enemy_x - own_x, 2) + Math.Pow(enemy_y - own_y, 2) + Math.Pow(enemy_z - own_z, 2));

            //perform initial angular course calculation
            double hypotenuse = Math.Sqrt(Math.Pow(enemy_x - own_x, 2) + Math.Pow(enemy_y - own_y, 2));
            relative_bearing[0] = Math.Atan((enemy_y - own_y) / (enemy_x - own_x));
            relative_bearing[1] = Math.Atan((enemy_z - own_z) / hypotenuse);

            //transform course along XY plane
            if (course[0] != 0)
            {
                enemy_x = hypotenuse * Math.Cos(relative_bearing[0] - (course[0] * Math.PI / 180));
                enemy_y = hypotenuse * Math.Sin(relative_bearing[0] - (course[0] * Math.PI / 180));
                relative_bearing[0] = Math.Atan((enemy_y - own_y) / (enemy_x - own_x));
                relative_bearing[1] = Math.Atan((enemy_z - own_z) / hypotenuse);
            }

            //transform course along XZ plane
            if (course[1] != 0)
            {
                hypotenuse = Math.Sqrt(Math.Pow(enemy_x - own_x, 2) + Math.Pow(enemy_z - own_z, 2));
                enemy_x = hypotenuse * Math.Cos(relative_bearing[1] - (course[1] * Math.PI / 180));
                enemy_z = hypotenuse * Math.Sin(relative_bearing[1] - (course[1] * Math.PI / 180));
                relative_bearing[0] = Math.Atan((enemy_y - own_y) / (enemy_x - own_x));
                relative_bearing[1] = Math.Atan((enemy_z - own_z) / hypotenuse);
            }

            //convert bearings into degrees
            relative_bearing[0] *= 180 / Math.PI;
            relative_bearing[1] *= 180 / Math.PI;

            //format cartesian quadrants II and III
            if (relative_bearing[0] > 0 && enemy_x - own_x < 0 && enemy_y - own_y < 0)
                relative_bearing[0] -= 180;
            if (relative_bearing[0] < 0 && enemy_x - own_x < 0 && enemy_y - own_y > 0)
                relative_bearing[0] += 180;
            if (relative_bearing[1] > 0 && enemy_x - own_x < 0 && enemy_z - own_z < 0)
                relative_bearing[1] -= 180;
            if (relative_bearing[1] < 0 && enemy_x - own_x < 0 && enemy_z - own_z > 0)
                relative_bearing[1] += 180;

            List<Cannon> can_attack_cannons = new List<Cannon>();
            foreach (Cannon cannon in armament)
            {
                if (cannon.CanAttack(relative_bearing, range_to_target))
                    can_attack_cannons.Add(cannon);
            }
            return can_attack_cannons;
        }

        public int CalculateHitThreshold(Frigate enemy_frigate, Cannon own_cannon)
        {
            double[] relative_bearing = new double[2];
            double own_x = location[0];
            double own_y = location[1];
            double own_z = location[2];
            double enemy_x = enemy_frigate.GetLocation()[0];
            double enemy_y = enemy_frigate.GetLocation()[1];
            double enemy_z = enemy_frigate.GetLocation()[2];
            double range_to_target = Math.Sqrt(Math.Pow(own_x - enemy_x, 2) + Math.Pow(own_y - enemy_y, 2) + Math.Pow(own_z - enemy_z, 2));

            //perform transform operation from the perspective of the target ship
            double hypotenuse = Math.Sqrt(Math.Pow(own_x - enemy_x, 2) + Math.Pow(own_y - enemy_y, 2));
            relative_bearing[0] = Math.Atan((own_y - enemy_y) / (own_x - enemy_x));
            relative_bearing[1] = Math.Atan((own_z - enemy_z) / hypotenuse);

            //transform course along XY plane
            if (enemy_frigate.GetCourse()[0] != 0)
            {
                enemy_x = hypotenuse * Math.Cos(relative_bearing[0] - (enemy_frigate.GetCourse()[0] * Math.PI / 180));
                enemy_y = hypotenuse * Math.Sin(relative_bearing[0] - (enemy_frigate.GetCourse()[0] * Math.PI / 180));
                relative_bearing[0] = Math.Atan((own_y - enemy_y) / (own_x - enemy_x));
                relative_bearing[1] = Math.Atan((own_z - enemy_z) / hypotenuse);
            }

            //transform course along XZ plane
            if (enemy_frigate.GetCourse()[1] != 0)
            {
                hypotenuse = Math.Sqrt(Math.Pow(own_x - enemy_x, 2) + Math.Pow(own_z - enemy_z, 2));
                enemy_x = hypotenuse * Math.Cos(relative_bearing[1] - (enemy_frigate.GetCourse()[1] * Math.PI / 180));
                enemy_z = hypotenuse * Math.Sin(relative_bearing[1] - (enemy_frigate.GetCourse()[1] * Math.PI / 180));
                relative_bearing[0] = Math.Atan((own_y - enemy_y) / (own_x - enemy_x));
                relative_bearing[1] = Math.Atan((own_z - enemy_z) / hypotenuse);
            }

            //convert bearings into degrees
            relative_bearing[0] *= 180 / Math.PI;
            relative_bearing[1] *= 180 / Math.PI;

            //format cartesian quadrants II and III
            if (relative_bearing[0] > 0 && own_x - enemy_x < 0 && own_y - enemy_y < 0)
                relative_bearing[0] -= 180;
            if (relative_bearing[0] < 0 && own_x - enemy_x < 0 && own_y - enemy_y > 0)
                relative_bearing[0] += 180;
            if (relative_bearing[1] > 0 && own_x - enemy_x < 0 && own_z - enemy_z < 0)
                relative_bearing[1] -= 180;
            if (relative_bearing[1] < 0 && own_x - enemy_x < 0 && own_z - enemy_z > 0)
                relative_bearing[1] += 180;

            //substitute into ship hit equations
            double theta_c = (1.0 / 6.0) * Math.Cos(2 * relative_bearing[0] * Math.PI / 180.0 + Math.PI) + (1.0 / 3.0);
            double phi_c = (1.0 / 6.0) * Math.Cos(2 * relative_bearing[1] * Math.PI / 180.0 + Math.PI) + (1.0 / 3.0);

            int hit_threshold = Convert.ToInt32((range_to_target * 100) 
                / own_cannon.GetMaxRange() 
                / Math.Sqrt(Math.Pow(theta_c, 2) + Math.Pow(phi_c, 2)));
            if (hit_threshold > 100)
                hit_threshold = 100;
            return hit_threshold;
        }

        public List<string> FireCannon(Frigate enemy_frigate, Cannon own_cannon, int hit_threshold)
        {
            int damage = own_cannon.Fire(hit_threshold);
            return enemy_frigate.InflictDamage(damage);
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
            else if (shields > 0 && shields - damage <= 0)
            {
                report.Add("Enemy shields down");
                damage -= shields;
                shields = 0;
            }

            if (shields == 0 && hull - damage > 0)
                hull -= damage;
            else if (shields == 0 && hull - damage <= 0)
            {
                report.Add("Enemy ship destroyed");
                destroyed = true;
            }

            return report;
        }
    }
}
