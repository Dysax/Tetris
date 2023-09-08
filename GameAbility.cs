using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameAbility
    {
        public string Description { get; set; }
        public Action<GameState> Ability { get; set; }
    }
}
