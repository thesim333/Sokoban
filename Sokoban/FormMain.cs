using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameGlobals;

namespace Sokoban
{
    public partial class FormMain : Form, IView
    {
        protected SokobanController Ctrl;
        protected Graphics MyGraphics;
        protected const int SQUARESIDESIZE = 40;
        protected const int LEVELMARGIN = 10;
        protected const int IMAGESIZE = 38;
        protected const int CTRLBTNHEIGHT = 50;
        protected const int CTRLBTNWIDTH = 100;
        protected const int CTRLBTNSPACEBTWN = 5;
        protected const int LEVELDESIGNBTNSIZE = 40;
        protected const int LEVELDESIGNSPACEBTWN = 1;
        protected const int LEVELDESIGNLEFTMARGIN = LEVELMARGIN * 2 + CTRLBTNWIDTH;
        protected ImageHandler IH;
        protected RadioButton[] partsSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormMain"/> class as an IView.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            MyGraphics = this.CreateGraphics();
            IH = new ImageHandler();
            this.AutoSize = true;
            MakeLevelDesignRadioPartControl();
        }

        /// <summary>
        /// Handles the Shown event of the FormMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FormMain_Shown(object sender, EventArgs e)
        {
            DisplayMain();
        }

        /// <summary>
        /// Adds the controller.
        /// </summary>
        /// <param name="ctrl">The controller.</param>
        public void AddController(SokobanController ctrl)
        {
            Ctrl = ctrl;
        }

        /// <summary>
        /// Resets the form to empty white.
        /// </summary>
        protected void ResetForm()
        {
            this.Controls.Clear();
            MyGraphics.Clear(Color.White);
        }

        /// <summary>
        /// Changes the view to the designer with a new a bew empty level.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The cols.</param>
        public void DesignerNewLevel(int rows, int cols)
        {
            ResetForm();
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    CreateLevelGridButton(r, c, Parts.Empty);
                }
            }

            for (int i = 0; i < partsSelector.Length; i++)
            {
                this.Controls.Add(partsSelector[i]);
            }
            DesignerButtons();
        }

        /// <summary>
        /// Changes the view to the designer waiting for the level to be loaded from a file.
        /// </summary>
        public void DesignerLoadLevel()
        {
            ResetForm();
            DesignerButtons();
        }

        /// <summary>
        /// Creates the control buttons for the designer.
        /// </summary>
        protected void DesignerButtons()
        {
            CreateControlButton("New Level", 0, levelDesignerNew_buttonClick);
            CreateControlButton("Save Level", 1, levelDesignerSave_buttonClick);
            CreateControlButton("Load Level", 2, levelDesignerLoad_buttonClick);
            CreateControlButton("Check Level", 3, checkDesignerLevel_buttonClick);
            CreateControlButton("Close Designer", 4, levelDesignerClose_buttonClick);
        }

        /// <summary>
        /// Creates the control buttons for the game.
        /// </summary>
        protected void GameButtons()
        {
            CreateControlButton("Restart Game", 0, restartGame_buttonClick);
            CreateControlButton("Load New Level", 1, gameLoad_buttonClick);
            CreateControlButton("Load Game State", 2, gameLoadState_buttonClick);
            CreateControlButton("Save Game State", 3, gameSaveState_buttonClick);
            CreateControlButton("Close Game", 4, gameClose_buttonClick);
            CreateControlButton("Undo", 5, undo_buttonClick);
        }

        /// <summary>
        /// Setup the view for the game
        /// </summary>
        /// <param name="moves">The moves from the game to display</param>
        public void GameSetup(int moves)
        {
            ResetForm();
            GameButtons();
            MakeMoveLabel();
            SetMoves(moves);
            CreateMoveButton("W", 33, 360, moveUp_buttonClick);
            CreateMoveButton("A", 10, 402, moveLeft_buttonClick);
            CreateMoveButton("S", 33, 444, moveDown_buttonClick);
            CreateMoveButton("D", 56, 402, moveRight_buttonClick);
            this.KeyPress -= FormMain_KeyPress;
            this.KeyPress += FormMain_KeyPress;
        }

        /// <summary>
        /// Draws a game position with the image representing the part from that position in the game.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="part">The part.</param>
        public void SetGamePosition(int row, int col, Parts part)
        {
            int rectCol = LEVELMARGIN + 10 + CTRLBTNWIDTH + SQUARESIDESIZE * col; //10 is the space between buttons and the grid
            int rectRow = LEVELMARGIN + SQUARESIDESIZE * row;
            MyGraphics.DrawRectangle(Pens.Black, new Rectangle(rectCol, rectRow, SQUARESIDESIZE, SQUARESIDESIZE));
            Rectangle inner = new Rectangle(rectCol + 1, rectRow + 1, IMAGESIZE, IMAGESIZE);
            MyGraphics.FillRectangle(Brushes.LightGray, inner);
            if (part != Parts.Empty)
            {
                MyGraphics.DrawImage(IH.GetMyPart(part), inner);
            }
        }

        /// <summary>
        /// Creates the main display.
        /// </summary>
        public void DisplayMain()
        {
            MyGraphics.Clear(Color.White);
            Controls.Clear();
            CreateControlButton("Game - Load Level", 0, gameLoad_buttonClick);
            CreateControlButton("Designer - New Level", 1, levelDesignerNew_buttonClick);
            CreateControlButton("Designer - Load Level", 2, levelDesignerLoad_buttonClick);
        }

        /// <summary>
        /// Makes the label showing the moves made in the game.
        /// </summary>
        protected void MakeMoveLabel()
        {
            Label moves = new Label();
            moves.Name = "lblMoves";
            moves.Location = new Point(LEVELMARGIN, 337);
            moves.Font = new Font("Rockwell", 14);
            moves.ForeColor = Color.Black;
            this.Controls.Add(moves);
        }

        /// <summary>
        /// Displays the current move count from game.
        /// </summary>
        /// <param name="moves">The moves.</param>
        public void SetMoves(int moves)
        {
            Label lblMoves = this.Controls.Find("lblMoves", false).FirstOrDefault() as Label;
            string x = "Moves:" + moves.ToString();
            lblMoves.Text = x;
        }

        /// <summary>
        /// Finishes the game. Changes the view to only show buttons that can start a new game.
        /// </summary>
        /// <param name="moves">The moves from the finish of the game.</param>
        public void FinishGame(int moves)
        {
            this.KeyPress -= FormMain_KeyPress;
            this.Controls.Clear();
            CreateControlButton("Restart Game", 0, restartGame_buttonClick);
            CreateControlButton("Load New Level", 1, gameLoad_buttonClick);
            CreateControlButton("Load Game State", 2, gameLoadState_buttonClick);
            SetMoves(moves);
        }

        /// <summary>
        /// Creates a level grid button.
        /// Displays the image representing the part.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="part">The part.</param>
        public void CreateLevelGridButton(int row, int col, Parts part)
        {
            Point p = new Point(LEVELDESIGNLEFTMARGIN + col * LEVELDESIGNBTNSIZE,
                50 + row * LEVELDESIGNBTNSIZE);
            Button newButton = new Button();
            newButton.Name = String.Format("{0}_{1}", row, col);
            newButton.Visible = true;
            newButton.Width = LEVELDESIGNBTNSIZE;
            newButton.Height = LEVELDESIGNBTNSIZE;
            newButton.Location = p;
            newButton.Click += new EventHandler(levelGrid_buttonClick);
            if (part != Parts.Empty)
            {
                newButton.BackgroundImage = IH.GetMyPart(part);
                newButton.BackgroundImageLayout = ImageLayout.Stretch;
            }
            this.Controls.Add(newButton);
        }

        /// <summary>
        /// Creates a move button for moving the player.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="myClick">What happens when the button is clicked.</param>
        protected void CreateMoveButton(string text, int x, int y, EventHandler myClick)
        {
            Button newButton = new Button();
            newButton.Text = text;
            newButton.Width = 40;
            newButton.Height = 40;
            newButton.Location = new Point(x, y);
            newButton.Click += new EventHandler(myClick);
            this.Controls.Add(newButton);
        }

        /// <summary>
        /// Creates a control button.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="y">The y position.</param>
        /// <param name="myClick">What happens when the button is clicked.</param>
        protected void CreateControlButton(string text, int y, EventHandler myClick)
        {
            Point p = new Point(LEVELMARGIN, GetControlButtonY(y));
            Button newButton = new Button();
            newButton.Text = text;
            newButton.Visible = true;
            newButton.Width = CTRLBTNWIDTH;
            newButton.Height = CTRLBTNHEIGHT;
            newButton.Location = p;
            newButton.Click += new EventHandler(myClick);
            this.Controls.Add(newButton);
        }

        /// <summary>
        /// Gets the control button y position.
        /// </summary>
        /// <param name="number">Which button this is for.</param>
        /// <returns>The y position calculation</returns>
        protected int GetControlButtonY(int number)
        {
            return LEVELMARGIN + number * (CTRLBTNHEIGHT + CTRLBTNSPACEBTWN);
        }

        /// <summary>
        /// Gets the selected radio button.
        /// </summary>
        /// <returns>Selected radio button</returns>
        protected RadioButton GetSelectedRB()
        {
            foreach (RadioButton rb in partsSelector)
            {
                if (rb.Checked == true)
                {
                    return rb;
                }
            }
            return null;
        }

        /// <summary>
        /// Makes the level design radio button control.
        /// </summary>
        protected void MakeLevelDesignRadioPartControl()
        {
            partsSelector = new RadioButton[7]; //empty, wall, player, playergoal, goal, block, blockgoal
            partsSelector[0] = MakeLevelDesignRadioButton(IH.Empty, Parts.Empty, 0);
            partsSelector[1] = MakeLevelDesignRadioButton(IH.Wall, Parts.Wall, 1);
            partsSelector[2] = MakeLevelDesignRadioButton(IH.Player, Parts.Player, 2);
            partsSelector[3] = MakeLevelDesignRadioButton(IH.PlayerGoal, Parts.PlayerOnGoal, 3);
            partsSelector[4] = MakeLevelDesignRadioButton(IH.Block, Parts.Block, 4);
            partsSelector[5] = MakeLevelDesignRadioButton(IH.BlockGoal, Parts.BlockOnGoal, 5);
            partsSelector[6] = MakeLevelDesignRadioButton(IH.Goal, Parts.Goal, 6);
            partsSelector[0].Checked = true;
        }

        /// <summary>
        /// Makes a level design RadioButton.
        /// </summary>
        /// <param name="imagePart">The image part.</param>
        /// <param name="part">The part.</param>
        /// <param name="x">The number in the row the button </param>
        /// <returns>The Radio Button</returns>
        protected RadioButton MakeLevelDesignRadioButton(Image imagePart, Parts part, int x)
        {
            RadioButton newRB = new RadioButton();
            newRB.BackgroundImage = imagePart;
            newRB.BackgroundImageLayout = ImageLayout.None;
            newRB.Name = String.Format("{0}", (char)part);
            newRB.Location = new Point(LEVELMARGIN * 2 + CTRLBTNWIDTH + x * (50), LEVELMARGIN);
            newRB.Width = 50;
            newRB.Height = 50;
            return newRB;
        }

        //Button Clicks        
        /// <summary>
        /// Changes a level grid button to the image selected by the radio button.
        /// Passes this change to the controller.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void levelGrid_buttonClick(object sender, System.EventArgs e)
        {
            //Update button
            //Update designer Level
            Button clickedButton = (Button)sender;
            RadioButton rb = GetSelectedRB();
            clickedButton.Image = rb.BackgroundImage;
            string[] row_col = clickedButton.Name.Split('_');
            Ctrl.DesignerSetPartAt(int.Parse(row_col[0]), int.Parse(row_col[1]), (Parts)char.Parse(rb.Name));
        }

        /// <summary>
        /// Opens a new level in the designer button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void levelDesignerNew_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.OpenDesignerNew();
        }

        /// <summary>
        /// Closes the designer button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void levelDesignerClose_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.CloseDesigner();
        }

        /// <summary>
        /// Loads a saved level into the designer button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void levelDesignerLoad_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.LoadLevel(Ctrl.Design_ST);
        }

        /// <summary>
        /// Saves the level in the designer button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void levelDesignerSave_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.SaveLevelDesigner();
        }

        /// <summary>
        /// Loads a level into game, starts a new game of that level button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void gameLoad_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.LoadLevel(Ctrl.Game_ST);
        }

        /// <summary>
        /// Restarts the currently loaded level in game button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void restartGame_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.NewGame();
        }

        /// <summary>
        /// Loads a state saved for the currently loaded level button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void gameLoadState_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.LoadGameState();
        }

        /// <summary>
        /// Saves the current game to a state button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void gameSaveState_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.SaveGameState();
        }

        /// <summary>
        /// Closes the game back to main menu button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void gameClose_buttonClick(object sender, System.EventArgs e)
        {
            this.KeyPress -= FormMain_KeyPress;
            Ctrl.CloseGame();
        }

        /// <summary>
        /// Key press to move player in game button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            //MoveFromKey((Keys)e.KeyChar);
            MoveFromKey(char.ToUpper((char)e.KeyChar));
        }

        /// <summary>
        /// Switches the key to the move direction.
        /// </summary>
        /// <param name="k">The k.</param>
        protected void MoveFromKey(char k)
        {
            if (k == 'W')
            {
                Ctrl.Move(Direction.Up);
            }
            else if (k == 'S')
            {
                Ctrl.Move(Direction.Down);
            }
            else if (k == 'A')
            {
                Ctrl.Move(Direction.Left);
            }
            else if (k == 'D')
            {
                Ctrl.Move(Direction.Right);
            }
        }
        
        // Move buttons
        protected void moveUp_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.Move(Direction.Up);
        }

        protected void moveDown_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.Move(Direction.Down);
        }

        protected void moveLeft_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.Move(Direction.Left);
        }

        protected void moveRight_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.Move(Direction.Right);
        }

        /// <summary>
        /// Undo button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void undo_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.Undo();
        }

        /// <summary>
        /// Check level for designer button handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void checkDesignerLevel_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.CheckDesignerLevel();
        }
    }
}