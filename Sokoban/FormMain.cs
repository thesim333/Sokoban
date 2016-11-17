using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Designer;
using GameNS;

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

        public FormMain()
        {
            InitializeComponent();
            MyGraphics = this.CreateGraphics();
            IH = new ImageHandler();
            this.AutoSize = true;
            MakeLevelDesignRadioPartControl();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            DisplayMain();
        }

        public void AddController(SokobanController ctrl)
        {
            Ctrl = ctrl;
        }

        //Display Changes
        protected void ResetForm()
        {
            this.Controls.Clear();
            MyGraphics.Clear(Color.White);
        }

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

        public void DesignerLoadLevel()
        {
            ResetForm();
            DesignerButtons();
        }

        protected void DesignerButtons()
        {
            CreateControlButton("New Level", 0, levelDesignerNew_buttonClick);
            CreateControlButton("Save Level", 1, levelDesignerSave_buttonClick);
            CreateControlButton("Load Level", 2, levelDesignerLoad_buttonClick);
            CreateControlButton("Close Designer", 3, levelDesignerClose_buttonClick);
        }

        protected void GameButtons()
        {
            CreateControlButton("Restart Game", 0, restartGame_buttonClick);
            CreateControlButton("Load New Level", 1, gameLoad_buttonClick);
            CreateControlButton("Load Game State", 2, gameLoadState_buttonClick);
            CreateControlButton("Save Game State", 3, gameSaveState_buttonClick);
            CreateControlButton("Close Game", 4, gameClose_buttonClick);
            CreateControlButton("Undo", 5, undo_buttonClick);
        }

        public void GameSetup(int moves)
        {
            ResetForm();
            GameButtons();
            MakeLabel();
            SetMoves(moves);
            CreateMoveButton("W", 33, 360, moveUp_buttonClick);
            CreateMoveButton("A", 10, 402, moveLeft_buttonClick);
            CreateMoveButton("S", 33, 444, moveDown_buttonClick);
            CreateMoveButton("D", 56, 402, moveRight_buttonClick);
            this.KeyPress -= FormMain_KeyPress;
            this.KeyPress += FormMain_KeyPress;
        }

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

        public void ReestablishKeys()
        {
            this.KeyPress += FormMain_KeyPress;
        }

        public void DisplayMain()
        {
            MyGraphics.Clear(Color.White);
            Controls.Clear();
            CreateControlButton("Game - Load Level", 0, gameLoad_buttonClick);
            CreateControlButton("Designer - New Level", 1, levelDesignerNew_buttonClick);
            CreateControlButton("Designer - Load Level", 2, levelDesignerLoad_buttonClick);
        }

        //Misc
        protected void MakeLabel()
        {
            Label moves = new Label();
            moves.Name = "lblMoves";
            moves.Location = new Point(LEVELMARGIN, 337);
            moves.Font = new Font("Rockwell", 14);
            moves.ForeColor = Color.Black;
            this.Controls.Add(moves);
        }

        public void SetMoves(int moves)
        {
            Label lblMoves = this.Controls.Find("lblMoves", false).FirstOrDefault() as Label;
            string x = "Moves:" + moves.ToString();
            lblMoves.Text = x;
        }

        public void FinishGame()
        {
            this.KeyPress -= FormMain_KeyPress;
            this.Controls.Clear();
            CreateControlButton("Restart Game", 0, restartGame_buttonClick);
            CreateControlButton("Load New Level", 1, gameLoad_buttonClick);
            CreateControlButton("Load Game State", 2, gameLoadState_buttonClick);
        }

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

        protected int GetControlButtonY(int number)
        {
            return LEVELMARGIN + number * (CTRLBTNHEIGHT + CTRLBTNSPACEBTWN);
        }

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

        protected void levelDesignerNew_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.OpenDesignerNew();
        }

        protected void levelDesignerClose_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.CloseDesigner();
        }

        protected void levelDesignerLoad_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.LoadLevel(Ctrl.Design_ST);
        }

        protected void levelDesignerSave_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.SaveLevelDesigner();
        }

        protected void gameLoad_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.LoadLevel(Ctrl.Game_ST);
        }

        protected void restartGame_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.NewGame();
        }

        protected void gameLoadState_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.LoadGameState();
        }

        protected void gameSaveState_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.SaveGameState();
        }

        protected void gameClose_buttonClick(object sender, System.EventArgs e)
        {
            this.KeyPress -= FormMain_KeyPress;
            Ctrl.CloseGame();
        }

        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            //MoveFromKey((Keys)e.KeyChar);
            MoveFromKey(char.ToUpper((char)e.KeyChar));
        }

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

        protected void undo_buttonClick(object sender, System.EventArgs e)
        {
            Ctrl.Undo();
        }
    }
}