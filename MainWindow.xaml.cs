using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("TetrisAssets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("TetrisAssets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Block-Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;
        //private readonly int maxDelay = 800;
        private readonly int minDelay = 75;
        //private readonly int delayDecrease = 25;

        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellsize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellsize,
                        Height = cellsize,
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellsize + 10);
                    Canvas.SetLeft(imageControl, c * cellsize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
            return imageControls;
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0;c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gamestate)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
            LineText.Text = $"Lines: {gameState.LineCount}";

        }

        private async Task GameLoop()
        {
            Draw(gameState);

            int baseDelay = 800; // Starting delay
            int scoreThreshold = 50; // The score threshold for speeding up
            int decreaseInterval = 50; // Amount to decrease delay by

            while (!gameState.GameOver && !gameState.Select)
            {
                int currentDelay = baseDelay - (gameState.Score / scoreThreshold) * decreaseInterval;
                currentDelay = Math.Max(currentDelay, minDelay);
                //int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(currentDelay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            if (gameState.GameOver)
            {
                GameOverMenu.Visibility = Visibility.Visible;
                FinalScoreText.Text = $"Score: {gameState.Score}";
            }
            else if (gameState.Select)
            {
                SelectScreen.Visibility = Visibility.Visible;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                case Key.X:
                    gameState.RotateBlockCW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }
            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(Object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private async void Resume_Click(Object sender, RoutedEventArgs e)
        {
            SelectScreen.Visibility = Visibility.Hidden;
            gameState.Select = false;
            await GameLoop();
        }
    }
}
