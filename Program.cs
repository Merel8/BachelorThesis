using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace SudokuSolverApp
{
    class Program
    {
        static void Main()
        {
            Application.Run(new MainScreen());
        }
    }

    class MainScreen : Form
    {
        Label titlelabel = new Label();
        Label inputlabel = new Label();
        Label outputlabel = new Label();
        Label signlabel = new Label();
        Label hand = new Label();
        Label explainlabel = new Label();
        Label steplabel = new Label();
        Label strategylabel = new Label();
        Label difficultylabel = new Label();

        TextBox inputfield = new TextBox();
        TextBox outputfield = new TextBox();

        Button convertbutton = new Button();

        Button easybutton = new Button();
        Button mediumbutton = new Button();
        Button hardbutton = new Button();
        Button solvebutton = new Button();
        Button helpbutton = new Button();
        Button stepbutton = new Button();
        Button skipbutton = new Button();

        Size fieldsize = new Size(580, 50);
        Size buttonsize = new Size(130, 130);
        Size smallbuttonsize = new Size(60, 60);

        Cell[] cells = new Cell[81];
        Cell[] helpcells = new Cell[81];
        Sudoku sudoku;
        Node[] graph = new Node[7];
        PictureBox[] pbs = new PictureBox[81];
        Bitmap[] bs = new Bitmap[81];
        Graphics[] gs = new Graphics[81];

        PictureBox highlightedcell;

        Queue<Move> helpmoves = new Queue<Move>();

        CheckBox[] strategyboxes = new CheckBox[18];
        CheckBox CBEbox = new CheckBox(), PMD1box = new CheckBox(), PMD2box = new CheckBox(), PMD3box = new CheckBox(),
                 HBEbox = new CheckBox(), LRCbox = new CheckBox(), PMIbox = new CheckBox(), N2Ebox = new CheckBox(),
                 N3Ebox = new CheckBox(), P2Ebox = new CheckBox(), P3Ebox = new CheckBox(), H2Ebox = new CheckBox(),
                 H3Ebox = new CheckBox(), XWEbox = new CheckBox(), XYWEbox = new CheckBox(), XYZWEbox = new CheckBox(),
                 SFEbox = new CheckBox(), JFEbox = new CheckBox();

        Allowed goal = new Allowed(false);
        Allowed helpallowed = new Allowed(false);
        Allowed shadowsolve = new Allowed(false);

        Allowed CBEallowed = new Allowed(true), PMD1allowed = new Allowed(true), PMD2allowed = new Allowed(true), PMD3allowed = new Allowed(true),
                HBEallowed = new Allowed(true), LRCallowed = new Allowed(true), PMIallowed = new Allowed(true), N2Eallowed = new Allowed(true),
                N3Eallowed = new Allowed(true), P2Eallowed = new Allowed(true), P3Eallowed = new Allowed(true), H2Eallowed = new Allowed(true),
                H3Eallowed = new Allowed(true), XWEallowed = new Allowed(true), XYWEallowed = new Allowed(true), XYZWEallowed = new Allowed(true),
                SFEallowed = new Allowed(true), JFEallowed = new Allowed(true);

        Allowed CBEskip = new Allowed(false), PMDskip = new Allowed(false), HBEskip = new Allowed(false), LRCskip = new Allowed(false),
                PMIskip = new Allowed(false), N2Eskip = new Allowed(false), N3Eskip = new Allowed(false), PTEskip = new Allowed(false),
                H2Eskip = new Allowed(false), H3Eskip = new Allowed(false), XWEskip = new Allowed(false), XYWEskip = new Allowed(false),
                XYZWEskip = new Allowed(false), SFEskip = new Allowed(false), JFEskip = new Allowed(false), AIskip = new Allowed(true);

        Allowed[] skips;

        Pen blackpen = new Pen(Color.Black), darkgreenpen = new Pen(Color.DarkGreen), darkredpen = new Pen(Color.DarkRed, 3);

        Font fieldfont = new Font("Tahoma", 10);
        Font pmfont = new Font("Tahoma", 16);
        Font digitfont = new Font("Tahoma", 20);
        Font smallbuttonfont = new Font("Tahoma", 8, FontStyle.Bold);
        Font buttonfont = new Font("Courier New", 16, FontStyle.Bold);
        Font titlefont = new Font("Tahoma", 30, FontStyle.Bold | FontStyle.Underline);
        Font fieldlabelfont = new Font("Tahoma", 11, FontStyle.Bold);
        Font steplabelfont = new Font("Tahoma", 14, FontStyle.Bold);
        Font handfont = new Font("Courier New", 12, FontStyle.Bold);
        Font difficultyfont = new Font("Courier New", 10, FontStyle.Bold);

        string[] easysudokus = new string[3];
        string[] mediumsudokus = new string[7];
        string[] hardsudokus = new string[7];

        string thinkingtext = "I'm thinking for a moment! Hold on.";

        public MainScreen()
        {

            //Screen info
            Width = 1600;
            Height = 940;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            //MinimizeBox = false;

            string starttext = "Hi there! " +
                "\n\nI am the Helping HAND (Heuristical Alethic Natural Deduction) program! I can help you solve a sudoku, based on looking for the most optimal logical solving strategies." +
                "\n\nYou can choose to import and create a sudoku from a string written in the input field, or you can create a random sudoku of difficulty of choice by pressing the FROM INPUT, EASY, MEDIUM and HARD buttons." +
                "\n\nIn solving the sudoku I will only use the selected strategies, so you can toggle whichever strategies you do or don't know how to use." +
                "\n\nAfterwards you can press the SOLVE button if you want me to solve the sudoku in one go, or you can press the HELP button. Then I will solve the sudoku step by step and explain why I choose each step." +

                "\n\nFor more information on these strategies or my algorithm, read Daan Di Scala's bachelor thesis. " +
                "\n\nGood Luck!";

            //Visual Sudoku
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Size s = new Size(70, 70);
                    PictureBox p = new PictureBox();
                    Bitmap b = new Bitmap(70, 70);
                    Graphics g = Graphics.FromImage(b);

                    pbs[i * 9 + j] = p;
                    bs[i * 9 + j] = b;
                    gs[i * 9 + j] = g;

                    p.Size = s;
                    p.BackColor = Color.White;
                    p.Image = b;
                    p.Location = new Point(102 + j * 75, 152 + i * 75);
                    p.MouseMove += Hovers;
                    p.MouseLeave += Leaves;
                    Controls.Add(p);
                }
            }
            Paint += DrawSudoku;

            //Sudokulists
            easysudokus = new string[3]   {"090600000060000804100000000002030700000072000080000000000840001037090020008050043",
                                           "000080000025000030300001200003600000800230010647000008470000000000020600100000045",
                                           "000030000400010000008000040000000076000060000012003005260009430070000060093500080"};
            mediumsudokus = new string[7] {"920000000300000790000008200740000000030160000000000051070200304006700005000005800",
                                           "000002003700500060009000804940000000500030000000800007000680095006007000000450170",
                                           "200007081000000670500040000001003000002600800008204005470000900000890000005000006",
                                           "000082000007000003080019005001040000203000010000006900005430700000600408100000050",
                                           "000008090900020030804700000000036000009000060045000270060001009000200000280000500",
                                           "009000030600000504040080000080000060000060009210000400000004781100070006000095000",
                                           "010400200000000000000500437080310000003084060000000004090000000070006590000900703"};
            hardsudokus = new string[7]   {"004000000000006052001003700082000400300910000000000520003000286690000000000504000",
                                           "030590100000000780060000040700008500900300000300002000001040096000010004002000001",
                                           "000009560004001000600000080420000005007680000000030009000400208371000000000500000",
                                           "000570000920000000000000938014000000050800014000600050200000700800000600000103000",
                                           "900001040000600030080090007006000003000000090050174000000710004004002000067803005",
                                           "600000070005900000090204100002000009050600000000000041000060800040028000039400050",
                                           "320000041600009000910000037004580009000037000000000004000000000030042000000060850"};
            skips = new Allowed[15] { CBEskip, PMDskip, HBEskip, LRCskip,
                PMIskip, N2Eskip, N3Eskip, PTEskip, H2Eskip, H3Eskip,
                XWEskip, XYWEskip, XYZWEskip, SFEskip, JFEskip };

            //LABELS
            hand.Text = "    _.-._ \n   | | | |_ \n   | | | | | \n   | | | | | \n _ |  '-._ | \n \\`\\`-.'-._; \n  \\    '   | \n   \\  .`  / \n    |    | \n";
            hand.Font = handfont;
            hand.Size = new Size(800, 200);
            hand.Location = new Point(1400, 200);
            Controls.Add(hand);

            titlelabel.Text = "Sudoku Solver: Helping HAND";
            titlelabel.Font = titlefont;
            titlelabel.Size = new Size(800, 50);
            titlelabel.Location = new Point(120, 20);
            Controls.Add(titlelabel);

            inputlabel.Text = "Input:";
            inputlabel.Font = fieldlabelfont;
            inputlabel.Size = new Size(60, 22);
            inputlabel.Location = new Point(130, 110);
            Controls.Add(inputlabel);

            outputlabel.Text = "Output:";
            outputlabel.Font = fieldlabelfont;
            outputlabel.Size = new Size(70, 22);
            outputlabel.Location = new Point(120, 850);
            Controls.Add(outputlabel);

            strategylabel.Text = "Strategies";
            strategylabel.Font = fieldlabelfont;
            strategylabel.Size = new Size(100, 25);
            strategylabel.Location = new Point(850, 70);
            Controls.Add(strategylabel);

            difficultylabel.Text = "--\nT\ni\ne\nr\n\n0\n\n\n\n\n\n\n\n\n\n\n\n\n--\nT\ni\ne\nr\n\n1\n\n--\nT\ni\ne\nr\n\n2\n\n\n\n\n--\nT\ni\ne\nr\n\n3\n\n--\nT\ni\ne\nr\n\n4";
            difficultylabel.Font = difficultyfont;
            difficultylabel.Size = new Size(40, 800);
            difficultylabel.Location = new Point(1010, 90);
            Controls.Add(difficultylabel);

            signlabel.Text = "Made by Daan Di Scala (2020), Merel de Goede (2022)";
            signlabel.Font = fieldlabelfont;
            signlabel.Size = new Size(500, 30);
            signlabel.Location = new Point(220, 75);
            Controls.Add(signlabel);

            explainlabel.Text = starttext;
            explainlabel.Font = fieldlabelfont;
            explainlabel.Size = new Size(400, 600);
            explainlabel.Location = new Point(1100, 400);
            Controls.Add(explainlabel);

            steplabel.Text = "";
            steplabel.Font = steplabelfont;
            steplabel.Size = new Size(200, 300);
            steplabel.Location = new Point(1100, 250);
            Controls.Add(steplabel);

            //FIELDS
            inputfield.Text = "005400602006020150293561784052314800301206405000057320030042560024005900507009240";
            inputfield.Font = fieldfont;
            inputfield.Size = fieldsize;
            inputfield.Location = new Point(190, 110);
            Controls.Add(inputfield);

            outputfield.Text = "";
            outputfield.Font = fieldfont;
            outputfield.Size = fieldsize;
            outputfield.Location = new Point(190, 850);
            Controls.Add(outputfield);

            //BUTTONS
            convertbutton.Text = "FROM INPUT";
            convertbutton.Font = smallbuttonfont;
            convertbutton.Cursor = Cursors.Hand;
            convertbutton.Size = smallbuttonsize;
            convertbutton.BackColor = Color.LightBlue;
            convertbutton.Location = new Point(1110, 40);
            convertbutton.Click += ConvertStringToSudoku;
            Controls.Add(convertbutton);

            easybutton.Text = "EASY";
            easybutton.Font = smallbuttonfont;
            easybutton.Cursor = Cursors.Hand;
            easybutton.Size = smallbuttonsize;
            easybutton.BackColor = Color.LightGreen;
            easybutton.Location = new Point(1180, 40);
            easybutton.Click += PickEasy;
            Controls.Add(easybutton);

            mediumbutton.Text = "MEDIUM";
            mediumbutton.Font = smallbuttonfont;
            mediumbutton.Cursor = Cursors.Hand;
            mediumbutton.Size = smallbuttonsize;
            mediumbutton.BackColor = Color.LightGoldenrodYellow;
            mediumbutton.Location = new Point(1110, 110);
            mediumbutton.Click += PickMedium;
            Controls.Add(mediumbutton);

            hardbutton.Text = "HARD";
            hardbutton.Font = smallbuttonfont;
            hardbutton.Cursor = Cursors.Hand;
            hardbutton.Size = smallbuttonsize;
            hardbutton.BackColor = Color.LightSalmon;
            hardbutton.Location = new Point(1180, 110);
            hardbutton.Click += PickHard;
            Controls.Add(hardbutton);

            solvebutton.Text = "SOLVE";
            solvebutton.Font = buttonfont;
            solvebutton.Cursor = Cursors.Hand;
            solvebutton.Size = buttonsize;
            solvebutton.BackColor = Color.LightBlue;
            solvebutton.Location = new Point(1250, 40);
            solvebutton.Click += SolveClicked;
            Controls.Add(solvebutton);

            helpbutton.Text = "HELP";
            helpbutton.Font = buttonfont;
            helpbutton.Cursor = Cursors.Hand;
            helpbutton.Size = buttonsize;
            helpbutton.BackColor = Color.PaleGoldenrod;
            helpbutton.Location = new Point(1390, 40);
            helpbutton.Click += HelpClicked;
            Controls.Add(helpbutton);

            stepbutton.Text = "STEP";
            stepbutton.Font = smallbuttonfont;
            stepbutton.Cursor = Cursors.Hand;
            stepbutton.Size = smallbuttonsize;
            stepbutton.BackColor = Color.PaleGoldenrod;
            stepbutton.Location = new Point(1310, 240);
            stepbutton.Click += StepClicked;
            //Controls.Add(stepbutton);

            skipbutton.Text = "SKIP";
            skipbutton.Font = smallbuttonfont;
            skipbutton.Cursor = Cursors.Hand;
            skipbutton.Size = smallbuttonsize;
            skipbutton.BackColor = Color.LightCoral;
            skipbutton.Location = new Point(1310, 310);
            skipbutton.Click += Skip;
            //Controls.Add(skipbutton);

            //CHECKBOXES
            strategyboxes[0] = CBEbox; CBEbox.Click += CBEclick;
            strategyboxes[1] = PMD1box; PMD1box.Click += PMD1click;
            strategyboxes[2] = PMD2box; PMD2box.Click += PMD2click;
            strategyboxes[3] = PMD3box; PMD3box.Click += PMD3click;
            strategyboxes[4] = HBEbox; HBEbox.Click += HBEclick;
            strategyboxes[5] = LRCbox; LRCbox.Click += LRCclick;
            strategyboxes[6] = PMIbox; PMIbox.Click += PMIclick;
            strategyboxes[7] = N2Ebox; N2Ebox.Click += N2Eclick;
            strategyboxes[8] = P2Ebox; P2Ebox.Click += P2Eclick;
            strategyboxes[9] = H2Ebox; H2Ebox.Click += H2Eclick;
            strategyboxes[10] = N3Ebox; N3Ebox.Click += N3Eclick;
            strategyboxes[11] = P3Ebox; P3Ebox.Click += P3Eclick;
            strategyboxes[12] = H3Ebox; H3Ebox.Click += H3Eclick;           //MEREL
            strategyboxes[13] = XWEbox; XWEbox.Click += XWEclick;
            strategyboxes[14] = XYWEbox; XYWEbox.Click += XYWEclick;        //MEREL : not succeeded to get this method working
            strategyboxes[15] = XYZWEbox; XYZWEbox.Click += XYZWEclick;     //MEREL : not succeeded to get this method working
            strategyboxes[16] = SFEbox; SFEbox.Click += SFEclick;           //MEREL
            strategyboxes[17] = JFEbox; JFEbox.Click += JFEclick;           //MEREL

            for (int i = 0; i < strategyboxes.Length; i++)
            {
                strategyboxes[i].Location = new Point(800, 100 + i * 41);
                strategyboxes[i].Appearance = Appearance.Button;
                strategyboxes[i].Font = smallbuttonfont;
                strategyboxes[i].Cursor = Cursors.Hand;
                strategyboxes[i].Size = new Size(200, 30);
                strategyboxes[i].Checked = true;
                Controls.Add(strategyboxes[i]);
            }

            CheckClicked(CBEbox, "Cell Based Elimination", CBEallowed);
            CheckClicked(PMD1box, "Pencil Mark Duality (Single)", PMD1allowed);
            CheckClicked(PMD2box, "Pencil Mark Duality (Pair)", PMD2allowed);
            CheckClicked(PMD3box, "Pencil Mark Duality (Triple)", PMD3allowed);
            CheckClicked(HBEbox, "House Based Elimination", HBEallowed);
            CheckClicked(LRCbox, "Last Remaining Cell", LRCallowed);
            CheckClicked(PMIbox, "Pencil Mark Introduction", PMIallowed);
            CheckClicked(N2Ebox, "Naked Pairs Elimination", N2Eallowed);
            CheckClicked(N3Ebox, "Naked Triples Elimination", N3Eallowed);
            CheckClicked(P2Ebox, "Pointing Pairs Elimination", P2Eallowed);
            CheckClicked(P3Ebox, "Pointing Triples Elimination", P3Eallowed);
            CheckClicked(H2Ebox, "Hidden Pairs Elimination", H2Eallowed);
            CheckClicked(H3Ebox, "Hidden Triples Elimination", H3Eallowed);     //MEREL
            CheckClicked(XWEbox, "X-Wing Elimination", XWEallowed);
            CheckClicked(XYWEbox, "XY-Wing Elimination", XYWEallowed);          //MEREL : not succeeded to get this method working
            CheckClicked(XYZWEbox, "XYZ-Wing Elimination", XYZWEallowed);       //MEREL : not succeeded to get this method working
            CheckClicked(SFEbox, "Swordfish Elimination", SFEallowed);          //MEREL
            CheckClicked(JFEbox, "Jellyfish Elimination", JFEallowed);          //MEREL

            //Create Graph
            for (int i = 0; i < 7; i++)
                graph[i] = new Node(i);

            //ORDER MATTERS HERE!
            //v0
            Edge PMIedge = new Edge(graph[0], graph[2], PMI, 0, PMIallowed);
            graph[0].outgoingEdges.Add(PMIedge);
            //v1
            Edge endedge = new Edge(graph[1], graph[1], CheckGoal, 0, new Allowed(true));
            graph[1].outgoingEdges.Add(endedge);
            Edge CBEedge = new Edge(graph[1], graph[4], CBE, 0, CBEallowed);
            graph[1].outgoingEdges.Add(CBEedge);
            Edge HBEedge = new Edge(graph[1], graph[4], HBE, 0, HBEallowed);
            graph[1].outgoingEdges.Add(HBEedge);
            //v2
            Edge AIedge1 = new Edge(graph[2], graph[3], AndIntroduction, 0, new Allowed(true));
            graph[2].outgoingEdges.Add(AIedge1);
            Edge LRCedge = new Edge(graph[2], graph[1], LRC, 0, LRCallowed);
            graph[2].outgoingEdges.Add(LRCedge);
            Edge XWEedge = new Edge(graph[2], graph[4], XWE, 0, XWEallowed);
            graph[2].outgoingEdges.Add(XWEedge);
            //Edge XYWEedge = new Edge(graph[2], graph[4], XYWE, 0, XYWEallowed);           //MEREL : not succeeded to get this method working
            //graph[2].outgoingEdges.Add(XYWEedge);                                         //MEREL : not succeeded to get this method working
            //Edge XYZWEedge = new Edge(graph[2], graph[4], XYZWE, 0, XYZWEallowed);        //MEREL : not succeeded to get this method working
            //graph[2].outgoingEdges.Add(XYZWEedge);                                        //MEREL : not succeeded to get this method working
            Edge SFEedge = new Edge(graph[2], graph[4], SFE, 0, SFEallowed);                //MEREL
            graph[2].outgoingEdges.Add(SFEedge);                                            //MEREL
            Edge JFEedge = new Edge(graph[2], graph[4], JFE, 0, JFEallowed);                //MEREL
            graph[2].outgoingEdges.Add(JFEedge);                                            //MEREL
            //v3
            Edge PMD1edge = new Edge(graph[3], graph[1], PMD, 1, PMD1allowed);
            graph[3].outgoingEdges.Add(PMD1edge);
            Edge PMD2edge = new Edge(graph[3], graph[5], PMD, 2, PMD2allowed);
            graph[3].outgoingEdges.Add(PMD2edge);
            Edge PMD3edge = new Edge(graph[3], graph[6], PMD, 3, PMD3allowed);
            graph[3].outgoingEdges.Add(PMD3edge);
            Edge P2Eedge = new Edge(graph[3], graph[4], PTE, 2, P2Eallowed);
            graph[3].outgoingEdges.Add(P2Eedge);
            Edge P3Eedge = new Edge(graph[3], graph[4], PTE, 3, P3Eallowed);
            graph[3].outgoingEdges.Add(P3Eedge);
            Edge H2Eedge = new Edge(graph[3], graph[4], H2E, 0, H2Eallowed);
            graph[3].outgoingEdges.Add(H2Eedge);
            Edge H3Eedge = new Edge(graph[3], graph[4], H3E, 0, H3Eallowed);                //MEREL
            graph[3].outgoingEdges.Add(H3Eedge);                                            //MEREL
            //v4
            Edge AIedge2 = new Edge(graph[4], graph[3], AndIntroduction, 0, new Allowed(true));
            graph[4].outgoingEdges.Add(AIedge2);
            //v5
            Edge N2Eedge = new Edge(graph[5], graph[4], N2E, 0, N2Eallowed);
            graph[5].outgoingEdges.Add(N2Eedge);
            Edge N3Eedge1 = new Edge(graph[6], graph[4], N3E, 0, N3Eallowed);
            graph[6].outgoingEdges.Add(N3Eedge1);
            //v6
            Edge N3Eedge2 = new Edge(graph[6], graph[4], N3E, 0, N3Eallowed);
            graph[6].outgoingEdges.Add(N3Eedge2);
        }

        //SUDOKU HOVER METHODS
        void Hovers(object o, MouseEventArgs mea)
        {
            foreach (PictureBox pb in pbs)
                if (o.Equals(pb))
                    pb.BackColor = Color.LightBlue;
        }

        void Leaves(object o, EventArgs ea)
        {
            foreach (PictureBox pb in pbs)
                if (o.Equals(pb))
                    if (o.Equals(highlightedcell))
                        pb.BackColor = Color.LightGoldenrodYellow;
                    else
                        pb.BackColor = Color.White;
        }

        void DrawSudoku(object o, PaintEventArgs e)
        {
            Color black = Color.Black;
            Pen small = new Pen(black);
            Pen big = new Pen(black, 4);
            Brush whitebrush = Brushes.White;
            e.Graphics.FillRectangle(whitebrush, 100, 150, 675, 675);
            for (int i = 0; i < 10; i++)
            {
                e.Graphics.DrawLine(small, new Point(100, 150 + 75 * i), new Point(775, 150 + 75 * i));
                e.Graphics.DrawLine(small, new Point(100 + 75 * i, 150), new Point(100 + 75 * i, 825));
            }
            for (int i = 0; i < 10; i += 3)
            {
                e.Graphics.DrawLine(big, new Point(100, 150 + 75 * i), new Point(775, 150 + 75 * i));
                e.Graphics.DrawLine(big, new Point(100 + 75 * i, 150), new Point(100 + 75 * i, 825));
            }
        }

        //CHECKBOX METHODS
        void CBEclick(object o, EventArgs ea) { CheckClicked(CBEbox, "Cell Based Elimination", CBEallowed); }
        void PMD1click(object o, EventArgs ea) { CheckClicked(PMD1box, "Pencil Mark Duality (Single)", PMD1allowed); }
        void PMD2click(object o, EventArgs ea) { CheckClicked(PMD2box, "Pencil Mark Duality (Pair)", PMD2allowed); }
        void PMD3click(object o, EventArgs ea) { CheckClicked(PMD3box, "Pencil Mark Duality (Triple)", PMD3allowed); }
        void HBEclick(object o, EventArgs ea) { CheckClicked(HBEbox, "House Based Elimination", HBEallowed); }
        void LRCclick(object o, EventArgs ea) { CheckClicked(LRCbox, "Last Remaining Cell", LRCallowed); }
        void PMIclick(object o, EventArgs ea) { CheckClicked(PMIbox, "Pencil Mark Introduction", PMIallowed); }
        void N2Eclick(object o, EventArgs ea) { CheckClicked(N2Ebox, "Naked Pairs Elimination", N2Eallowed); }
        void N3Eclick(object o, EventArgs ea) { CheckClicked(N3Ebox, "Naked Triples Elimination", N3Eallowed); }
        void P2Eclick(object o, EventArgs ea) { CheckClicked(P2Ebox, "Pointing Pairs Elimination", P2Eallowed); }
        void P3Eclick(object o, EventArgs ea) { CheckClicked(P3Ebox, "Pointing Triples Elimination", P3Eallowed); }
        void H2Eclick(object o, EventArgs ea) { CheckClicked(H2Ebox, "Hidden Pairs Elimination", H2Eallowed); }
        void H3Eclick(object o, EventArgs ea) { CheckClicked(H3Ebox, "Hidden Triples Elimination", H3Eallowed); }       //MEREL
        void XWEclick(object o, EventArgs ea) { CheckClicked(XWEbox, "X-Wing Elimination", XWEallowed); }
        void XYWEclick(object o, EventArgs ea) { CheckClicked(XYWEbox, "XY-Wing Elimination", XYWEallowed); }           //MEREL
        void XYZWEclick(object o, EventArgs ea) { CheckClicked(XYZWEbox, "XYZ-Wing Elimination", XYZWEallowed); }       //MEREL
        void SFEclick(object o, EventArgs ea) { CheckClicked(SFEbox, "Swordfish Elimination", SFEallowed); }            //MEREL
        void JFEclick(object o, EventArgs ea) { CheckClicked(JFEbox, "Jellyfish Elimination", JFEallowed); }            //MEREL

        void CheckClicked(CheckBox cb, string s, Allowed a)
        {
            if (cb.Checked)
            {
                cb.Text = s + ": On";
                cb.BackColor = Color.Aquamarine;
                a.b = true;
            }
            else
            {
                cb.Text = s + ": Off";
                cb.BackColor = Color.DarkSalmon;
                a.b = false;
            }
        }

        void ConvertStringToSudoku(object o, EventArgs ea)
        {
            steplabel.Text = "";
            Controls.Remove(stepbutton);
            Controls.Remove(skipbutton);
            helpbutton.BackColor = Color.PaleGoldenrod;
            helpbutton.Text = "HELP";
            explainlabel.Text = "You've chosen a to import a Sudoku from an input string." +
                "\n\nYou can toggle your preferred strategies and then press the SOLVE or HELP button." +
                "\n\nIn solving the sudoku I will only use the selected strategies, so you can toggle whichever strategies you do or don't know how to use." +
                "\n\nAfterwards you can press the SOLVE button if you want me to solve the sudoku in one go, or you can press the HELP button. Then I will solve the sudoku step by step and explain why I choose each step.";

            //Read Input
            string read = inputfield.Text;
            if (read.Length == 81)
                cells = CellListMaker(read);
            else
                inputfield.Text = "WRITE YOUR SUDOKU HERE FIRST";



        }

        void UpdateCell(int cellname)
        {
            //Bitmap b = new Bitmap(70, 70);
            //Graphics g = Graphics.FromImage(b);
            Cell cell = cells[cellname];
            Graphics g = gs[cell.name];
            g.Clear(Color.Transparent);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            if (cell.given)
                g.DrawString(cell.box_list[0].ToString(), digitfont, Brushes.Black, 25, 25);
            else if (cell.box_list.Count == 1)
            {
                g.DrawString(cell.box_list[0].ToString(), digitfont, Brushes.DarkGreen, 25, 25);
            }
            else
                for (int y = 0; y < 3; y++)
                    for (int x = 0; x < 3; x++)
                    {
                        int i = 1 + x + y * 3;
                        if (cell.diamond_set.Contains(i))
                            g.DrawString(i.ToString(), pmfont, Brushes.DarkGreen, x * 22, y * 22);
                        if (cell.negdiamond_set.Contains(i))
                        {
                            g.DrawString(i.ToString(), pmfont, Brushes.DarkRed, x * 22, y * 22);
                            g.DrawLine(darkredpen, x * 22, y * 22 + 16, x * 22 + 16, y * 22);
                        }
                    }
            gs[cell.name].Flush();
            pbs[cell.name].Image = bs[cell.name];
            pbs[cell.name].Update();
        }

        void SolveClicked(object o, EventArgs ea)
        {
            if (cells[0] == null)
            {
                steplabel.Text = "Choose a Sudoku first! You can either press the FROM INPUT, EASY, MEDIUM or HARD button.";
            }
            else if (helpbutton.Text != "HELPING")
            {
                steplabel.Text = "";
                steplabel.Refresh();
                helpallowed.b = false;
                shadowsolve.b = false;
                solvebutton.BackColor = Color.DarkBlue;
                solvebutton.Text = "SOLVING";
                helpbutton.Text = "HELP";
                helpbutton.BackColor = Color.PaleGoldenrod;
                solvebutton.ForeColor = Color.White;
                solvebutton.Refresh();
                Solve();
            }
        }

        void HelpClicked(object o, EventArgs ea)
        {
            if (cells[0] == null)
            {
                steplabel.Text = "Choose a Sudoku first! You can either press the FROM INPUT, EASY, MEDIUM or HARD button.";
            }
            else if (helpbutton.Text != "HELPING")
            {
                foreach (Allowed skip in skips)
                    skip.b = false;
                AIskip.b = true;

                steplabel.Text = thinkingtext;
                steplabel.Update();
                helpallowed.b = true;
                shadowsolve.b = true;
                helpbutton.BackColor = Color.Gold;
                helpbutton.Text = "HELPING";
                helpbutton.Refresh();
                Solve();

            }
        }

        void Solve()
        {
            //Create Sudoku
            Cell[][] horizontals = GroupMaker(cells, "h");
            Cell[][] verticals = GroupMaker(cells, "v");
            Cell[][] blocks = GroupMaker(cells, "b");
            Cell[][] lines = CombineArray(horizontals, verticals);
            Cell[][] groups = CombineArray(lines, blocks);

            sudoku = new Sudoku(cells, horizontals, verticals, blocks, lines, groups);

            goal.b = false;
            //Start searching!
            SudokuSolveSearch(graph[0], graph[1]);

            //Write solution
            if (!helpallowed.b)
            {
                SudokuWriter(cells);
                //Refresh buttons
                solvebutton.Text = "SOLVE";
                solvebutton.BackColor = Color.LightBlue;
                solvebutton.ForeColor = Color.Black;
            }
            else
            {
                //perform first step
                if (helpmoves.Count != 0)
                {
                    steplabel.Text = "";
                    steplabel.Update();
                    shadowsolve.b = false;
                    cells = CellListMaker(inputfield.Text);
                    horizontals = GroupMaker(cells, "h");
                    verticals = GroupMaker(cells, "v");
                    blocks = GroupMaker(cells, "b");
                    lines = CombineArray(horizontals, verticals);
                    groups = CombineArray(lines, blocks);
                    sudoku = new Sudoku(cells, horizontals, verticals, blocks, lines, groups);
                    goal.b = false;
                    ShowNextStep(helpmoves.Peek());
                    Controls.Add(stepbutton);
                    Controls.Add(skipbutton);
                }
                else
                {
                    steplabel.Text = "The Sudoku cannot be solved any further!";
                }

            }
        }

        void SudokuSolveSearch(Node v0, Node v1)
        {
            Stack<Tuple<Node, Cell>> St = new Stack<Tuple<Node, Cell>>();

            foreach (Cell c in sudoku.cells)
            {
                if (c.box_list.Count == 0)
                    St.Push(new Tuple<Node, Cell>(v0, c));
                else
                    St.Push(new Tuple<Node, Cell>(v1, c));
            }

            while (St.Count != 0 && !goal.b)
            {
                Tuple<Node, Cell> vc = St.Pop();

                for (int i = 0; i < vc.Item1.outgoingEdges.Count; i++)
                {
                    Edge e = vc.Item1.outgoingEdges[i];
                    if (e.isAllowed.b)
                    {
                        List<Move> L = new List<Move>();
                        L = e.strategy(vc.Item2, e.strategy_n);

                        if (L.Count > 1)
                        {
                            St.Push(vc);
                        }
                        if (L.Count != 0)
                        {
                            Move move = L[0];
                            if (helpallowed.b)
                                helpmoves.Enqueue(move); //save move
                            ApplyMove(move);
                            St.Push(new Tuple<Node, Cell>(e.to, cells[move.cellname_to_update]));
                        }
                    }
                }
            }
        }

        void ShowNextStep(Move move)
        {
            steplabel.Text = "The next strategy I would advise to perform is: " +
                "\n" + move.strategyname;

            string cellmodality = "";
            switch (move.modality)
            {
                case "d": cellmodality = "place Pencil Mark " + move.PM; break;
                case "-d": cellmodality = "remove Pencil Mark " + move.PM; break;
                case "I": cellmodality = "do nothing"; break;
                case "b":
                    if (move.PM != 0)
                        cellmodality = "place Digit " + move.PM;
                    else
                        cellmodality = "change nothing";
                    break;
            }

            explainlabel.Text = "This suggestion is because strategy " + move.strategyname + " is the easiest strategy I found to do next, after the previous step. " +
                                "\n\nAlso, I would recommend to perform " + move.explainmove + "."
                                + "\n\nThis would " + cellmodality + " in cell c" + move.cellname_to_update + "."
                                + "\n\nDo you want me to do execute this strategy? Press the STEP button." +
                                "\n\nDo you want me to skip over every step of this strategy? Press the SKIP button.";

            pbs[move.cellname_to_update].BackColor = Color.LightGoldenrodYellow;
            highlightedcell = pbs[move.cellname_to_update];
        }

        void StepClicked(object o, EventArgs ea)
        {
            Step();
        }

        void Step()
        {
            Move move = new Move();
            bool finished = false;

            if (helpmoves.Count != 0)
            {
                move = helpmoves.Dequeue();
                ApplyMove(move);
                Move nextmove = helpmoves.Peek();

                while (nextmove.skip.b && !finished)
                {
                    if (helpmoves.Count != 0)
                    {
                        nextmove = helpmoves.Dequeue();
                        ApplyMove(nextmove);
                    }
                    else finished = true;

                    if (helpmoves.Count != 0)
                        nextmove = helpmoves.Peek();
                    else finished = true;
                }
                ShowNextStep(nextmove);
            }
            else finished = true;

            if (finished)
            {
                helpbutton.Text = "HELP";
                helpbutton.BackColor = Color.PaleGoldenrod;
                steplabel.Text = "Finished!";
                explainlabel.Text = "";
                Controls.Remove(stepbutton);
                Controls.Remove(skipbutton);
            }
        }

        void Skip(object o, EventArgs ea)
        {
            Move move = helpmoves.Peek();
            move.skip.b = true;
            Step();
        }

        void ApplyMove(Move move)
        {
            pbs[move.cellname_to_update].BackColor = Color.Transparent;

            switch (move.modality)
            {
                case "-d": cells[move.cellname_to_update].NegdiamondAdd(move.PM); break;
                case "d": cells[move.cellname_to_update].DiamondAdd(move.PM); break;
                case "b": cells[move.cellname_to_update].BoxAdd(move.PMs); break;
                case "I":; break;
            }
            if (!shadowsolve.b)
            {
                UpdateCell(move.cellname_to_update);
            }
        }

        void PickEasy(object o, EventArgs ea)
        {
            Random rnd = new Random();
            int p = rnd.Next(3);
            inputfield.Text = easysudokus[p];
            cells = CellListMaker(inputfield.Text);
            steplabel.Text = "";
            Controls.Remove(stepbutton);
            Controls.Remove(skipbutton);
            helpbutton.BackColor = Color.PaleGoldenrod;
            helpbutton.Text = "HELP";
            explainlabel.Text = "You've chosen a Sudoku of Easy difficulty. I've selected an Easy Sudoku based on the solving strategies." +
                "\n\nIf you use all the strategies up to Beginner level, I guarantee you that you can solve the current sudoku. " +
                "If you choose another subset of strategies, you might be able to solve the sudoku, but not guaranteed." +
                "\n\nYou can toggle your preferred strategies and then press the SOLVE or HELP button." +
                "\n\nIn solving the sudoku I will only use the selected strategies, so you can toggle whichever strategies you do or don't know how to use." +
                "\n\nAfterwards you can press the SOLVE button if you want me to solve the sudoku in one go, or you can press the HELP button. Then I will solve the sudoku step by step and explain why I choose each step.";
        }

        void PickMedium(object o, EventArgs ea)
        {
            Random rnd = new Random();
            int p = rnd.Next(7);
            inputfield.Text = mediumsudokus[p];
            cells = CellListMaker(inputfield.Text);
            steplabel.Text = "";
            Controls.Remove(stepbutton);
            Controls.Remove(skipbutton);
            helpbutton.BackColor = Color.PaleGoldenrod;
            helpbutton.Text = "HELP";
            explainlabel.Text = "You've chosen a Sudoku of Medium difficulty. I've selected an Medium Sudoku based on the solving strategies." +
                "\n\nIf you use all the strategies up to Intermediate level, I guarantee you that you can solve the current sudoku. " +
                "If you choose another subset of strategies, you might be able to solve the sudoku, but not guaranteed." +
                "\n\nYou can toggle your preferred strategies and then press the SOLVE or HELP button." +
                "\n\nIn solving the sudoku I will only use the selected strategies, so you can toggle whichever strategies you do or don't know how to use." +
                "\n\nAfterwards you can press the SOLVE button if you want me to solve the sudoku in one go, or you can press the HELP button. Then I will solve the sudoku step by step and explain why I choose each step.";
        }

        void PickHard(object o, EventArgs ea)
        {
            Random rnd = new Random();
            int p = rnd.Next(7);
            inputfield.Text = hardsudokus[p];
            cells = CellListMaker(inputfield.Text);
            steplabel.Text = "";
            Controls.Remove(stepbutton);
            Controls.Remove(skipbutton);
            helpbutton.BackColor = Color.PaleGoldenrod;
            helpbutton.Text = "HELP";
            explainlabel.Text = "You've chosen a Sudoku of Hard difficulty. I've selected an Hard Sudoku based on the solving strategies." +
                "\n\nIf you use all the strategies up to Advanced level, I guarantee you that you can solve the current sudoku. " +
                "If you choose another subset of strategies, you might be able to solve the sudoku, but not guaranteed." +
                "\n\nYou can toggle your preferred strategies and then press the SOLVE or HELP button." +
                "\n\nIn solving the sudoku I will only use the selected strategies, so you can toggle whichever strategies you do or don't know how to use." +
                "\n\nAfterwards you can press the SOLVE button if you want me to solve the sudoku in one go, or you can press the HELP button. Then I will solve the sudoku step by step and explain why I choose each step.";
        }

        //STRATEGY METHODS
        //Edge cases
        List<Move> AndIntroduction(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            if (updated_cell.diamond_set.Count != 0 && updated_cell.negdiamond_set.Count != 0)
                moves.Add(new Move(updated_cell.name, "I", 0, AIskip, "AND INTRODUCTION", "AND INTRODUCTION because c" + updated_cell.name + " contains both phi and neg phi"));
            return moves;
        }

        List<Move> CheckGoal(Cell updated_cell, int n)
        {
            int counter = 0;
            foreach (Cell cell in sudoku.cells)
                if (cell.filled)
                    counter++;
            if (counter == sudoku.cells.Length)
                goal.b = true;

            return new List<Move>();
        }

        /*---------------------------------------2.3.1---------------------------------------*/
        List<Move> CBE(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            Cell cell = updated_cell;
            int[] candidates = sudoku.PMs;

            if (cell.box_set.Count == 1)  //if c entails box phi
            {
                int phi = cell.box_list[0];
                foreach (int psi in candidates) //then for all other psi
                {
                    //psi is not phi and not yet updated 
                    if (psi != phi && !cell.negdiamond_set.Contains(psi))
                    {
                        moves.Add(new Move(cell.name, "-d", psi, CBEskip, "Cell Based Elimination",
                        "CBE, because cell c" + cell.name + " must contain digit" + phi +
                        ", so it cannot contain digit" + psi));
                    }
                    if (moves.Count == 2) break;
                }
            }
            return moves;
        }

        /*---------------------------------------2.3.2---------------------------------------*/
        List<Move> PMD(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            Cell cell = updated_cell;
            //if c contains di phi 1,2,3 times & c contains neg di phi 8,7,6 times & not yet updated
            if (cell.diamond_set.Count == n && cell.negdiamond_set.Count == (9 - n) && cell.box_set.Count != n)
            {
                List<int> PMs = new List<int>();
                for (int i = 0; i < n; i++) //then c entails phi (or phi (or phi))
                {
                    PMs.Add(cell.diamond_list[i]);
                }
                moves.Add(new Move(cell.name, "b", PMs, PMDskip, "Pencil Mark Duality",
                "PMD" + n + ", because cell c" + cell.name + " contains " + n + " PM's and the other "
                + (9 - n) + " PM's are not possible in cell c" + cell.name +
                ", so it must contain either one of these " + n + " PM's"));
            }
            return moves;
        }

        /*---------------------------------------2.3.3---------------------------------------*/
        List<Move> HBE(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            foreach (int i in updated_cell.whichgroup)
            {
                Cell[] group = sudoku.groups[i];
                foreach (Cell cell in group)
                {
                    if (cell.box_set.Count == 1) //if c entails box phi
                    {
                        int phi = cell.box_list[0];
                        foreach (Cell neighbor in group) //then for all cells in same house
                        {
                            if (cell.name != neighbor.name && !neighbor.negdiamond_set.Contains(phi))
                            { //if other cell and not yet updated
                                moves.Add(new Move(neighbor.name, "-d", phi, HBEskip, "House Based Elimination",
                                "HBE, because cell c" + cell.name + " must contain " + phi + ", so cell c"
                                + neighbor.name + " cannot contain " + phi));
                            }
                        }
                    }
                }
                if (moves.Count == 2) break;
            }
            return moves;
        }

        /*---------------------------------------2.3.4---------------------------------------*/
        List<Move> LRC(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            int[] PMs = sudoku.PMs;
            foreach (int option in sudoku.PMs)
            {
                foreach (int i in updated_cell.whichgroup)
                {
                    Cell[] group = sudoku.groups[i];
                    foreach (Cell cell in group)
                    {
                        int optionCount = 0;
                        foreach (Cell neighbor in group)
                        { //if all other cells in same house entail neg di phi
                            if (neighbor.name != cell.name && neighbor.negdiamond_set.Contains(option))
                                optionCount++;
                        }
                        if (optionCount == 8 && cell.filled == false) //then cell c entails box phi
                        {
                            List<int> options = new List<int>();
                            options.Add(option);
                            moves.Add(new Move(cell.name, "b", options, LRCskip, "Last Remaining Cell",
                            "LRC, because cell c" + cell.name + " is the last remaining cell in its house " +
                            "that could contain " + option + ", so it must hold" + option));
                        }
                    }
                }
            }
            return moves;
        }

        /*---------------------------------------2.3.5---------------------------------------*/
        List<Move> PMI(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            int[] options = sudoku.PMs; //for all possible phi
            foreach (int option in options)
            {
                List<Cell> considered_cells_list = new List<Cell>();
                HashSet<Cell> considered_cells_set = new HashSet<Cell>();
                foreach (int i in updated_cell.whichgroup)
                {
                    Cell[] group = sudoku.groups[i];
                    foreach (Cell cell in group)
                    {
                        if (!considered_cells_set.Contains(cell))
                        {
                            considered_cells_list.Add(cell);
                            considered_cells_set.Add(cell);
                        }
                        int optionCount = 0;
                        foreach (Cell neighbor in group)
                        {
                            //if all other cells in same house entail neg box phi
                            if (neighbor.name != cell.name && neighbor.negbox_set.Contains(option))
                                optionCount++;
                        }
                        //if not yet updated and c does not entail diamond or is already filled
                        if (optionCount == 8 && !cell.diamond_set.Contains(option) &&
                          !cell.negdiamond_set.Contains(option) && cell.filled == false)
                            cell.possibility_count += 1; //count this house as possible 
                    }
                }
                foreach (Cell cell in considered_cells_list)
                {
                    if (cell.possibility_count == 3) //if for all 3 houses cell c is in
                    {
                        moves.Add(new Move(cell.name, "d", option, PMIskip, "Pencil Mark Introduction",
                        "PMI, because there is no cell in either on of the 3 houses that necessarily must " +
                        "hold " + option + ", so cell c" + cell.name + " could possibly contain " + option));
                    }
                    cell.possibility_count = 0; //reset counter
                    if (moves.Count == 2) break;
                }
                if (moves.Count == 2) break;
            }
            return moves;
        }

        /*---------------------------------------2.3.6---------------------------------------*/
        List<Move> N2E(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            foreach (int i in updated_cell.whichgroup)
            {
                Cell[] group = sudoku.groups[i];
                foreach (Cell cell in group)
                {
                    if (cell.box_set.Count == 2) //if c contains phi or phi
                    {
                        List<Cell> same_neighbors = new List<Cell>();
                        List<Cell> other_neighbors = new List<Cell>();
                        foreach (Cell neighbor in group)
                        {
                            //for all other cells c in same house, keep track which do and don't contain phi or phi
                            if (cell.name != neighbor.name && cell.box_set.SetEquals(neighbor.box_set))
                                same_neighbors.Add(neighbor);
                            else if (cell.name != neighbor.name && neighbor.filled == false)
                                other_neighbors.Add(neighbor);
                        }
                        //for all other cells c in same house, if 1 of them entail phi or phi
                        if (same_neighbors.Count == 1)
                        {
                            foreach (int candidate in cell.box_list)
                            {
                                foreach (Cell neighbor in other_neighbors) //then for all other c in same house
                                {
                                    if (!neighbor.negdiamond_set.Contains(candidate)) //if not already updated
                                    {
                                        moves.Add(new Move(neighbor.name, "-d", candidate, N2Eskip,
                                          "Naked Pairs Elimination", "N2E, because cell c" + cell.name + " and cell c"
                                          + same_neighbors[0].name + " form a Naked Pair of which either one of the " +
                                          "two must hold digit " + candidate + ", so in cell c" + neighbor.name +
                                          " there cannot be PM " + candidate));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return moves;
        }

        List<Move> N3E(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            foreach (int i in updated_cell.whichgroup)
            {
                Cell[] group = sudoku.groups[i];
                foreach (Cell cell1 in group)
                {
                    int phi = 0;
                    int psi = 0;
                    int chi = 0;
                    int[] ps = new int[3];
                    if (cell1.box_set.Count == 2) //if first potential triple found
                    {
                        phi = cell1.box_list[0];
                        psi = cell1.box_list[1];
                        foreach (Cell cell2 in group)
                        {
                            chi = 0;
                            //if second potential triple found
                            if (cell1.name != cell2.name && cell2.box_set.Count < 4)
                            {
                                //check whether it has 2 or 3 phis
                                if (cell2.box_set.Count == 2)
                                {
                                    if (cell2.box_list[0] == psi && cell2.box_list[1] != phi)
                                        chi = cell2.box_list[1];
                                    else if (cell2.box_list[1] == psi && cell2.box_list[0] != phi)
                                        chi = cell2.box_list[0];
                                    foreach (Cell cell3 in group)
                                    {
                                        //if third potential triple found
                                        if (cell1.name != cell3.name && cell2.name != cell3.name
                                              && cell3.box_set.Count < 4)
                                        {
                                            if (cell3.box_set.Count == 2)
                                            {
                                                if ((cell3.box_list[0] == phi && cell3.box_list[1] == chi) ||
                                                    (cell3.box_list[1] == phi && cell3.box_list[0] == chi))
                                                {
                                                    ps = new int[3] { phi, psi, chi };//We found a triple!
                                                    moves = N3Ehelper(moves, group, ps, cell1, cell2, cell3);
                                                }
                                            }
                                            else if (cell3.box_set.Count == 3)
                                            {
                                                if (cell3.box_set.Contains(phi) && cell3.box_set.Contains(psi)
                                                      && cell3.box_set.Contains(chi))
                                                {
                                                    ps = new int[3] { phi, psi, chi };//We found a triple!
                                                    moves = N3Ehelper(moves, group, ps, cell1, cell2, cell3);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (cell2.box_set.Count == 3)
                                {
                                    if (cell2.box_set.Contains(phi) && cell2.box_set.Contains(psi))
                                    {
                                        foreach (int p in cell2.box_list)
                                            if (p != phi && p != psi)
                                                chi = p;
                                        foreach (Cell cell3 in group)
                                        {
                                            //if third potential triple found
                                            if (cell1.name != cell3.name && cell2.name != cell3.name &&
                                                  cell3.box_set.Count < 4)
                                            {
                                                if (cell3.box_set.Count == 2)
                                                {
                                                    if ((cell3.box_list[0] == phi && cell3.box_list[1] == chi) ||
                                                    (cell3.box_list[1] == phi && cell3.box_list[0] == chi))
                                                    {
                                                        ps = new int[3] { phi, psi, chi };//We found a triple!
                                                        moves = N3Ehelper(moves, group, ps, cell1, cell2, cell3);
                                                    }
                                                }
                                                else if (cell3.box_set.Count == 3)
                                                {
                                                    if (cell3.box_set.Contains(phi) && cell3.box_set.Contains(psi)
                                                          && cell3.box_set.Contains(chi))
                                                    {
                                                        ps = new int[3] { phi, psi, chi };//We found a triple!
                                                        moves = N3Ehelper(moves, group, ps, cell1, cell2, cell3);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (cell1.box_set.Count == 3)
                    {
                        phi = cell1.box_list[0];
                        psi = cell1.box_list[1];
                        chi = cell1.box_list[2];
                        foreach (Cell cell2 in group)
                        {
                            //if second potential triple found
                            if (cell1.name != cell2.name && cell2.box_set.Count < 4)
                            {
                                //check whether it has 3 phis
                                if (cell2.box_set.Count == 3)
                                {
                                    if (cell2.box_set.Contains(phi) && cell2.box_set.Contains(psi)
                                          && cell2.box_set.Contains(chi))
                                    {
                                        foreach (Cell cell3 in group)
                                        {
                                            if (cell1.name != cell3.name && cell2.name != cell3.name
                                                  && cell3.box_set.Count < 4)
                                            {
                                                if (cell3.box_set.Count == 2)
                                                {
                                                    if ((cell3.box_list[0] == phi && cell3.box_list[1] == chi) ||
                                                    (cell3.box_list[1] == phi && cell3.box_list[0] == chi))
                                                    {
                                                        ps = new int[3] { phi, psi, chi };//We found a triple!
                                                        moves = N3Ehelper(moves, group, ps, cell1, cell2, cell3);
                                                    }
                                                }
                                                else if (cell3.box_set.Count == 3)
                                                {
                                                    if (cell3.box_set.Contains(phi) && cell3.box_set.Contains(psi)
                                                          && cell3.box_set.Contains(chi))
                                                    {
                                                        ps = new int[3] { phi, psi, chi };//We found a triple!
                                                        moves = N3Ehelper(moves, group, ps, cell1, cell2, cell3);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return moves;
        }

        List<Move> N3Ehelper(List<Move> moves, Cell[] group, int[] ps, Cell cell1, Cell cell2, Cell cell3)
        {
            foreach (Cell cell4 in group)
            {
                if (cell4.name != cell1.name && cell4.name != cell2.name && cell4.name != cell3.name)
                {
                    foreach (int p in ps)
                    {
                        if (!cell4.negdiamond_set.Contains(p) && !cell4.filled)
                        {
                            moves.Add(new Move(cell4.name, "-d", p, N3Eskip, "Naked Triples Elimination",
                                "N3E, because cells c" + cell1.name + ", c" + cell2.name + " and c" +
                                cell3.name + " form a Naked Triple of which either one of the Three must " +
                                "hold digit " + p + ", so in cell c" + cell4.name + " there cannot be PM " + p));
                        }
                    }
                }
            }
            return moves;
        }

        /*---------------------------------------2.3.7---------------------------------------*/
        List<Move> PTE(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            foreach (int phi in sudoku.PMs)//for all possible phi
            {
                Cell[] block = sudoku.blocks[updated_cell.whichblock];
                List<Cell> diamonds = new List<Cell>(); //keep track of which cells contain diamond phi
                List<Cell> negdiamonds = new List<Cell>();
                foreach (Cell cell in block)
                {
                    if (cell.diamond_set.Contains(phi))
                        diamonds.Add(cell);
                    if (cell.negdiamond_set.Contains(phi))
                        negdiamonds.Add(cell);
                }
                //if 2/3 cells in the same house contain dia phi and the other 7/6 contain neg di phi
                if (diamonds.Count == n && negdiamonds.Count == (9 - n))
                {
                    //keep track if these cells are in the same horizontal or vertical
                    bool samehorizontal = true;
                    bool samevertical = true;
                    int horizontal = diamonds[0].whichhorizontal;
                    int vertical = diamonds[0].whichvertical;
                    int blocki = diamonds[0].whichblock;
                    foreach (Cell cell in diamonds)
                    {
                        if (cell.whichhorizontal != horizontal)
                            samehorizontal = false;
                        if (cell.whichvertical != vertical)
                            samevertical = false;
                    }
                    //if these cells are in the same horizontal
                    if (samehorizontal)
                    {
                        foreach (Cell cell in sudoku.horizontals[diamonds[0].whichhorizontal])
                        { //then for all other c in this horizontal
                            if (!cell.negdiamond_set.Contains(phi) && cell.whichblock !=
                            blocki && cell.filled == false) //if not yet updated, not og cells & not done
                            {
                                moves.Add(new Move(cell.name, "-d", phi, PTEskip, "Pointing Tuples Elimination",
                                    "P" + n + "E, because there is a Pointing Tuple of cells containing PM "
                                    + phi + ", which prevents that cell c" + cell.name + " can contain PM " + phi));
                            }
                        }
                    }
                    //if these cells are in the same vertical
                    if (samevertical)
                    { //then for all other c in this vertical
                        foreach (Cell cell in sudoku.verticals[diamonds[0].whichvertical])
                            if (!cell.negdiamond_set.Contains(phi) && cell.whichblock != blocki && cell.filled == false)
                            { //if not yet updated and not original cells and not done
                                moves.Add(new Move(cell.name, "-d", phi, PTEskip, "Pointing Tuples Elimination",
                                    "P" + n + "E, because there is a Pointing Tuple of cells containing PM " + phi +
                                    ", which prevents that cell c" + cell.name + " can contain PM " + phi)); ;
                                //cell.NegdiamondAdd(phi); //c entails neg diamond
                            }
                    }
                }
                if (moves.Count >= 2) break;
            }
            return moves;
        }

        /*---------------------------------------2.3.8---------------------------------------*/
        List<Move> H2E(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            int[] candidates = sudoku.PMs;
            List<Tuple<Cell, Cell, int>> pair_cells = new List<Tuple<Cell, Cell, int>>();
            foreach (int phi in candidates)
            {
                foreach (int i in updated_cell.whichgroup)
                {
                    Cell[] group = sudoku.groups[i];

                    //keep track of which cell entails diamond phi
                    List<int> phis = new List<int>();
                    List<int> negphis = new List<int>();
                    List<Cell> phicells = new List<Cell>();

                    foreach (Cell cell in group)
                    {
                        if (cell.diamond_set.Contains(phi))
                        {
                            phis.Add(phi);
                            phicells.Add(cell);
                        }
                        else if (cell.negdiamond_set.Contains(phi))
                        {
                            negphis.Add(phi);
                        }
                    }

                    //if there is a pair of cells that entails a certain diamond phi
                    if (phis.Count == 2 && negphis.Count == 7)
                    {
                        pair_cells.Add(new Tuple<Cell, Cell, int>(phicells[0], phicells[1], phi));
                    }
                }
            }

            foreach (Tuple<Cell, Cell, int> cellpair in pair_cells)
            {
                foreach (Tuple<Cell, Cell, int> neighbor in pair_cells)
                {
                    //If we find a pair of cells which both contain di phi and di psi
                    if (cellpair.Item1 == neighbor.Item1 && cellpair.Item2 == neighbor.Item2
                          && cellpair.Item3 != neighbor.Item3)
                    {
                        //we found a hidden pair! //for both of these cells:
                        foreach (Cell cell in new Cell[2] { cellpair.Item1, cellpair.Item2 })
                        {
                            foreach (int chi in cell.diamond_set)
                            {
                                //if ther was a chi possible in this cell, it is not possible anymore
                                if (chi != cellpair.Item3 && chi != neighbor.Item3 && !cell.negdiamond_set.Contains(chi))
                                {
                                    moves.Add(new Move(cell.name, "-d", chi, H2Eskip, "Hidden Pairs Elimination",
                                    "H2E, because cells c" + cellpair.Item1.name + " and c" + cellpair.Item2.name
                                    + " form a Hidden Pair" + ", as they are the last in their group that can " +
                                    "contain either PM " + cellpair.Item3 + " or " + neighbor.Item3 +
                                    ", so cell c" + cell.name + " cannot contain PM " + chi));
                                }
                            }
                        }
                    }
                }
            }
            return moves;
        }

        /*---------------------------------------2.3.9---------------------------------------*/
        List<Move> XWE(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            moves = XWEhelper(moves, sudoku.horizontals, sudoku.verticals, 1);
            moves = XWEhelper(moves, sudoku.verticals, sudoku.horizontals, 0);
            return moves;
        }

        List<Move> XWEhelper(List<Move> moves, Cell[][] lines1, Cell[][] lines2, int hv)
        {
            int[] candidates = sudoku.PMs;
            foreach (int phi in candidates)
            {
                List<Tuple<Cell, Cell, int>> pair_cells = new List<Tuple<Cell, Cell, int>>();
                foreach (Cell[] line1 in lines1)
                {
                    List<int> phis = new List<int>();
                    List<int> negphis = new List<int>();
                    List<Cell> phicells = new List<Cell>();
                    foreach (Cell cell in line1)
                    {
                        if (cell.diamond_set.Contains(phi))
                        {
                            phis.Add(phi);
                            phicells.Add(cell);
                        }
                        else if (cell.negdiamond_set.Contains(phi))
                        {
                            negphis.Add(phi);
                        }
                    }
                    if (phis.Count == 2 && negphis.Count == 7)
                    {
                        pair_cells.Add(new Tuple<Cell, Cell, int>(phicells[0], phicells[1], phi));
                    }
                }
                foreach (Tuple<Cell, Cell, int> cellpair in pair_cells)
                {
                    foreach (Tuple<Cell, Cell, int> neighbor in pair_cells)
                    {
                        if (cellpair != neighbor && cellpair.Item1.whichline[hv] == neighbor.Item1.whichline[hv]
                              && cellpair.Item2.whichline[hv] == neighbor.Item2.whichline[hv])
                        {
                            //we hebben een xwing!
                            Cell[][] xverticals = CombineArray(new Cell[1][]
                            {lines2[cellpair.Item1.whichline[hv]]}, new Cell[1][] { lines2[cellpair.Item2.whichline[hv]] });
                            foreach (Cell[] xvertical in xverticals)
                                foreach (Cell cell in xvertical)
                                    if (cell != cellpair.Item1 && cell != cellpair.Item2 && cell != neighbor.Item1
                                          && cell != neighbor.Item2 && !cell.negdiamond_set.Contains(phi))
                                        moves.Add(new Move(cell.name, "-d", phi, XWEskip, "X-Wing Elimination",
                                            "XWE, because the cells c" + cellpair.Item1.name + ", c" +
                                            cellpair.Item2.name + ", c" + neighbor.Item1.name + " and c" +
                                            +neighbor.Item2.name + " form an X-Wing. They all hold " +
                                            phi + ", therefore cell c" + cell.name + " cannot hold " + phi));
                        }
                    }
                }
            }
            return moves;
        }

        /*---------------------------------------3.8---------------------------------------*/
        List<Move> H3E(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            int[] candidates = sudoku.PMs;
            List<Tuple<Cell, Cell, Cell, int>> pair_cells = new List<Tuple<Cell, Cell, Cell, int>>();
            foreach (int phi in candidates)
            {
                foreach (int i in updated_cell.whichgroup)
                {
                    Cell[] group = sudoku.groups[i];

                    //keep track of which cell entails diamond phi
                    List<int> phis = new List<int>();
                    List<int> negphis = new List<int>();
                    List<Cell> phicells = new List<Cell>();

                    foreach (Cell cell in group)
                    {
                        if (cell.diamond_set.Contains(phi))
                        {
                            phis.Add(phi);
                            phicells.Add(cell);
                        }
                        else if (cell.negdiamond_set.Contains(phi))
                        {
                            negphis.Add(phi);
                        }
                    }

                    //if there is a triple of cells that entails a certain diamond phi
                    if (phis.Count == 3 && negphis.Count == 6)
                    {
                        pair_cells.Add(new Tuple<Cell, Cell, Cell, int>(phicells[0], phicells[1], phicells[2], phi));
                    }
                }
            }

            foreach (Tuple<Cell, Cell, Cell, int> cellpair in pair_cells)
            {
                foreach (Tuple<Cell, Cell, Cell, int> neighbor in pair_cells)
                {
                    //If we find a triple of cells which contain di phi and di psi and a third digit
                    if (cellpair.Item1 == neighbor.Item1 && cellpair.Item2 == neighbor.Item2
                          && cellpair.Item3 == neighbor.Item3 && cellpair.Item4 != neighbor.Item4)
                    {
                        //we found a hidden triple! //for all of these three cells:
                        foreach (Cell cell in new Cell[3] { cellpair.Item1, cellpair.Item2, cellpair.Item3 })
                        {
                            foreach (int chi in cell.diamond_set)
                            {
                                //if ther was a chi possible in this cell, it is not possible anymore
                                if (chi != cellpair.Item4 && chi != neighbor.Item4 && !cell.negdiamond_set.Contains(chi))
                                {
                                    moves.Add(new Move(cell.name, "-d", chi, H2Eskip, "Hidden Triples Elimination",
                                    "H3E, because cells c" + cellpair.Item1.name + ", c" + cellpair.Item2.name + " and c" + cellpair.Item3.name
                                    + " form a Hidden Triple" + ", as they are the last in their group that can " +
                                    "contain either PM " + cellpair.Item4 + " or " + neighbor.Item4 +
                                    ", so cell c" + cell.name + " cannot contain PM " + chi));
                                }
                            }
                        }
                    }
                }
            }
            return moves;
        }

        /*---------------------------------------3.12---------------------------------------*/
        List<Move> SFE(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            moves = SFEhelper(moves, sudoku.horizontals, sudoku.verticals, 1);
            moves = SFEhelper(moves, sudoku.verticals, sudoku.horizontals, 0);
            return moves;
        }

        List<Move> SFEhelper(List<Move> moves, Cell[][] lines1, Cell[][] lines2, int hv)
        {
            int[] candidates = sudoku.PMs;
            foreach (int phi in candidates)
            {
                List<Tuple<Cell, Cell, Cell, int>> triple_cells = new List<Tuple<Cell, Cell, Cell, int>>();
                foreach (Cell[] line1 in lines1)
                {
                    List<int> phis = new List<int>();
                    List<int> negphis = new List<int>();
                    List<Cell> phicells = new List<Cell>();
                    foreach (Cell cell in line1)
                    {
                        if (cell.diamond_set.Contains(phi))
                        {
                            phis.Add(phi);
                            phicells.Add(cell);
                        }
                        else if (cell.negdiamond_set.Contains(phi))
                        {
                            negphis.Add(phi);
                        }
                    }
                    if (phis.Count == 3 && negphis.Count == 6)
                    {
                        triple_cells.Add(new Tuple<Cell, Cell, Cell, int>(phicells[0], phicells[1], phicells[2], phi));
                    }
                }
                if (triple_cells.Count == 3)
                {
                    foreach (Tuple<Cell, Cell, Cell, int> cellpair in triple_cells)
                    {
                        foreach (Tuple<Cell, Cell, Cell, int> neighbor in triple_cells)
                        {
                            foreach (Tuple<Cell, Cell, Cell, int> secondneighbor in triple_cells)
                            {
                                if (cellpair != neighbor && cellpair != secondneighbor && neighbor != secondneighbor
                                    && cellpair.Item1.whichline[hv] == neighbor.Item1.whichline[hv]
                                    && cellpair.Item2.whichline[hv] == neighbor.Item2.whichline[hv]
                                    && cellpair.Item3.whichline[hv] == neighbor.Item3.whichline[hv]
                                    && cellpair.Item1.whichline[hv] == secondneighbor.Item1.whichline[hv]
                                    && cellpair.Item2.whichline[hv] == secondneighbor.Item2.whichline[hv]
                                    && cellpair.Item3.whichline[hv] == secondneighbor.Item3.whichline[hv])
                                {
                                    //we hebben een Swordfish!
                                    Cell[][] xverticals = CombineArray(new Cell[1][]
                                    {lines2[cellpair.Item1.whichline[hv]]}, new Cell[1][] { lines2[cellpair.Item2.whichline[hv]] },
                                    new Cell[1][] { lines2[cellpair.Item3.whichline[hv]] });
                                    foreach (Cell[] xvertical in xverticals)
                                        foreach (Cell cell in xvertical)
                                            if (cell != cellpair.Item1 && cell != cellpair.Item2 && cell != cellpair.Item3 && cell != neighbor.Item1
                                                    && cell != neighbor.Item2 && cell != neighbor.Item3 && cell != secondneighbor.Item1
                                                    && cell != secondneighbor.Item2 && cell != secondneighbor.Item3 && !cell.negdiamond_set.Contains(phi))
                                                moves.Add(new Move(cell.name, "-d", phi, SFEskip, "Swordfish Elimination",
                                                    "SFE, because the cells c" + cellpair.Item1.name + ", c" +
                                                    cellpair.Item2.name + ", c" + cellpair.Item3.name + ", c" +
                                                    neighbor.Item1.name + ", c" + neighbor.Item2.name + ", c" +
                                                    neighbor.Item3.name + ", c" + secondneighbor.Item1.name + ", c" +
                                                    secondneighbor.Item2.name + " and c" + secondneighbor.Item3.name +
                                                    " form an Swordfish. They all hold " +
                                                    phi + ", therefore cell c" + cell.name + " cannot hold " + phi));
                                }
                            }
                        }
                    }
                }
            }
            return moves;
        }

        /*---------------------------------------3.13---------------------------------------*/
        List<Move> JFE(Cell updated_cell, int n)
        {
            List<Move> moves = new List<Move>();
            moves = JFEhelper(moves, sudoku.horizontals, sudoku.verticals, 1);
            moves = JFEhelper(moves, sudoku.verticals, sudoku.horizontals, 0);
            return moves;
        }

        List<Move> JFEhelper(List<Move> moves, Cell[][] lines1, Cell[][] lines2, int hv)
        {
            int[] candidates = sudoku.PMs;
            foreach (int phi in candidates)
            {
                List<Tuple<Cell, Cell, Cell, Cell, int>> quadruple_cells = new List<Tuple<Cell, Cell, Cell, Cell, int>>();
                foreach (Cell[] line1 in lines1)
                {
                    List<int> phis = new List<int>();
                    List<int> negphis = new List<int>();
                    List<Cell> phicells = new List<Cell>();
                    foreach (Cell cell in line1)
                    {
                        if (cell.diamond_set.Contains(phi))
                        {
                            phis.Add(phi);
                            phicells.Add(cell);
                        }
                        else if (cell.negdiamond_set.Contains(phi))
                        {
                            negphis.Add(phi);
                        }
                    }
                    if (phis.Count == 4 && negphis.Count == 5)
                    {
                        quadruple_cells.Add(new Tuple<Cell, Cell, Cell, Cell, int>(phicells[0], phicells[1], phicells[2], phicells[3], phi));
                    }
                }
                if (quadruple_cells.Count == 4)
                {
                    foreach (Tuple<Cell, Cell, Cell, Cell, int> cellpair in quadruple_cells)
                    {
                        foreach (Tuple<Cell, Cell, Cell, Cell, int> neighbor in quadruple_cells)
                        {
                            foreach (Tuple<Cell, Cell, Cell, Cell, int> secondneighbor in quadruple_cells)
                            {
                                foreach (Tuple<Cell, Cell, Cell, Cell, int> thirdneighbor in quadruple_cells)
                                {
                                    if (cellpair != neighbor && cellpair != secondneighbor && cellpair != thirdneighbor
                                        && neighbor != secondneighbor && neighbor != thirdneighbor && secondneighbor != thirdneighbor
                                        && cellpair.Item1.whichline[hv] == neighbor.Item1.whichline[hv]
                                        && cellpair.Item2.whichline[hv] == neighbor.Item2.whichline[hv]
                                        && cellpair.Item3.whichline[hv] == neighbor.Item3.whichline[hv]
                                        && cellpair.Item4.whichline[hv] == neighbor.Item4.whichline[hv]
                                        && cellpair.Item1.whichline[hv] == secondneighbor.Item1.whichline[hv]
                                        && cellpair.Item2.whichline[hv] == secondneighbor.Item2.whichline[hv]
                                        && cellpair.Item3.whichline[hv] == secondneighbor.Item3.whichline[hv]
                                        && cellpair.Item4.whichline[hv] == secondneighbor.Item4.whichline[hv]
                                        && cellpair.Item1.whichline[hv] == thirdneighbor.Item1.whichline[hv]
                                        && cellpair.Item2.whichline[hv] == thirdneighbor.Item2.whichline[hv]
                                        && cellpair.Item3.whichline[hv] == thirdneighbor.Item3.whichline[hv]
                                        && cellpair.Item4.whichline[hv] == thirdneighbor.Item4.whichline[hv])
                                    {
                                        //we hebben een Jellyfish!
                                        Cell[][] xverticals = CombineArray(new Cell[1][]
                                        {lines2[cellpair.Item1.whichline[hv]]}, new Cell[1][] { lines2[cellpair.Item2.whichline[hv]] },
                                        new Cell[1][] { lines2[cellpair.Item3.whichline[hv]] }, new Cell[1][] { lines2[cellpair.Item4.whichline[hv]] });
                                        foreach (Cell[] xvertical in xverticals)
                                            foreach (Cell cell in xvertical)
                                                if (cell != cellpair.Item1 && cell != cellpair.Item2 && cell != cellpair.Item3 && cell != neighbor.Item1
                                                        && cell != neighbor.Item2 && cell != neighbor.Item3 && cell != secondneighbor.Item1
                                                        && cell != secondneighbor.Item2 && cell != secondneighbor.Item3 && cell != thirdneighbor.Item1
                                                        && cell != thirdneighbor.Item2 && cell != thirdneighbor.Item3 && !cell.negdiamond_set.Contains(phi))
                                                    moves.Add(new Move(cell.name, "-d", phi, SFEskip, "Jellyfish Elimination",
                                                        "JFE, because the cells c" + cellpair.Item1.name + ", c" +
                                                        cellpair.Item2.name + ", c" + cellpair.Item3.name + ", c" +
                                                        cellpair.Item4.name + ", c" + neighbor.Item1.name + ", c" +
                                                        neighbor.Item2.name + ", c" + neighbor.Item3.name + ", c" +
                                                        neighbor.Item4.name + ", c" + secondneighbor.Item1.name + ", c" +
                                                        secondneighbor.Item2.name + ", c" + secondneighbor.Item3.name + ", c" +
                                                        secondneighbor.Item4.name + ", c" + thirdneighbor.Item1.name + ", c" +
                                                        thirdneighbor.Item2.name + ", c" + thirdneighbor.Item3.name + " and c" +
                                                        thirdneighbor.Item4.name + " form an Jellyfish. They all hold " +
                                                        phi + ", therefore cell c" + cell.name + " cannot hold " + phi));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return moves;
        }

        // helper functions construction
        Cell[][] CombineArray(Cell[][] a, Cell[][] b)
        {
            int newLength = a.Length + b.Length;
            Cell[][] newArray = new Cell[newLength][];
            for (int i = 0; i < a.Length; i++)
                newArray[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                newArray[i + a.Length] = b[i];
            return newArray;
        }

        Cell[][] CombineArray(Cell[][] a, Cell[][] b, Cell[][] c)
        {
            int newLength = a.Length + b.Length + c.Length;
            Cell[][] newArray = new Cell[newLength][];
            for (int i = 0; i < a.Length; i++)
                newArray[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                newArray[i + a.Length] = b[i];
            for (int i = 0; i < c.Length; i++)
                newArray[i + a.Length + b.Length] = c[i];
            return newArray;
        }

        Cell[][] CombineArray(Cell[][] a, Cell[][] b, Cell[][] c, Cell[][] d)
        {
            int newLength = a.Length + b.Length + c.Length + d.Length;
            Cell[][] newArray = new Cell[newLength][];
            for (int i = 0; i < a.Length; i++)
                newArray[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                newArray[i + a.Length] = b[i];
            for (int i = 0; i < c.Length; i++)
                newArray[i + a.Length + b.Length] = c[i];
            for (int i = 0; i < c.Length; i++)
                newArray[i + a.Length + b.Length + c.Length] = d[i];
            return newArray;
        }

        Cell[][] GroupMaker(Cell[] cells, string g)
        {
            Cell[][] groups = new Cell[9][];
            for (int i = 0; i < 9; i++)
            {
                Cell[] group = new Cell[9];
                for (int j = 0; j < 9; j++)
                {
                    if (g == "h")
                    {
                        group[j] = cells[j + (i * 9)];
                        cells[j + (i * 9)].whichhorizontal = i;
                        cells[j + (i * 9)].whichline[0] = i;
                        cells[j + (i * 9)].whichgroup[0] = i;
                    }
                    if (g == "v")
                    {
                        group[j] = cells[i + (j * 9)];
                        cells[j + (i * 9)].whichvertical = j;
                        cells[j + (i * 9)].whichline[1] = j;
                        cells[j + (i * 9)].whichgroup[1] = j + 9;
                    }
                }
                groups[i] = group;
            }
            if (g == "b")
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Cell[] group = new Cell[9];
                        for (int ii = 0; ii < 3; ii++)
                        {
                            for (int jj = 0; jj < 3; jj++)
                            {
                                group[ii + (jj * 3)] = cells[(i * 3) + (j * 3) * 9 + ii + (jj * 9)];
                                cells[(i * 3) + (j * 3) * 9 + ii + (jj * 9)].whichblock = (i * 3) + j;
                                cells[(i * 3) + (j * 3) * 9 + ii + (jj * 9)].whichgroup[2] = (i * 3) + j + 18;
                            }
                        }
                        groups[j + i * 3] = group;
                    }
                }
            }
            return groups;
        }

        Cell[] CellListMaker(string s)
        {
            Cell[] cells = new Cell[81];
            for (int i = 0; i < 81; i++)
            {
                if (s[i] == '0')
                {
                    cells[i] = new Cell(i);
                    helpcells[i] = new Cell(i);
                    Bitmap b = new Bitmap(70, 70);
                    Graphics g = Graphics.FromImage(b);
                    bs[i] = b;
                    gs[i] = g;
                }

                else
                {
                    string digit = s[i].ToString();
                    cells[i] = new Cell(i, Convert.ToInt32(digit));
                    helpcells[i] = new Cell(i, Convert.ToInt32(digit));
                    Graphics g = gs[i];
                    g.Clear(Color.Transparent);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    g.DrawString(digit, new Font("Tahoma", 20), Brushes.Black, 25, 25);
                }

                gs[i].Flush();
                pbs[i].Image = bs[i];
                pbs[i].Update();

            }
            return cells;
        }

        void SudokuWriter(Cell[] cells)
        {
            outputfield.Text = "";
            for (int i = 0; i < 81; i++)
            {
                if (cells[i].box_list.Count == 1)
                    outputfield.Text = outputfield.Text + (cells[i].box_list[0]);
                else
                    outputfield.Text = outputfield.Text + 0;
            }
        }
    }

    //Sudoku and Graph Classes

    class Cell
    {
        public int name;

        public bool filled = false;
        public bool given = false;
        public int possibility_count = 0;
        public int whichhorizontal;
        public int whichvertical;
        public int[] whichline = new int[2];
        public int whichblock;
        public int[] whichgroup = new int[3]; //in groups

        public HashSet<int> box_set = new HashSet<int>(); //and
        public HashSet<int> negbox_set = new HashSet<int>(); //or
        public HashSet<int> diamond_set = new HashSet<int>(); //and
        public HashSet<int> negdiamond_set = new HashSet<int>(); //and

        public List<int> box_list = new List<int>(); //and
        public List<int> negbox_list = new List<int>(); //or
        public List<int> diamond_list = new List<int>(); //and
        public List<int> negdiamond_list = new List<int>(); //and

        public Cell(int n)
        {
            name = n;
            int[] candidates = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int candidate in candidates)
            {
                negbox_set.Add(candidate);
                negbox_list.Add(candidate);
            }
        }

        public Cell(int n, int c)
        {
            name = n;
            filled = true;
            given = true;

            box_set.Add(c);
            box_list.Add(c);

            int[] candidates = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int candidate in candidates)
            {
                if (!box_set.Contains(candidate))
                {
                    negbox_set.Add(candidate);
                    negbox_list.Add(candidate);
                }
            }
        }

        public void NegdiamondAdd(int c)
        {
            if (diamond_set.Contains(c))
            {
                diamond_set.Remove(c);
                diamond_list.Remove(c);
            }
            negdiamond_set.Add(c);
            negdiamond_list.Add(c);
        }

        public void DiamondAdd(int c)
        {
            diamond_list.Add(c);
            diamond_set.Add(c);
        }

        public void BoxAdd(List<int> pms)
        {
            box_set.Clear();
            box_list.Clear();
            foreach (int pm in pms)
            {
                box_set.Add(pm);
                box_list.Add(pm);
            }
            if (pms.Count == 1)
            {
                filled = true;
                diamond_list.Clear();
                diamond_set.Clear();
                diamond_list.Add(pms[0]);
                diamond_set.Add(pms[0]);
                negdiamond_list.Clear();
                negdiamond_set.Clear();
            }
        }

    }

    class Sudoku
    {
        public Cell[] cells;
        public Cell[][] horizontals;
        public Cell[][] verticals;
        public Cell[][] blocks;
        public Cell[][] lines;
        public Cell[][] groups;
        public int[] PMs = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public Sudoku(Cell[] c, Cell[][] h, Cell[][] v, Cell[][] b, Cell[][] l, Cell[][] g)
        {
            cells = c;
            horizontals = h;
            verticals = v;
            blocks = b;
            lines = l;
            groups = g;
        }
    }

    class Move
    {
        public int cellname_to_update;//Cell cell_to_update;
        public string strategyname;
        public string modality;
        public int PM = 0;
        public List<int> PMs;
        public string explainmove;
        public Allowed skip;

        public Move() { }

        public Move(int c, string m, int pm, Allowed sk, string s, string e)
        {
            cellname_to_update = c;
            modality = m;
            PM = pm;
            skip = sk;
            strategyname = s;
            explainmove = e;
        }

        public Move(int c, string m, List<int> pms, Allowed sk, string s, string e)
        {
            cellname_to_update = c;
            modality = m;
            PMs = pms;
            skip = sk;
            strategyname = s;
            explainmove = e;
        }
    }

    class Node
    {
        public int name;
        public List<Edge> outgoingEdges = new List<Edge>();

        public Node(int n)
        {
            name = n;
            //outgoingEdges = e;
        }
    }

    class Edge
    {
        //public int name;
        public Node from;
        public Node to;
        public Func<Cell, int, List<Move>> strategy;
        public int strategy_n;
        public Allowed isAllowed;

        public Edge(Node f, Node t, Func<Cell, int, List<Move>> s, int n, Allowed a)
        {
            from = f;
            to = t;
            strategy = s;
            strategy_n = n;
            isAllowed = a;
        }
    }

    class Allowed
    {
        public bool b;
        public Allowed(bool bo)
        {
            b = bo;
        }

    }
}
