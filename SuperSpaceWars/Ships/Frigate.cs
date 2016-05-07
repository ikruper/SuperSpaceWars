using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSpaceWars.Modules;

namespace SuperSpaceWars.Ships
{
    class Frigate : Ship
    {
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

        

        public override List<Cannon> GetReadyCannons()
        {
            List<Cannon> ready_cannons = new List<Cannon>();
            foreach (Cannon cannon in armament)
            {
                if (cannon.IsReady())
                    ready_cannons.Add(cannon);
            }
            return ready_cannons;
        }

        public override List<Cannon> GetCanAttackCannons(Ship enemy_ship)
        {
            double[] relative_bearing = new double[2];
            double own_x = location[0];
            double own_y = location[1];
            double own_z = location[2];
            double enemy_x = enemy_ship.GetLocation()[0];
            double enemy_y = enemy_ship.GetLocation()[1];
            double enemy_z = enemy_ship.GetLocation()[2];
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

        public override List<string> FireCannon(Ship enemy_ship, Cannon own_cannon, int hit_threshold)
        {
            int damage = own_cannon.Fire(hit_threshold);
            return enemy_ship.InflictDamage(damage);
        }

        public override int CalculateHitThreshold(Ship enemy_ship, Cannon enemy_cannon)
        {
            double[] relative_bearing = new double[2];
            double own_x = location[0];
            double own_y = location[1];
            double own_z = location[2];
            double enemy_x = enemy_ship.GetLocation()[0];
            double enemy_y = enemy_ship.GetLocation()[1];
            double enemy_z = enemy_ship.GetLocation()[2];
            double range_to_target = Math.Sqrt(Math.Pow(own_x - enemy_x, 2) + Math.Pow(own_y - enemy_y, 2) + Math.Pow(own_z - enemy_z, 2));

            //perform transform operation from the perspective of the target ship
            double hypotenuse = Math.Sqrt(Math.Pow(own_x - enemy_x, 2) + Math.Pow(own_y - enemy_y, 2));
            relative_bearing[0] = Math.Atan((own_y - enemy_y) / (own_x - enemy_x));
            relative_bearing[1] = Math.Atan((own_z - enemy_z) / hypotenuse);

            //transform course along XY plane
            if (enemy_ship.GetCourse()[0] != 0)
            {
                enemy_x = hypotenuse * Math.Cos(relative_bearing[0] - (enemy_ship.GetCourse()[0] * Math.PI / 180));
                enemy_y = hypotenuse * Math.Sin(relative_bearing[0] - (enemy_ship.GetCourse()[0] * Math.PI / 180));
                relative_bearing[0] = Math.Atan((own_y - enemy_y) / (own_x - enemy_x));
                relative_bearing[1] = Math.Atan((own_z - enemy_z) / hypotenuse);
            }

            //transform course along XZ plane
            if (enemy_ship.GetCourse()[1] != 0)
            {
                hypotenuse = Math.Sqrt(Math.Pow(own_x - enemy_x, 2) + Math.Pow(own_z - enemy_z, 2));
                enemy_x = hypotenuse * Math.Cos(relative_bearing[1] - (enemy_ship.GetCourse()[1] * Math.PI / 180));
                enemy_z = hypotenuse * Math.Sin(relative_bearing[1] - (enemy_ship.GetCourse()[1] * Math.PI / 180));
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
                / enemy_cannon.GetMaxRange() 
                / Math.Sqrt(Math.Pow(theta_c, 2) + Math.Pow(phi_c, 2)));
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
