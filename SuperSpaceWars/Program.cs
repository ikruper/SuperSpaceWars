using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSpaceWars.Ships;
using SuperSpaceWars.Modules;

namespace SuperSpaceWars
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test cannon aim
            Frigate my_frigate = new Frigate("my_frigate", new double[3] { 1, 1, 1 }, new int[2] { 0, 0 });
            Frigate enemy_frigate = new Frigate("enemy_frigate", new double[3] { 2, 2, 2 }, new int[2] { 60, 60 });

            List<Cannon> can_attack_cannons = my_frigate.GetCanAttackCannons(enemy_frigate);
            if (can_attack_cannons.Count > 0)
                Console.Write("Hit Chance: " + (100 - my_frigate.CalculateHitThreshold(enemy_frigate, can_attack_cannons.First())) + "%");

            Console.WriteLine("\nPress any key to exit...");
            ConsoleKeyInfo KInfo = Console.ReadKey(true);
        }
    }
}
