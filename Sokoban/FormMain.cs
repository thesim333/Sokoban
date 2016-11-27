using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameGlobals;

namespace Sokoban
{
    /// <summary>
    /// Main View of the Sokoban application
    /// Used in conjunction with an IController object
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    /// <seealso cref="Sokoban.IView" />
    public partial class FormMain : Form, IView
    {
        protected IController Ctrl;
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
        protected int Rows;
        protected int Cols;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormMain"/> class as an IView.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            MyGraphics = this.CreateGraphics();
            IH = new ImageHandler();
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
        public void AddController(IController ctrl)
        {
            Ctrl = ctrl;
        }

        /// <summary>
        /// Resets the form to empty white.
        /// </summary>
        protected void ResetForm()
        {
            Controls.Clear();
            MyGraphics.Clear(Color.White);
            AutoSize = false;
            Width = LEVELMARGIN * 2 + SQUARESIDESIZE * 20 + CTRLBTNWIDTH + 30;
            Height = LEVELMARGIN * 2 + SQUARESIDESIZE * 20 + 40;
        }

        /// <summary>
        /// Changes the view to the designer with a new a bew empty level.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The cols.</param>
        public void DesignerNewLevel(int rows, int cols)
        {
            ResetForm();
            Rows = rows;
            Cols = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    CreateLevelGridButton(r, c, Parts.Empty);
                }
            }

            DesignerRadioButtonDisplay();
            DesignerButtons();
            AutoSize = true;
        }

        /// <summary>
        /// Displays the radio buttons for selection of the Parts in Designer.
        /// </summary>
        protected void DesignerRadioButtonDisplay()
        {
            for (int i = 0; i < partsSelector.Length; i++)
            {
                Controls.Add(partsSelector[i]);
            }
        }

        /// <summary>
        /// Changes the view to the designer waiting for the level to be loaded from a file.
        /// </summary>
        public void DesignerLoadLevel(int rows, int cols)
        {
            ResetForm();
            DesignerRadioButtonDisplay();
            DesignerButtons();
            Rows = rows;
            Cols = cols;
            AutoSize = true;
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
            CreateControlButton("Delete Game State", 3, GameDeleteState_buttonClick);
            CreateControlButton("Save Game State", 4, gameSaveState_buttonClick);
            CreateControlButton("Close Game", 5, gameClose_buttonClick);
            CreateControlButton("Undo", 6, undo_buttonClick);
        }

        /// <summary>
        /// Setup the view for the game
        /// </summary>
        /// <param name="moves">The moves from the game to display</param>
        public void GameSetup(int moves, int rows, int cols)
        {
            ResetForm();
            GameButtons();
            MakeMoveLabel();
            SetMoves(moves);
            MakeMoveButtons();
            Rows = rows;
            Cols = cols;
        }

        /// <summary>
        /// Draws a game position with the image representing the part from that position in the game.
        /// Draws up to a 20 x 20 grid.
        /// Centers the grid in the game space.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="part">The part.</param>
        public void SetGamePosition(int row, int col, Parts part)
        {
            int rectCol = LEVELMARGIN + 10 + CTRLBTNWIDTH + SQUARESIDESIZE * col + ((20 - Cols) / 2) * SQUARESIDESIZE ; //10 is the space between buttons and the grid
            int rectRow = LEVELMARGIN + SQUARESIDESIZE * row + ((20 - Rows) / 2) * SQUARESIDESIZE;
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
            ResetForm();
            CreateControlButton("Game - Load Level", 0, gameLoad_buttonClick);
            CreateControlButton("Designer - New Level", 1, levelDesignerNew_buttonClick);
            CreateControlButton("Designer - Load Level", 2, levelDesignerLoad_buttonClick);
            DisplayMainPic();
        }

        protected void DisplayMainPic()
        {
            PictureBox pb = new PictureBox();
            pb.Image = IH.MainPic;
            pb.Location = new Point(LEVELMARGIN + CTRLBTNWIDTH + 10, LEVELMARGIN);
            pb.Size = new Size(155, 155);
            Controls.Add(pb);
        }

        /// <summary>
        /// Makes the label showing the moves made in the game.
        /// </summary>
        protected void MakeMoveLabel()
        {
            Label moves = new Label();
            moves.Name = "lblMoves";
            moves.Location = new Point(LEVELMARGIN, 397);
            moves.Font = new Font("Rockwell", 14);
            moves.ForeColor = Color.Black;
            Controls.Add(moves);
        }

        /// <summary>
        /// Displays the current move count from game.
        /// </summary>
        /// <param name="moves">The moves.</param>
        public void SetMoves(int moves)
        {
            Label lblMoves = Controls.Find("lblMoves", false).FirstOrDefault() as Label;
            string x = "Moves:" + moves.ToString();
            lblMoves.Text = x;
        }

        /// <summary>
        /// Finishes the game. Changes the view to only show buttons that can start a new game.
        /// </summary>
        /// <param name="moves">The moves from the finish of the game.</param>
        public void FinishGame(int moves)
        {
            //this.KeyPress -= FormMain_KeyPress;
            Controls.Clear();
            CreateControlButton("Restart Game", 0, restartGame_buttonClick);
            CreateControlButton("Load New Level", 1, gameLoad_buttonClick);
            CreateControlButton("Load Game State", 2, gameLoadState_buttonClick);
            CreateControlButton("Delete Game State", 3, GameDeleteState_buttonClick);
            MakeMoveLabel();
            SetMoves(moves);
            WriteGameWon();
        }

        /// <summary>
        /// Writes game complete across top of grid area.
        /// </summary>
        protected void WriteGameWon()
        {
            MyGraphics.DrawString("Game Complete", new Font("Rockwell", 30), Brushes.Black, new Point(150, 15));
        }

        /// <summary>
        /// Makes the move buttons.
        /// </summary>
        protected void MakeMoveButtons()
        {
            CreateMoveButton("U", 33, 435, moveUp_buttonClick);
            CreateMoveButton("L", 10, 477, moveLeft_buttonClick);
            CreateMoveButton("D", 33, 519, moveDown_buttonClick);
            CreateMoveButton("R", 56, 477, moveRight_buttonClick);
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
            Point p = new Point(LEVELDESIGNLEFTMARGIN + col * LEVELDESIGNBTNSIZE + ((20 - Rows) / 2) * LEVELDESIGNBTNSIZE,
                60 + row * LEVELDESIGNBTNSIZE + ((20 - Cols) / 2) * LEVELDESIGNBTNSIZE);
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
            Controls.Add(newButton);
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
            Controls.Add(newButton);
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
            Controls.Add(newButton);
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
        /// Changes the level grid button to the image selected by the radio button.
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
        /// Removes a game state buttonClick handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void GameDeleteState_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.DeleteState();
        }

        /// <summary>
        /// Opens a new level in the designer - buttonClick handler.
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
            Ctrl.LoadLevelDesign();
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
            Ctrl.LoadLevelGame();
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
        /// Passes the direction to controller.
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

        /// <summary>
        /// Gets the name and path of the file to save.
        /// </summary>
        /// <param name="initialDir">The initial dir.</param>
        /// <returns></returns>
        public string SaveMyFile(string initialDir)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "level files (*.lvl)|*.lvl";
            sfd.Title = "Save This Level";
            sfd.DefaultExt = "lvl";
            sfd.InitialDirectory = initialDir;
            sfd.ShowDialog();
            return sfd.FileName;
        }

        /// <summary>
        /// Displays the specified message as a dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Display(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Gets the input. 
        /// User name or state name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public string GetInput(string message)
        {
            InputForm inForm = new InputForm();
            inForm.SetLabel(message);
            inForm.ShowDialog();
            return inForm.GetResult();
        }

        /// <summary>
        /// Gets the size of the level to be created in designer.
        /// </summary>
        /// <returns>Array {rows, columns}</returns>
        public int[] GetLevelSize()
        {
            SizeDialog sd = new SizeDialog();
            sd.ShowDialog();
            return new int[] { sd.GetRows(), sd.GetCols() };
        }

        /// <summary>
        /// Gets the user response.
        /// Yes, No, Cancel
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <returns></returns>
        public string GetUserResponse(string message, string caption)
        {
            switch (MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    return "Y";
                case DialogResult.No:
                    return "N";
                default:
                    return "C";
            }
        }

        /// <summary>
        /// Gets the file to load.
        /// </summary>
        /// <param name="initialDir">The initial directory to open.</param>
        /// <returns>The full file path</returns>
        public string GetFileToLoad(string initialDir)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = initialDir;
            ofd.Multiselect = false;
            ofd.Filter = "level files (*.lvl)|*.lvl";
            ofd.Title = "Select Level To Load";
            ofd.ShowDialog();
            return ofd.FileName;
        }

        /// <summary>
        /// Displays the states saved for this level in a dialog.
        /// The user can select one.
        /// </summary>
        /// <param name="states">The states.</param>
        /// <returns>The name of the selected state</returns>
        public string GetSelectedState(string[] states)
        {
            StateLoadDialog sld = new StateLoadDialog();
            sld.InsertStates(states);
            if (sld.ShowDialog() == DialogResult.OK)
            {
                return sld.GetSelected();
            }
            return string.Empty;
        }
    }
}