using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Sudoku
{
    public partial class Sudoku_Form : Form
    {
        #region Global variables Initializers & structs

        public struct btnkey
        {
            // Declare button
            public Button btn;
            // Declare intiger values for x axis position y axis position and box position
            public int xpos, ypos, bpos;
            // For every button in struct _btnkey there shall be an xposition, yposition, and box position
            public btnkey(Button btn, int xpos, int ypos, int bpos)
            {
                this.btn = btn;
                this.xpos = xpos;
                this.ypos = ypos;
                this.bpos = bpos;
            }
        }

        // Initialize struct _btnkey as an array value with 81 index positions
        btnkey[] key = new btnkey[81];
        // TODO :: figure out wtf i was using these for and add a comment that describes what they are for and why i am initializing them
        // TODO :: find better names than ...string, and use camel case
        string[] xpvstring = new string[9];
        string[] ypvstring = new string[9];
        string[] bpvstring = new string[9];
        string[] tempxpvstring = new string[9];
        string[] tempypvstring = new string[9];
        string[] tempbpvstring = new string[9];

        /*  Initialize intiger values for x and y axis, and horizontal/vertical drawing points
            Initialize intiger values for button mapping keys x y b and v  */
        int xdp = 7, ydp = 7, hdp = 0, vdp = 0;
        int xkey = 0, ykey = 1, bkey = 0, vkey = 0;

        bool isAllOneToNine;
        Button mouseOverButton;

        #endregion

        public Sudoku_Form()
        {
            InitializeComponent();
            buildButtons(); // populates buttons on Sudoku_Form
        }

        #region Events

        void b_MouseClick(object sender, MouseEventArgs e)
        {
            SimulateButtonDown();
            int count;
            count = cbt(sender);
            clickType(sender, e, count);
            validateForm();
            endgame();
        }

        void b_MouseEnter(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.Select();
            mouseOverButton = button;
        }

        void b_MouseLeave(object sender, EventArgs e)
        {
            button1.Select();
            mouseOverButton = null;
        }

        void b_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (mouseOverButton != null)
            {
                Match OneToNine = Regex.Match(e.KeyChar.ToString(), @"[1-9]");
                if (OneToNine.Success)
                {
                    mouseOverButton.Text = e.KeyChar.ToString();
                    validateForm();
                    endgame();
                }
                if (e.KeyChar == 8 || e.KeyChar == 32)
                {
                    SimulateButtonUp();
                    mouseOverButton.Text = null;
                }
            }
        }

        #endregion

        #region Form population methods

        public void buildButtons()
        {
            for (int i = 0; i <= 80; i++)
            {
                //initialize pointer values for button key struct
                this.key[i].xpos = xposkey();
                this.key[i].ypos = yposkey();
                this.key[i].bpos = bposkey();
                this.key[i].btn = new Button();

                //define button properties
                key[i].btn.Location = new System.Drawing.Point(XPos(), YPos());
                key[i].btn.Size = new System.Drawing.Size(29, 24);
                key[i].btn.Name = i.ToString();
                key[i].btn.TabIndex = i;
                key[i].btn.MouseDown += new MouseEventHandler(b_MouseClick);
                key[i].btn.MouseEnter += new System.EventHandler(b_MouseEnter);
                key[i].btn.MouseLeave += new System.EventHandler(b_MouseLeave);
                key[i].btn.KeyPress += new KeyPressEventHandler(b_KeyPress);
                key[i].btn.Visible = true;


                //button text for debuging
                //key[i].btn.Text = i.ToString(); 
                //key[i].btn.Text = "x"+key[i].xpos.ToString();
                //key[i].btn.Text = "y"+key[i].ypos.ToString();
                //key[i].btn.Text = "b"+key[i].bpos.ToString();
                //key[i].btn.Text = key[i].xpos.ToString() + key[i].ypos.ToString();
                //if (key[i].xpos.ToString() == key[i].ypos.ToString()) { key[i].btn.Text = key[i].xpos.ToString(); } else { key[i].btn.Text = key[i].xpos.ToString() + key[i].ypos.ToString(); }
                //key[i].btn.Size = new System.Drawing.Size(33, 24); //must be enabled for display of three charicter strings inside buttons on form.
                //key[i].btn.Text = key[i].xpos.ToString() + key[i].ypos.ToString() + key[i].bpos.ToString();

                //actual implimentaion value of text
                key[i].btn.ResetText(); // comment this line to see the button layout on form

                // add buttons to form1
                this.Controls.Add(key[i].btn);
            }
        }

        public int XPos()
        {
            hdp++;
            if (hdp > 1) { xdp = xdp + 29; }
            if (hdp == 4) { xdp = xdp + 7; }
            if (hdp == 7) { xdp = xdp + 7; }
            if (hdp == 10) { xdp = 7; }
            return xdp;

        }

        public int YPos()
        {
            if (hdp == 10)
            {
                ydp = ydp + 24;
                hdp = 1;
                vdp++;
                if (vdp == 3) { ydp = ydp + 7; }
                if (vdp == 6) { ydp = ydp + 7; }
                return ydp;
            }
            else
            {
                return ydp;
            }
        }

        #endregion

        #region Button mapping methods

        public int xposkey()
        {
            xkey++;
            if (xkey == 10)
            {
                xkey = 1;
                return xkey;
            }
            else
            {
                return xkey;
            }
        }

        public int yposkey()
        {
            vkey++;
            if (vkey == 10)
            {
                ykey++;
                vkey = 1;
                return ykey;
            }
            else
            {
                return ykey;
            }
        }

        public int bposkey()
        {
            if (ykey <= 3 && xkey <= 3) { bkey = 1; }
            if (ykey <= 3 && xkey >= 4 && xkey <= 6) { bkey = 2; }
            if (ykey <= 3 && xkey >= 7 && xkey <= 9) { bkey = 3; }
            if (ykey >= 4 && ykey <= 6 && xkey <= 3) { bkey = 4; }
            if (ykey >= 4 && ykey <= 6 && xkey >= 4 && xkey <= 6) { bkey = 5; }
            if (ykey >= 4 && ykey <= 6 && xkey >= 7 && xkey <= 9) { bkey = 6; }
            if (ykey >= 7 && ykey <= 9 && xkey <= 3) { bkey = 7; }
            if (ykey >= 7 && ykey <= 9 && xkey >= 4 && xkey <= 6) { bkey = 8; }
            if (ykey >= 7 && ykey <= 9 && xkey >= 7 && xkey <= 9) { bkey = 9; }

            return bkey;
        }

        #endregion

        #region functions

        //button click count up
        public static int bccup(int clickcount)
        {
            clickcount = clickcount + 1;
            if (clickcount == 10)
            {
                clickcount = 1;
            }
            return clickcount;
        }

        //button click count down
        public static int bccdown(int clickcount)
        {
            if (clickcount <= 0) { clickcount = 1; }
            clickcount = clickcount - 1;
            if (clickcount == 0)
            {
                clickcount = 9;
            }

            return clickcount;
        }

        //gets the mouse button used to click on form button and calls appropriate method 
        void clickType(object sender, MouseEventArgs e, int count)
        {
            Button button = sender as Button;
            if (e.Button == MouseButtons.Right)
            {
                button.Text = bccdown(count).ToString();
            }
            else
            {
                button.Text = bccup(count).ToString();
            }
        }

        //get clicked button text and convert to intiger value
        public int cbt(object sender)
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

        public void SimulateButtonDown()
        {

            this.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 0, 1, 1, 0));
        }

        public void SimulateButtonUp()
        {
            this.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
        }

        #endregion

        #region form validation

        public void validateForm()
        {
            isAllOneToNine = true;
            string[] xvalue = new string[9];
            string[] yvalue = new string[9];
            string[] bvalue = new string[9];
            for (int i = 1; i < 10; i++)
            {
                if (isAllOneToNine == true)
                {
                    for (int f = 0; f <= 80; f++)
                    {
                        if (key[f].xpos == i) { xvalue[i - 1] += key[f].btn.Text.ToString(); }
                        if (key[f].ypos == i) { yvalue[i - 1] += key[f].btn.Text.ToString(); }
                        if (key[f].bpos == i) { bvalue[i - 1] += key[f].btn.Text.ToString(); }
                    }
                    if (isAllOneToNine == true)
                    {
                        checkval(xvalue[i - 1]);
                        if (isAllOneToNine == true)
                        {
                            checkval(yvalue[i - 1]);
                            if (isAllOneToNine == true)
                            {
                                checkval(bvalue[i - 1]);
                            }
                        }
                    }
                }
            }
        }

        public void checkval(string val)
        {
            Match match = Regex.Match(val, @"(?!.*([1-9]).*\1)^[1-9]{9}$");
            if (match.Success)
            {
                isAllOneToNine = true;
            }
            else
            {
                isAllOneToNine = false;
            }
        }

        #endregion

        public void endgame()
        {
            if (isAllOneToNine == true)
            {
                MessageBox.Show("You have successfully completed a game of Sudoku.", "Congratulations!", MessageBoxButtons.OK);
            }
        }

    }
}
