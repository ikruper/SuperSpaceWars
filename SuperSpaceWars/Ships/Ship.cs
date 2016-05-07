using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSpaceWars.Modules;

namespace SuperSpaceWars.Ships
{
    abstract class Ship
    {
        protected string name;
        protected int hull;
        protected int shields;
        protected double[] location;
        protected int[] course;
        protected bool destroyed;
        protected List<Cannon> armament;

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

        //these methods are called on own ship
        public abstract List<Cannon> GetReadyCannons();
        public abstract List<Cannon> GetCanAttackCannons(Ship enemy_ship);
        public abstract List<string> FireCannon(Ship enemy_ship, Cannon own_cannon, int hit_threshold);

        //these methods are called on an enemy ships
        public abstract int CalculateHitThreshold(Ship enemy_ship, Cannon enemy_cannon);
        public abstract List<string> InflictDamage(int damage);
    }
}
