using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;

namespace Sudoku_V2
{
    public partial class SudokuForm : Form
    {
        public SudokuForm()
        {
                InitializeComponent();
        }

        #region General comments and tasks
        // DBK (ACRYNIM FOR) DYNAMIC BUTTON KEY

        /* TODO: List of general todo's here.
         * Fill in all summarys with appropriate descriptions
         * comment each line of code where it may be needed
         * refactor and reduce number of global variables anything that is not needed for multiple events should not be global
         * global variables that are needed should be called from seperate class and values stored in that class insead of being global
         */
        #endregion

        // Strictly used for global variables that will need to be accessed for events with different call timing
        #region Class variable initializers & structs
        static Properties.Settings settings = new Properties.Settings();
        // Used for linking x and y coordinates and visual box coordinates with button object
        struct DynamicButtonKey
        {
            // declare button object 
            public Button btn;
            // declare intiger values for x axis position y axis position and visual box position
            public int xPosistion, yPosistion, xValue, yValue, boxValue;
            public List<int> invalidInput;
            public DynamicButtonKey(Button btn, int xPosistion, int yPosistion, int xValue, int yValue, int boxValue, List<int> invalidInput)
            {
                this.btn = btn;
                this.xPosistion = xPosistion;
                this.yPosistion = yPosistion;
                this.xValue = xValue;
                this.yValue = yValue;
                this.boxValue = boxValue;
                this.invalidInput = invalidInput;
            }
        }
        Dictionary<string, DynamicButtonKey> btnKeys = new Dictionary<string, DynamicButtonKey>();
        Dictionary<string, int> btnInputValue = new Dictionary<string, int>();
        #region Lists for validating form
        //lists for storing btn in each row column and box
        //used for gettig list to be used when validating
        List<List<Button>> xListofLists = new List<List<Button>>();
        List<List<Button>> yListofLists = new List<List<Button>>();
        List<List<Button>> bListofLists = new List<List<Button>>();
        //used to store buttons with same x values
        List<Button> xList1 = new List<Button>();
        List<Button> xList2 = new List<Button>();
        List<Button> xList3 = new List<Button>();
        List<Button> xList4 = new List<Button>();
        List<Button> xList5 = new List<Button>();
        List<Button> xList6 = new List<Button>();
        List<Button> xList7 = new List<Button>();
        List<Button> xList8 = new List<Button>();
        List<Button> xList9 = new List<Button>();
        //used to store buttons with same y values
        List<Button> yList1 = new List<Button>();
        List<Button> yList2 = new List<Button>();
        List<Button> yList3 = new List<Button>();
        List<Button> yList4 = new List<Button>();
        List<Button> yList5 = new List<Button>();
        List<Button> yList6 = new List<Button>();
        List<Button> yList7 = new List<Button>();
        List<Button> yList8 = new List<Button>();
        List<Button> yList9 = new List<Button>();
        //used to store buttons with same box values
        List<Button> bList1 = new List<Button>();
        List<Button> bList2 = new List<Button>();
        List<Button> bList3 = new List<Button>();
        List<Button> bList4 = new List<Button>();
        List<Button> bList5 = new List<Button>();
        List<Button> bList6 = new List<Button>();
        List<Button> bList7 = new List<Button>();
        List<Button> bList8 = new List<Button>();
        List<Button> bList9 = new List<Button>();
        #endregion
        // using these to preform math for posistion of buttons and values
        // sets variables for DynamicButtonKey
        public int y { get; set; }
        public int x { get; set; }
        public int yVal { get; set; }
        public int xVal { get; set; }
        public int bVal { get; set; }
        // using hardcoded width and height values for buttons
        // may eventualy be used for scaling button size on form when form deminsions change
        public int width { get; set; }
        public int height { get; set; }
        //used to set the button the mouse is over
        Button mouseOverButton;
        // use these for storing settings so we get them for the settings file only once
        // better perfomance than calling them directly form the settings file each time we want to use them
        static bool showStatus = settings.ShowStatus;
        static bool showToolTip = settings.ShowToolTip;
        #region Stat variales
        // used for calculating a score at end game
        int numberOfInputs; // keeps count of number of times there was succesful input
        int numberOfFailedAttempts; // keeps count of number of times there was failed input eg when a box can have no values placed 
        int twoToneTicks; // keeps counts of number of seconds sense firstt click -- naming convention irrelivant here was thinking timmy two tone for some reason
        #endregion
        #endregion

        // Code in the form population region will only happen on form load;
        #region Form population
        // All custom controls and values will be stored in List<DynamicButtonKey> BtnKeys for later operations;
        /// <summary>
        /// used to manipulate form size per user settings
        /// </summary>
        private void SetShowStatusOnForm()
        {
            bool showStatus = settings.ShowStatus;
            if (showStatus)
            {   
                this.SuspendLayout(); // layout must be suspended for resizing
                this.ResizeRedraw = true;
                this.Size = new System.Drawing.Size(296, 314);
                this.PerformLayout();
                statusStrip1.Visible = true;
                StatusLabel.Visible = true;
            } 
            else 
            {
                this.SuspendLayout();
                this.ResizeRedraw = true;
                this.Size = new System.Drawing.Size(296, 314 - 22);
                this.PerformLayout();
                StatusLabel.Visible = false;
                statusStrip1.Visible = false;
                
            }
        }

        /// <summary>
        /// adds buttons to appropriate list using xy and box values
        /// </summary>
        private void PopulateXYBLists()
        {
            #region populate lists of type list
            // populate each list for storing buttons contained inside of lists for storing each list with the appropriate button
            xListofLists.Add(xList1);
            xListofLists.Add(xList2);
            xListofLists.Add(xList3);
            xListofLists.Add(xList4);
            xListofLists.Add(xList5);
            xListofLists.Add(xList6);
            xListofLists.Add(xList7);
            xListofLists.Add(xList8);
            xListofLists.Add(xList9);
            yListofLists.Add(yList1);
            yListofLists.Add(yList2);
            yListofLists.Add(yList3);
            yListofLists.Add(yList4);
            yListofLists.Add(yList5);
            yListofLists.Add(yList6);
            yListofLists.Add(yList7);
            yListofLists.Add(yList8);
            yListofLists.Add(yList9);
            bListofLists.Add(bList1);
            bListofLists.Add(bList2);
            bListofLists.Add(bList3);
            bListofLists.Add(bList4);
            bListofLists.Add(bList5);
            bListofLists.Add(bList6);
            bListofLists.Add(bList7);
            bListofLists.Add(bList8);
            bListofLists.Add(bList9);
            #endregion
            #region comments
            // using x value y value and box value as indexs 
            // code could be reduced in number of lines here simply by making the base lists arrays insted of hard coding them out
            // but i feel that may become too damn complicated for later use
            // would end up looking something like this in the foreach below -- I think.... :)
            // XLists[btnKeys[keyPair.Key].xValue - 1].Add(btnKeys[keyPair.Key].btn);
            // the problem lays in the ability to get the list we need to use later in code
            // i think it would slow operations down way more than this method 
            // as the only way i can forsee actually getting that list is by doing loops like the one below each time
            // in a later itterations we may find that this is a design flaw
            // im a positive there are better ways to do this
            // but this seems to be working well for now =p
            #endregion comments
            foreach (KeyValuePair<string, DynamicButtonKey> keyPair in btnKeys)
            {
                xListofLists[btnKeys[keyPair.Key].xValue - 1].Add(btnKeys[keyPair.Key].btn);
                yListofLists[btnKeys[keyPair.Key].yValue - 1].Add(btnKeys[keyPair.Key].btn);
                bListofLists[btnKeys[keyPair.Key].boxValue - 1].Add(btnKeys[keyPair.Key].btn);
            }
        }

        /// <summary>
        /// Initializes starting values for global variables that need it
        /// </summary>
        private void InitializeApplicationStartingValues()
        {
            // no point in extra code/work only instanciate variables that need instanciated
            x = 7;
            y = 7;
            xVal = 1;
            yVal = 1;
            // setting default selected object here as well seemed like a good place to do it
            btnHideFocus.Select(); // this is used as a visual enhancement
        }

        /// <summary>
        /// adds 81 button to the form 
        /// </summary>
        public void AddDynamicButtonsOnForm()
        {
            SetButtonHeightWidthValues();
            for (int i = 0; i < 81; i++)
            {
                DynamicButtonKey key = GetButtonKeyProperties(i);
                btnKeys.Add(key.btn.Name, key);
                this.Controls.Add(btnKeys[key.btn.Name].btn);
                #region used for debuging
                //HACK: show x values on button text; row
                //btnKeys[key.btn.Name].btn.Text = "X" + btnKeys[key.btn.Name].xValue.ToString();
                //HACK: show y values as button text; column
                //btnKeys[key.btn.Name].btn.Text = "Y" + btnKeys[key.btn.Name].yValue.ToString();
                //HACK: Show box values as button text; box
                //btnKeys[key.btn.Name].btn.Text = "B" + btnKeys[key.btn.Name].boxValue.ToString();
                #endregion
            }
        }

        /// <summary>
        /// gets the properties of a single DBK
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private DynamicButtonKey GetButtonKeyProperties(int i)
        {
            DynamicButtonKey DBKForAdditionToBtnKeysList = new DynamicButtonKey();
            DBKForAdditionToBtnKeysList = SetXYBValues(DBKForAdditionToBtnKeysList);
            DBKForAdditionToBtnKeysList = SetXPosistion(DBKForAdditionToBtnKeysList);
            DBKForAdditionToBtnKeysList = SetYPosistion(DBKForAdditionToBtnKeysList);
            DBKForAdditionToBtnKeysList.invalidInput = new List<int>();
            DBKForAdditionToBtnKeysList = SetBtnProperties(i, DBKForAdditionToBtnKeysList);
            return DBKForAdditionToBtnKeysList;
        }

        /// <summary>
        /// Defines properties for button object used in DynamicButtonKey.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="DBKForAdditionToBtnKeysList"></param>
        /// <returns>DynamicButtonKey</returns>
        private DynamicButtonKey SetBtnProperties(int i, DynamicButtonKey DBKForAdditionToBtnKeysList)
        {
            DBKForAdditionToBtnKeysList.btn = new Button();
            DBKForAdditionToBtnKeysList.btn.Name = "btn_" + i;
            DBKForAdditionToBtnKeysList.btn.Text = "";
            DBKForAdditionToBtnKeysList.btn.Visible = true;
            DBKForAdditionToBtnKeysList.btn.Size = new System.Drawing.Size(width, height);
            DBKForAdditionToBtnKeysList.btn.Location = new System.Drawing.Point(DBKForAdditionToBtnKeysList.xPosistion, DBKForAdditionToBtnKeysList.yPosistion + 22);
            DBKForAdditionToBtnKeysList.btn.TabIndex = i;
            DBKForAdditionToBtnKeysList.btn.Enabled = true;
            DBKForAdditionToBtnKeysList.btn.MouseEnter += new EventHandler(btn_MouseEnter);
            DBKForAdditionToBtnKeysList.btn.MouseLeave += new EventHandler(btn_MouseLeave);
            DBKForAdditionToBtnKeysList.btn.MouseDown += new MouseEventHandler(btn_MouseDown);
            DBKForAdditionToBtnKeysList.btn.KeyPress += new KeyPressEventHandler(btn_KeyPress);
            DBKForAdditionToBtnKeysList.btn.MouseClick += new MouseEventHandler(btn_MouseClick);
            DBKForAdditionToBtnKeysList.btn.MouseHover += new EventHandler(btn_MouseHover);
            return DBKForAdditionToBtnKeysList;
        }

        /// <summary>
        /// Sets dynamic buttons x posistion drawing point
        /// </summary>
        /// <param name="DBKForAdditionToBtnKeysList"></param>
        /// <returns>DBKForAdditionToBtnKeysList</returns>
        private DynamicButtonKey SetXPosistion(DynamicButtonKey DBKForAdditionToBtnKeysList)
        {
            DBKForAdditionToBtnKeysList.xPosistion = x;
            if (x < 282) { x += 29; };
            switch (x) { case 94: x += 7; break; case 188: x += 7; break; case 282: x = 7; break; }
            return DBKForAdditionToBtnKeysList;
        }

        /// <summary>
        /// Sets dynamic buttons y posistion drawing point
        /// </summary>
        /// <param name="DBKForAdditionToBtnKeysList"></param>
        /// <returns>DynamicButtonKey</returns>
        private DynamicButtonKey SetYPosistion(DynamicButtonKey DBKForAdditionToBtnKeysList)
        {
            DBKForAdditionToBtnKeysList.yPosistion = y;
            if (x == 7) { y += 24; }
            switch (y) { case 79: y += 7; break; case 158: y += 7; break; }
            return DBKForAdditionToBtnKeysList;
        }

        /// <summary>
        /// Sets the x and y values used for validation purposes
        /// </summary>
        /// <param name="DBKForAdditionToBtnKeysList"></param>
        /// <returns>DynamicButtonKey</returns>
        private DynamicButtonKey SetXYBValues(DynamicButtonKey DBKForAdditionToBtnKeysList)
        {
            if (yVal == 10) { yVal = 1; xVal++; }
            SetBoxValues(ref DBKForAdditionToBtnKeysList);
            DBKForAdditionToBtnKeysList.yValue = yVal++;
            DBKForAdditionToBtnKeysList.xValue = xVal;
            return DBKForAdditionToBtnKeysList;
        }

        /// <summary>
        /// Sets the box values using the x and y values used for validation purposes
        /// </summary>
        /// <param name="DBKForAdditionToBtnKeysList"></param>
        /// <returns></returns>
        private void SetBoxValues(ref DynamicButtonKey DBKForAdditionToBtnKeysList)
        {
            if (xVal < 4 && yVal < 4) { bVal = 1; }
            if (xVal < 4 && yVal > 3 && yVal < 7) { bVal = 2; }
            if (xVal <= 3 && yVal >= 7 && yVal <= 9) { bVal = 3; }
            if (xVal >= 4 && xVal <= 6 && yVal <= 3) { bVal = 4; }
            if (xVal >= 4 && xVal <= 6 && yVal >= 4 && yVal <= 6) { bVal = 5; }
            if (xVal >= 4 && xVal <= 6 && yVal >= 7 && yVal <= 9) { bVal = 6; }
            if (xVal >= 7 && xVal <= 9 && yVal <= 3) { bVal = 7; }
            if (xVal >= 7 && xVal <= 9 && yVal >= 4 && yVal <= 6) { bVal = 8; }
            if (xVal >= 7 && xVal <= 9 && yVal >= 7 && yVal <= 9) { bVal = 9; }
            DBKForAdditionToBtnKeysList.boxValue = bVal;
        }

        /// <summary>
        /// may eventualy be used to expand the hieght and width of buttons when form grows
        /// </summary>
        private void SetButtonHeightWidthValues()
        {
            width = 29;
            height = 24;
        }
        #endregion

        // Code in this region will control all operations after form load;
        #region Calling Events and Methods
        #region Events
        void btn_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                StartTimer();
                if (mouseOverButton != null)
                {
                    InsertKeyedValueIfValid(e);
                    ShowToolTip(mouseOverButton);
                    CheckIfComplete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        void btn_MouseDown(object sender, MouseEventArgs e)
        {
            int count;
            try
            {
                Button button = sender as Button;
                if (button != null)
                {
                    GetValidMoveText(button);
                    count = ConvertButtonText(button);
                    GetClickType(sender, e, count);
                    GetValidMoveText(button);
                    ShowToolTip(button);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        void btn_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null)
                {
                    btnHideFocus.Select();
                    mouseOverButton = null;
                    //MyToolTip.RemoveAll();
                    //MyToolTip = new ToolTip();
                    StatusLabel.ForeColor = DefaultForeColor;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        void btn_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null)
                {
                    button.Select();
                    mouseOverButton = button;
                    ShowToolTip(button);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        void btn_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                StartTimer();
                CheckIfComplete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        void btn_MouseHover(object sender, EventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null)
                {
                    ShowToolTip(button);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StopTimer();
                using (SettingsForm setF = new SettingsForm())
                {
                    DialogResult result = setF.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        Application.Restart();
                    }
                    else
                    { 
                        StartTimer();
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        void SudokuForm_Load(object sender, EventArgs e)
        {
            try
            {
                SetShowStatusOnForm();
                InitializeApplicationStartingValues();
                AddDynamicButtonsOnForm();
                PopulateXYBLists();
                #region used for debugging
                //HACK: show buttons held in xList
                //foreach (Button btn in xList8) { btnKeys[btn.Name].btn.Text = "XL"; }
                //HACK: show buttons held in yList
                //foreach (Button btn in yList3) { if (btnKeys[btn.Name].btn.Text == "") { btnKeys[btn.Name].btn.Text = "YL"; } else { btnKeys[btn.Name].btn.Text = "XY"; } }
                //HACK: show buttons held in bList
                //foreach (Button btn in bList9) { if (btnKeys[btn.Name].btn.Text == "") { btnKeys[btn.Name].btn.Text = "BL"; } }
                #endregion used for debugging
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        int systemSecondDifference;
        void timerTwoTone_Tick(object sender, EventArgs e)
        {
            try
            {
                
                watchToolStripMenuItem.Text = DateTime.Now.Second.ToString();
                twoToneTicks++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StopTimer();
                AboutBox1 about = new AboutBox1();
                about.ShowDialog(this);
                StartTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }
        #endregion Events

        #region Methods
        /// <summary>
        /// Starts the timer on form if not started
        /// </summary>
        private void StartTimer()
        {
            if (!timerTwoTone.Enabled)
            {
                timerTwoTone.Start();
                watch.Start();
            }
        }

        /// <summary>
        /// Stops the timer on form if not started
        /// </summary>
        private void StopTimer()
        {
            if (timerTwoTone.Enabled)
            {
                timerTwoTone.Stop();
                watch.Stop();
            }
        }

        /// <summary>
        /// checks if the puzzle is complete 
        /// </summary>
        private void CheckIfComplete()
        {
            bool CheckComplete = true;
            foreach (KeyValuePair<string, DynamicButtonKey> keypair in btnKeys)
            {
                if (btnKeys[keypair.Key].btn.Text == "")
                {
                    CheckComplete = false;
                    break;
                }
            }

            Completed(CheckComplete);
        }

        /// <summary>
        /// This is the endgame method
        /// </summary>
        /// <param name="CheckComplete"></param>
        private void Completed(bool CheckComplete)
        {
            if (CheckComplete)
            {
                timerTwoTone.Stop();
                string score = GetScore().ToString();
                MessageBox.Show(@"Congratulations You Won!!!
                                  Your Score: " + score);
            }
        }

        /// <summary>
        /// Gets the players score based on time taken number of good clicks and number of bad clicks
        /// </summary>
        /// <returns></returns>
        private long GetScore()
        {
            long score = 0;
            if ((((999999999999 / numberOfInputs) / (numberOfFailedAttempts * 3)) / twoToneTicks) > 0)
            {
                score = (long)(((999999999999 / numberOfInputs) / (numberOfFailedAttempts * 3)) / twoToneTicks);
            }
            return score;
        }

        /// <summary>
        /// inserts key pressed to as button value if key is valid key
        /// </summary>
        /// <param name="e"></param>
        private void InsertKeyedValueIfValid(KeyPressEventArgs e)
        {
            Match OneToNine = Regex.Match(e.KeyChar.ToString(), @"[1-9]");
            if (OneToNine.Success)
            {
                if (!btnKeys[mouseOverButton.Name].invalidInput.Contains(Int32.Parse(e.KeyChar.ToString())))
                {
                    if (btnInputValue.ContainsKey(mouseOverButton.Name))
                    {
                        btnInputValue.Remove(mouseOverButton.Name);
                    }
                    btnInputValue.Add(mouseOverButton.Name, Int32.Parse(e.KeyChar.ToString()));
                    mouseOverButton.Text = btnInputValue[mouseOverButton.Name].ToString();
                    numberOfInputs++;
                    ShowToolTip(mouseOverButton);
                }
                else
                {
                    numberOfFailedAttempts++;
                    //MyToolTip.ForeColor = Color.Red;
                    StatusLabel.ForeColor = Color.Red;
                    ShowToolTip("Invalid move: ", mouseOverButton);
                    
                }
            }
            if (e.KeyChar.ToString().ToUpper() == "L")
            {
                if (mouseOverButton.Enabled)
                {
                    mouseOverButton.Enabled = false;
                }
                else 
                {
                    mouseOverButton.Enabled = true;
                }
            }
            // if backspace or space clear value of button
            if (e.KeyChar == 8 || e.KeyChar == 32)
            {
                if (btnInputValue.ContainsKey(mouseOverButton.Name))
                {
                    btnInputValue.Remove(mouseOverButton.Name);
                }
                btnInputValue.Add(mouseOverButton.Name, 0);
                mouseOverButton.Text = null;
                ShowToolTip(mouseOverButton);
            } 
        }

        /// <summary>
        /// shows the tooltip for given button -- overloaded method
        /// </summary>
        /// <param name="button"></param>
        private void ShowToolTip(Button button)
        {
            ToolTip MyToolTip = new ToolTip();
            MyToolTip.ForeColor = DefaultForeColor;
            string toolTipText = GetValidMoveText(button);
            ShowStatus(toolTipText);
            if (showToolTip)
            {
                MyToolTip.RemoveAll();
                MyToolTip.Show(toolTipText, button);
            }
            this.Refresh();
        }
        
        /// <summary>
        /// overload: shows the tooltip for given button with prepended custom text
        /// </summary>
        /// <param name="OptionalTextToPrepend"></param>
        /// <param name="button"></param>
        private void ShowToolTip(string OptionalTextToPrepend, Button button)
        {
            ToolTip MyToolTip = new ToolTip();
            MyToolTip.RemoveAll();
            MyToolTip.ForeColor = DefaultForeColor;
            string toolTipText = OptionalTextToPrepend + GetValidMoveText(button);
            ShowStatus(toolTipText);
            if (showToolTip)
            {
                MyToolTip.RemoveAll();
                MyToolTip.Show(toolTipText, button);
            }
            this.Refresh();
        }

        /// <summary>
        /// set status label text to given text
        /// </summary>
        /// <param name="toolTipText"></param>
        private void ShowStatus(string toolTipText)
        {
            if (showStatus)
            {
                StatusLabel.Text = toolTipText;
            }
        }

        /// <summary>
        /// gets text describing valid moves for a given button
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private string GetValidMoveText(Button button)
        {
            GetListOfInvalidInput(button);
            string toolTipText = "Valid moves are: ";
            for (int i = 1; i < 10; i++)
            {
                if (!btnKeys[button.Name].invalidInput.Contains(i))
                {
                    toolTipText = toolTipText + i.ToString() + ", ";
                }
            }
            toolTipText = toolTipText.TrimEnd(',', ' ');
            if (toolTipText == "Valid moves are:")
            {
                //MyToolTip.ForeColor = Color.Red;
                toolTipText = "There are no valid move's for this box";
            }
            return toolTipText;
        }

        /// <summary>
        /// populates list that contains invalid inputs for a given button
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private bool GetListOfInvalidInput(Button button)
        {
            bool finishedProcessing = false;
            if (button == null) { return true; }
            if(btnKeys.ContainsKey(button.Name))
            {
                btnKeys[button.Name].invalidInput.Clear();
            }
            PopulateInvalidXList(button);
            PopulateInvalidYList(button);
            PopulateInvalidBList(button);
            finishedProcessing = true;
            return finishedProcessing;
        }

        /// <summary>
        /// Populate invalid list of inputs for the box the button is in
        /// </summary>
        /// <param name="button"></param>
        private void PopulateInvalidBList(Button button)
        {
            foreach (Button btn in bListofLists[btnKeys[button.Name].boxValue - 1])
            {
                int inputValue = 0;
                if (btn.Text != "" && btnInputValue.ContainsKey(btn.Name))
                {
                    inputValue = btnInputValue[btn.Name];
                }
                if (!btnKeys[button.Name].invalidInput.Contains(inputValue) && inputValue != 0)
                {
                    btnKeys[button.Name].invalidInput.Add(inputValue);
                }
            }
        }

        /// <summary>
        /// Populate invalid list of inputs for the column the button is in
        /// </summary>
        /// <param name="button"></param>
        private void PopulateInvalidYList(Button button)
        {
            foreach (Button btn in yListofLists[btnKeys[button.Name].yValue - 1])
            {
                int inputValue = 0;
                if (btn.Text != "" && btnInputValue.ContainsKey(btn.Name))
                {
                    inputValue = btnInputValue[btn.Name];
                }
                if (!btnKeys[button.Name].invalidInput.Contains(inputValue) && inputValue != 0)
                {
                    btnKeys[button.Name].invalidInput.Add(inputValue);
                }
            }
        }

        /// <summary>
        /// Populate invalid list of inputs for the row the button is in
        /// </summary>
        /// <param name="button"></param>
        private void PopulateInvalidXList(Button button)
        {
            foreach (Button btn in xListofLists[btnKeys[button.Name].xValue - 1])
            {
                int inputValue = 0;
                if (btn.Text != "" && btnInputValue.ContainsKey(btn.Name))
                {
                    inputValue = btnInputValue[btn.Name];
                }
                if (!btnKeys[button.Name].invalidInput.Contains(inputValue) && inputValue != 0)
                {
                    btnKeys[button.Name].invalidInput.Add(inputValue);
                }
            }
        }

        /// <summary>
        /// button click count up
        /// </summary>
        /// <param name="clickcount"></param>
        /// <returns></returns>
        private int BccUp(int clickcount)
        {
            clickcount = clickcount + 1;
            if (clickcount == 10)
            {
                clickcount = 1;
            }
            return clickcount;
        }

        /// <summary>
        /// button click count down
        /// </summary>
        /// <param name="clickcount"></param>
        /// <returns></returns>
        private int BccDown(int clickcount)
        {
            if (clickcount <= 0) { clickcount = 1; }
            clickcount = clickcount - 1;
            if (clickcount == 0)
            {
                clickcount = 9;
            }
            return clickcount;
        }

        /// <summary>
        /// gets the mouse button used to click on form button and calls appropriate method 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="count"></param>
        private void GetClickType(object sender, MouseEventArgs e, int count)
        {
            Button button = sender as Button;
            if (e.Button == MouseButtons.Right)
            {
                bool invalidMove = false;
                int val = BccDown(count);
                int i = 0;
                while (btnKeys[button.Name].invalidInput.Contains(val))
                {
                    i++;
                    val = BccDown(val);
                    if (i >= 20)
                    {
                        //MyToolTip.ForeColor = Color.Red;
                        StatusLabel.ForeColor = Color.Red;
                        ShowToolTip(button);
                        invalidMove = true;
                        numberOfFailedAttempts++;
                        break;
                    }
                }
                if (!invalidMove)
                {
                    if (btnInputValue.ContainsKey(button.Name))
                    {
                        btnInputValue.Remove(button.Name);
                    }
                    btnInputValue.Add(button.Name, val);
                    button.Text = btnInputValue[button.Name].ToString();
                    numberOfInputs++;
                }
            }
            else
            {
                bool invalidMove = false;
                int val = BccUp(count);
                int i = 0;
                while (btnKeys[button.Name].invalidInput.Contains(val))
                {
                    i++;
                    val = BccUp(val);
                    if (i >= 20)
                    {
                        //MyToolTip.ForeColor = Color.Red;
                        StatusLabel.ForeColor = Color.Red;
                        ShowToolTip(button);
                        invalidMove = true;
                        numberOfFailedAttempts++;
                        break;
                    }
                }
                if (!invalidMove)
                {
                    if (btnInputValue.ContainsKey(button.Name))
                    {
                        btnInputValue.Remove(button.Name);
                    }
                    btnInputValue.Add(button.Name, val);
                    button.Text = btnInputValue[button.Name].ToString();
                    numberOfInputs++;
                }
            }
        }

        /// <summary>
        /// converts a clicked buttons text to an intiger value
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public int ConvertButtonText(object sender)
        {
            Button button = sender as Button;
            int count;
            if (button.Text == "")
            {
                count = 0;
            }
            else
            {
                count = Int32.Parse(button.Text);
            }
            return count;
        }
        #endregion Methods
        #endregion Calling Events and Methods
    }
}