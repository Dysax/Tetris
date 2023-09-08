using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tetris.MainWindow;

namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;
        public event EventHandler<AbilitiesEventArgs> OnNewAbilitiesAvailable;



        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }
        

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; set; }
        public bool Select { get; set; }
        public int Score { get; private set; }
        public int LineCount { get; set; }
        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; }
        public Action<GameState> Ability { get; set; }


        public class AbilitiesEventArgs : EventArgs
        {
            public List<GameAbility> Abilities { get; }

            public AbilitiesEventArgs(List<GameAbility> abilities)
            {
                Abilities = abilities;
            }
        }


        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
            Select = false;
        }

        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column ))
                {
                    return false;
                }
            }
            return true;
        }

        public void HoldBlock()
        {
            if (!CanHold)
            {
                return;
            }

            if (HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;
        }

        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }

        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }


        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            int tempScore = GameGrid.ClearFullRows();
            Score += tempScore * 10;
            if ((LineCount % 5) + tempScore >= 5)
            {
                Select = true;

                //get random ability for the select screen
                var randomAbility = AbilityManager.GetRandomAbilities(3);


                List<GameAbility> currentAbilities = AbilityManager.GetRandomAbilities(3);
                OnNewAbilitiesAvailable?.Invoke(this, new AbilitiesEventArgs(currentAbilities));

            }

            LineCount += tempScore;
            

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }

            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

        public void TriggerNewAbilitiesAvailable(List<GameAbility> newAbilities)
        {
            OnNewAbilitiesAvailable?.Invoke(this, new AbilitiesEventArgs(newAbilities));
        }

    }
}
