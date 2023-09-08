using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class AbilityManager
    {
        private static List<GameAbility> AvailableAbilities = new List<GameAbility>
        {
            new GameAbility
            {
                Description = "TEST - TEST - TEST - TEST - TEST",
                Ability = (GameState gameState) =>
                {
                    // apply ability here based on your existing GameState and GameAbility classes
                }
            },
            new GameAbility
            {
                Description = "TEST - TEST - TEST",
                Ability = (GameState gameState) =>
                {
                    // apply ability here based on your existing GameState and GameAbility classes
                }
            },
            new GameAbility
            {
                Description = "TEST",
                Ability = (GameState gameState) =>
                {
                    // apply ability here based on your existing GameState and GameAbility classes
                }
            },
            // Add other abilities...
        };

        private static Random rng = new Random();

        public static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static List<GameAbility> GetRandomAbilities(int number)
        {
            var shuffledAbilities = new List<GameAbility>(AvailableAbilities);
            Shuffle(shuffledAbilities);
            return shuffledAbilities.Take(number).ToList();
        }

    }
}
