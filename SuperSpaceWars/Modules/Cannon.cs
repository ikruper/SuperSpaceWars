using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSpaceWars.Modules
{
    class Cannon
    {
        private string name;
        private int hull;
        private int ammunition;
        private int max_range;
        private int max_damage;
        private int[,] motion_range;
        private double agility_coefficient;

        //Constructor
        public Cannon(string name, int hull, int ammunition, int max_range, int max_damage, int[,] motion_range, double agility_coefficient)
        {
            this.name = name;
            this.hull = hull;
            this.ammunition = ammunition;
            this.max_range = max_range;
            this.max_damage = max_damage;
            this.motion_range = motion_range;
            this.agility_coefficient = agility_coefficient;
        }

        public string GetName()
        {
            return name;
        }

        public int GetHull()
        {
            return hull;
        }

        public int GetAmmunition()
        {
            return ammunition;
        }

        public int GetMaxRange()
        {
            return max_range;
        }

        //Checks if the cannon can physically fire in that it has ammunition and is not destroyed
        public bool IsReady()
        {
            if (hull > 0 && ammunition > 0)
                return true;
            return false;
        }

        //Checks if the cannon can actually engage a target at a specific bearing
        public bool CanAttack(double[] target_bearing, double range_to_target)
        {
            if (target_bearing[0] >= motion_range[0, 0] && target_bearing[0] <= motion_range[0, 1]
                && target_bearing[1] >= motion_range[1, 0] && target_bearing[1] <= motion_range[1, 1]
                && range_to_target <= max_range)
                return true;
            return false;
        }

        //Determines whether the shot hit and how much damage was dealt
        public int Fire(int hit_threshold)
        {
            Random dice_roll = new Random();
            ammunition--;
            if (dice_roll.Next() * agility_coefficient < hit_threshold)
                return 0;
            int damage = dice_roll.Next(max_damage / 5, max_damage);
            return damage;
        }
    }
}
