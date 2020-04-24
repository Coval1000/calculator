using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Media;
using System.Threading;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        enum Operation
        {
            None,
            Addition,
            Substraction,
            Multiplication,
            Division,
            Power,
            Equal,
            Bracket_Left,
            Bracket_Right,
            X,
            Factorial,
            Sinus,
            Cosinus,
            Tanges,
            Sinus_Hiperbolide,
            Cosinus_Hiperbolide,
            Tanges_Hiperbolide,
            Root,
            Log,
            Ln,
            Modulo,
            Exponenta,
            Intinger
        }

        private class Equation
        {
            enum Phrase
            {
                None, Operation, Value, Funcion, Bracket_Left, Bracket_Right, Right_Side_Modifer, X, Funcion_With_Base
            }

            private class Element
            {
                private Phrase _phrase;
                private double value;
                private Operation _operation;
                public Phrase phrase
                {
                    get { return _phrase; }
                }
                public Operation operation
                {
                    get { return _operation; }
                }

                public Element(double d)
                {
                    _phrase = Phrase.Value;
                    value = d;
                }

                public Element(Operation op, double n = 1)
                {
                    switch (op)
                    {
                        case Operation.Substraction:
                        case Operation.Addition:
                        case Operation.Multiplication:
                        case Operation.Division:
                        case Operation.Power:
                        case Operation.Modulo:
                            _phrase = Phrase.Operation;
                            break;

                        case Operation.Bracket_Left:
                            _phrase = Phrase.Bracket_Left;
                            break;

                        case Operation.Bracket_Right:
                            _phrase = Phrase.Bracket_Right;
                            break;

                        case Operation.Factorial:
                            _phrase = Phrase.Right_Side_Modifer;
                            break;

                        case Operation.X:
                            _phrase = Phrase.X;
                            break;

                        case Operation.Sinus:
                        case Operation.Sinus_Hiperbolide:
                        case Operation.Cosinus:
                        case Operation.Cosinus_Hiperbolide:
                        case Operation.Tanges:
                        case Operation.Tanges_Hiperbolide:
                        case Operation.Ln:
                        case Operation.Exponenta:
                        case Operation.Intinger:
                            _phrase = Phrase.Funcion;
                            break;

                        case Operation.Log:
                        case Operation.Root:
                            _phrase = Phrase.Funcion_With_Base;
                            value = n;
                            break;
                        case Operation.None:
                            _phrase = Phrase.None;
                            break;
                        default:
                            throw new ArgumentException("Undefined Operation: " + op.ToString(), "op in Element");
                    }
                    _operation = op;
                }

                public override string ToString()
                {
                    if (phrase == Phrase.Value)
                        return value.ToString();
                    else if (phrase == Phrase.Operation)
                        return " " + operation_symbol(_operation) + " ";
                    else
                        return operation_symbol(_operation);
                }

                public double ToValue()
                {
                    if (phrase == Phrase.Value || phrase == Phrase.Funcion_With_Base || phrase == Phrase.X)
                        return value;
                    else
                        return double.NaN;
                }

                public void Value_of_x(double x)
                {
                    if (phrase == Phrase.X)
                    {
                        value = x;
                    }
                }
            }

            private List<Element> element;

            /// <summary>
            /// Add element to equation. Returns true if successed
            /// </summary>
            /// <param name="d"></param>
            /// <returns></returns>
            public bool Add(double d)
            {
                if (element.Count > 0)
                {
                    if (element[element.Count - 1].phrase == Phrase.Value ||
                        element[element.Count - 1].phrase == Phrase.Right_Side_Modifer) return false;
                }

                element.Add(new Element(d));
                return true;
            }

            public bool Add(Operation op, double n = 1)
            {
                Element tmp = new Element(op, n);
                if (element.Count > 0)
                {
                    switch (tmp.phrase)
                    {
                        case Phrase.Bracket_Left:
                        case Phrase.Funcion:
                        case Phrase.Funcion_With_Base:
                            if (element[element.Count - 1].phrase == Phrase.Value ||
                               element[element.Count - 1].phrase == Phrase.Funcion ||
                               element[element.Count - 1].phrase == Phrase.Funcion_With_Base ||
                               element[element.Count - 1].phrase == Phrase.X ||
                               element[element.Count - 1].phrase == Phrase.Bracket_Left ||
                               element[element.Count - 1].phrase == Phrase.Bracket_Right ||
                               element[element.Count - 1].phrase == Phrase.Operation)
                            {
                                element.Add(tmp);
                                return true;
                            }
                            break;

                        case Phrase.Bracket_Right:
                            if (element[element.Count - 1].phrase == Phrase.Value ||
                               element[element.Count - 1].phrase == Phrase.X ||
                               element[element.Count - 1].phrase == Phrase.Bracket_Left ||
                               element[element.Count - 1].phrase == Phrase.Bracket_Right ||
                               element[element.Count - 1].phrase == Phrase.Right_Side_Modifer)
                            {
                                element.Add(tmp);
                                return true;
                            }
                            break;

                        case Phrase.Operation:
                            if (element[element.Count - 1].phrase == Phrase.Value ||
                               element[element.Count - 1].phrase == Phrase.X ||
                               element[element.Count - 1].phrase == Phrase.Bracket_Right ||
                               element[element.Count - 1].phrase == Phrase.Right_Side_Modifer)
                            {
                                element.Add(tmp);
                                return true;
                            }
                            break;

                        case Phrase.Right_Side_Modifer:
                            if (element[element.Count - 1].phrase == Phrase.Value ||
                               element[element.Count - 1].phrase == Phrase.X ||
                               element[element.Count - 1].phrase == Phrase.Bracket_Right)
                            {
                                element.Add(tmp);
                                return true;
                            }
                            break;

                        case Phrase.X:
                            if (element[element.Count - 1].phrase == Phrase.Value ||
                               element[element.Count - 1].phrase == Phrase.Funcion ||
                               element[element.Count - 1].phrase == Phrase.Funcion_With_Base ||
                               element[element.Count - 1].phrase == Phrase.Bracket_Left ||
                               element[element.Count - 1].phrase == Phrase.Bracket_Right ||
                               element[element.Count - 1].phrase == Phrase.Operation)
                            {
                                element.Add(tmp);
                                return true;
                            }
                            break;
                    }
                }
                else if (tmp.phrase == Phrase.X ||
                    tmp.phrase == Phrase.Bracket_Left ||
                    tmp.phrase == Phrase.Funcion ||
                    tmp.phrase == Phrase.Funcion_With_Base)
                {
                    element.Add(tmp);
                    return true;
                }
                return false;
            }

            public string equation()
            {
                string s = "";
                foreach (Element e in element)
                {
                    s += e.ToString();
                }
                return s;
            }

            public double answer(double x = 0)
            {
                List<Element> list = new List<Element>(element);
                for (int i = 0; i < element.Count - 1; i++)
                {
                    if ((list[i].phrase == Phrase.Value ||
                        list[i].phrase == Phrase.X) &&
                        (list[i + 1].phrase == Phrase.Bracket_Left ||
                        list[i + 1].phrase == Phrase.Funcion ||
                        list[i + 1].phrase == Phrase.Funcion_With_Base ||
                        list[i + 1].phrase == Phrase.Value ||
                        list[i + 1].phrase == Phrase.X))
                    {
                        list.Insert(i + 1, new Element(Operation.Multiplication));
                    }
                    else if (list[i].phrase == Phrase.Bracket_Right &&
                        (list[i + 1].phrase == Phrase.Funcion ||
                        list[i + 1].phrase == Phrase.Funcion_With_Base ||
                        list[i + 1].phrase == Phrase.Value ||
                        list[i + 1].phrase == Phrase.X))
                    {
                        list.Insert(i + 1, new Element(Operation.Multiplication));
                    }

                }

                foreach (Element e in list)
                {
                    e.Value_of_x(x);
                }

                double d = answer(list);

                return d;
            }

            private double answer(List<Element> piece)
            {
                if (piece.Count == 0) return 0;

                int left, right, i;

                for (i = -1; i < piece.Count - 1;)
                {
                    i++;
                    if (piece[i].phrase == Phrase.Bracket_Left)
                    {
                        left = i;
                        right = i;
                        for (int j = 1; j > 0 && piece.Count - 1 != right;)
                        {
                            right++;
                            if (piece[right].phrase == Phrase.Bracket_Left) j++;
                            if (piece[right].phrase == Phrase.Bracket_Right) j--;
                        }
                        if (right == piece.Count - 1 && piece[right].phrase != Phrase.Bracket_Right) return -1;

                        Element[] part = new Element[right - left - 1];
                        piece.CopyTo(left + 1, part, 0, right - left - 1);
                        piece.RemoveRange(left, right - left + 1);
                        piece.Insert(left, new Element(answer(new List<Element>(part))));
                    }
                }

                piece = calc_funcion(piece, Operation.Sinus);
                piece = calc_funcion(piece, Operation.Sinus_Hiperbolide);
                piece = calc_funcion(piece, Operation.Cosinus);
                piece = calc_funcion(piece, Operation.Cosinus_Hiperbolide);
                piece = calc_funcion(piece, Operation.Tanges);
                piece = calc_funcion(piece, Operation.Tanges_Hiperbolide);
                piece = calc_funcion(piece, Operation.Ln);
                piece = calc_funcion(piece, Operation.Log);
                piece = calc_funcion(piece, Operation.Exponenta);
                piece = calc_funcion(piece, Operation.Intinger);
                piece = calc_operator(piece, Operation.Power);
                piece = calc_funcion(piece, Operation.Root);
                piece = calc_operator(piece, Operation.Division);
                piece = calc_operator(piece, Operation.Modulo);
                piece = calc_operator(piece, Operation.Multiplication);
                piece = calc_operator(piece, Operation.Substraction);
                piece = calc_operator(piece, Operation.Addition);

                if (piece.Count > 1) throw new ArgumentException("Remains in equation:" + piece.Count, "piece in answer");
                return piece[piece.Count - 1].ToValue();
            }

            private List<Element> calc_operator(List<Element> piece, Operation op)
            {
                int i;

                for (i = -1; i < piece.Count - 1;)
                {
                    i++;
                    if (piece[i].phrase == Phrase.Operation && piece[i].operation == op)
                    {
                        double d1 = piece[i - 1].ToValue();
                        double d2 = piece[i + 1].ToValue();
                        piece.RemoveRange(i - 1, 3);

                        switch (op)
                        {
                            case Operation.Addition:
                                piece.Insert(i - 1, new Element(d1 + d2));
                                break;

                            case Operation.Substraction:
                                piece.Insert(i - 1, new Element(d1 - d2));
                                break;

                            case Operation.Multiplication:
                                piece.Insert(i - 1, new Element(d1 * d2));
                                break;

                            case Operation.Division:
                                piece.Insert(i - 1, new Element(d1 / d2));
                                break;

                            case Operation.Modulo:
                                piece.Insert(i - 1, new Element(d1 % d2));
                                break;

                            case Operation.Power:
                                piece.Insert(i - 1, new Element(Math.Pow(d1, d2)));
                                break;

                            default:
                                throw new ArgumentException("Unexpected operator: " + op.ToString(), "type in calc_operator");
                        }
                        i--;
                    }
                }
                return piece;
            }

            private List<Element> calc_funcion(List<Element> piece, Operation funcion)
            {
                int i;

                for (i = -1; i < piece.Count - 1;)
                {
                    i++;
                    if ((piece[i].phrase == Phrase.Funcion || piece[i].phrase == Phrase.Funcion_With_Base) && piece[i].operation == funcion)
                    {
                        if (piece[i + 1].phrase == Phrase.Funcion)
                        {
                            Element[] part = new Element[piece.Count - 1 - i];
                            piece.CopyTo(i + 1, part, 0, piece.Count - 1 - i);
                            piece.RemoveRange(i, piece.Count - 1 - i);
                            piece.Insert(i, new Element(answer(new List<Element>(part))));
                        }

                        double bas = 0;
                        if (piece[i].phrase == Phrase.Funcion_With_Base) bas = piece[i].ToValue();
                        double d1 = piece[i + 1].ToValue();
                        piece.RemoveRange(i, 2);

                        switch (funcion)
                        {
                            case Operation.Sinus:
                                piece.Insert(i, new Element(Math.Sin(d1)));
                                break;

                            case Operation.Sinus_Hiperbolide:
                                piece.Insert(i, new Element(Math.Sinh(d1)));
                                break;

                            case Operation.Cosinus:
                                piece.Insert(i, new Element(Math.Cos(d1)));
                                break;

                            case Operation.Cosinus_Hiperbolide:
                                piece.Insert(i, new Element(Math.Cosh(d1)));
                                break;

                            case Operation.Tanges:
                                piece.Insert(i, new Element(Math.Tan(d1)));
                                break;

                            case Operation.Tanges_Hiperbolide:
                                piece.Insert(i, new Element(Math.Tanh(d1)));
                                break;

                            case Operation.Ln:
                                piece.Insert(i, new Element(Math.Log(d1)));
                                break;

                            case Operation.Exponenta:
                                piece.Insert(i, new Element(Math.Exp(d1)));
                                break;

                            case Operation.Intinger:
                                piece.Insert(i, new Element(Math.Round(d1)));
                                break;

                            case Operation.Root:
                                piece.Insert(i, new Element(Math.Pow(d1, 1f / bas)));
                                break;

                            case Operation.Log:
                                piece.Insert(i, new Element(Math.Log(d1, bas)));
                                break;
                            default:
                                throw new ArgumentException("Unexpected funcion: " + funcion, "funcion in calc_funcion");
                        }
                    }
                }
                return piece;
            }

            public void Clear()
            {
                element.RemoveRange(0, element.Count);
            }

            public Equation()
            {
                element = new List<Element>();
            }

            ~Equation()
            {
            }

        }

        Operation operation;

        private Bitmap graph;
        private Bitmap display;

        private double _value;
        private double tmp;
        private double _memory;

        private float pitch;
        private float amplitude;
        private float showX;
        private float showY;

        private Color display_bg;
        private Color display_font_color;
        private Color _memory_buttons;
        private Color AxisX;
        private Color AxisY;
        private Color Graph;

        private Font display_font;
        private Font display_font_lesser;
        private Font graph_font;

        private bool comma;
        private bool drawComma;
        private bool next;
        private bool first;
        private bool theX;
        private bool Zero;
        private bool _isMemory;
        private bool _Focus;
        private bool _FocusOX;
        private bool _FocusOY;
        private bool _FocusP;
        private bool _FocusA;
        private bool _FocusSX;
        private bool _FocusSY;
        private bool _Inv;

        private int Zeros;
        private int lastDigit;
        private int _offSetX;
        private int _offSetY;
        private int _brascet_balance;

        private Point grab_point;

        private Equation equationC;
        //
        //Accessors
        //
        public bool Inv
        {
            get { return _Inv; }
            set
            {

                if (_Inv == value) return;
                _Inv = value;
                if (_Inv)
                {
                    b_Ln.Text = "Exp";
                }
                else
                {
                    b_Ln.Text = "ln";
                }
            }
        }

        public int offSetX
        {
            get { return _offSetX; }
            set
            {
                _offSetX = value;
                tB_OffSetX.Text = value.ToString();
            }
        }

        public int offSetY
        {
            get { return _offSetY; }
            set
            {
                _offSetY = value;
                tB_OffSetY.Text = value.ToString();
            }
        }

        private int brascet_balance
        {
            get { return _brascet_balance; }
            set
            {
                if (value > -1)
                {
                    _brascet_balance = value;
                    if (_brascet_balance > 0)
                        b_dud.Text = "(=" + _brascet_balance;
                    else b_dud.Text = "";
                }
            }
        }

        public bool isMemory
        {
            get { return _isMemory; }
            set
            {
                if (!value) memory = 0;
                _isMemory = value;
            }
        }

        public double memory
        {
            get { return _memory; }
            set
            {
                _memory = value;
                isMemory = true;
            }
        }

        public double value
        {
            get { return _value; }
            set
            {
                _value = value;
                drawComma = false;
                Zero = false;
                print();

            }
        }

        public Color memory_buttons
        {
            get { return _memory_buttons; }
            set
            {
                if (value != _memory_buttons)
                {
                    b_MS.BackColor = value;
                    b_MR.BackColor = value;
                    b_Mn.BackColor = value;
                    b_Mp.BackColor = value;
                    b_MC.BackColor = value;
                    _memory_buttons = value;
                }
            }
        }

        private new bool Focus
        {
            get { return _Focus; }
            set
            {
                if (FocusOX || FocusOY || FocusA || FocusP || FocusSX || FocusSY)
                    _Focus = true;
                else
                    _Focus = false;
            }
        }
        private bool FocusOX
        {
            get { return _FocusOX; }
            set
            {
                _FocusOX = value;
                Focus = value;
            }
        }
        private bool FocusOY
        {
            get { return _FocusOY; }
            set
            {
                _FocusOY = value;
                Focus = value;
            }
        }
        private bool FocusP
        {
            get { return _FocusP; }
            set
            {
                _FocusP = value;
                Focus = value;
            }
        }
        private bool FocusA
        {
            get { return _FocusA; }
            set
            {
                _FocusA = value;
                Focus = value;
            }
        }
        private bool FocusSX
        {
            get { return _FocusSX; }
            set
            {
                _FocusSX = value;
                Focus = value;
            }
        }
        private bool FocusSY
        {
            get { return _FocusSY; }
            set
            {
                _FocusSY = value;
                Focus = value;
            }
        }
        //
        //Constructor
        //
        public Form1()

        {
            InitializeComponent();
            p_Advanced_Pad.Visible = false;
            p_Graph.Visible = false;
            this.Width = p_Num_Pad.Width + 36;
            p_Main.Width = p_Num_Pad.Width + 18;
            //menuStrip1.Visible = false;
            display_font_color = Color.FromArgb(255, 6, 38, 62);
            display_bg = Color.FromName("GradientInactiveCaption");
            display_font = new Font("Calibri", pB_Display.Height * 0.5f, FontStyle.Bold, GraphicsUnit.Pixel);
            display_font_lesser = new Font("Calibri", pB_Display.Height * 0.2f, FontStyle.Bold, GraphicsUnit.Pixel);
            display = new Bitmap(1, 1);
            lastDigit = 0;
            Zeros = 0;
            next = true;
            first = true;
            isMemory = false;
            operation = Operation.None;
            equationC = new Equation();
            _value = 0;
            memory_buttons = Color.FromName("AppWorkspace");
            brascet_balance = 0;
            print();
            //
            //Graph
            //
            graph_font = new Font("Calibri", 10, FontStyle.Regular, GraphicsUnit.Pixel);
            graph = new Bitmap(1, 1);
            theX = false;
            offSetX = 0;
            offSetY = 0;
            showX = 0;
            showY = 0;
            AxisX = Color.DimGray;
            AxisY = Color.DimGray;
            Graph = Color.Black;
            pitch = 1;
            amplitude = 1;
            //
            //Timer
            //
        }
   
        private void calc()
        {
            if (!first && !next)
            {
                if (naukowyToolStripMenuItem.Checked)
                {
                    if (operation == Operation.Equal)
                    {
                        if (wykresToolStripMenuItem.Checked)
                        {
                            drawGraph();
                        }
                        else
                        {

                            value = equationC.answer();//value = calc_equation(equation) ?? 0;
                        }
                    }
                }
                else if (standardowyToolStripMenuItem.Checked)
                {
                    switch (operation)
                    {
                        case Operation.Addition:
                            value = tmp + value;
                            break;
                        case Operation.Substraction:
                            value = tmp - value;
                            break;
                        case Operation.Multiplication:
                            value = tmp * value;
                            break;
                        case Operation.Division:
                            value = tmp / value;
                            break;
                    }
                }
            }
            first = false;
            next = true;
            tmp = value;
            operation = Operation.None;
        }

        private void print()
        {
            Graphics g;

            StringFormat sf_main_text = new StringFormat();
            sf_main_text.Alignment = StringAlignment.Far;
            sf_main_text.LineAlignment = StringAlignment.Center;

            StringFormat sf_operation = new StringFormat();
            sf_operation.Alignment = StringAlignment.Near;
            sf_operation.LineAlignment = StringAlignment.Center;

            StringFormat sf_memory = new StringFormat();
            sf_memory.Alignment = StringAlignment.Near;
            sf_memory.LineAlignment = StringAlignment.Far;

            StringFormat sf4 = new StringFormat();
            sf4.Alignment = StringAlignment.Far;
            sf4.LineAlignment = StringAlignment.Near;

            SolidBrush SB_Display_Font = new SolidBrush(display_font_color);
            SolidBrush SB_Display_Bg = new SolidBrush(display_bg);

            display.Dispose();

            display = new Bitmap(pB_Display.Width, pB_Display.Height);
            g = Graphics.FromImage(display);

            g.FillRectangle(SB_Display_Bg, 0, 0, display.Width, display.Height);

            string txt;

            if (grupowanieLiczbToolStripMenuItem.Checked && value < 179000000000000)
            {
                txt = _value.ToString("### ### ### ### ### ### ##0.###############");
            }
            else
            {
                txt = _value.ToString();
            }

            if (drawComma) txt += ",";
            if (!Zero) Zeros = 0;
            else for (int i = 0; i < Zeros; i++) txt += "0";
            if (theX)
            {
                if (next) txt = "x";
                else txt += "x";
            }

            g.DrawString(txt,
                display_font,
                SB_Display_Font,
                new Rectangle(0, 0, display.Width, display.Height), sf_main_text);

            /*
            g.DrawString(txt,
                    display_font,
                    SB_Display_Font,
                    new Rectangle(0, 0, display.Width, display.Height), sf_operation);
            *///Wygląda źle

            if (isMemory)
            {
                g.DrawString("M = " + memory.ToString(),
                    display_font_lesser,
                    SB_Display_Font,
                    new Rectangle(0, 0, display.Width, display.Height), sf_memory);
            }

            g.DrawString((equationC?.equation() ?? " ") + operation_symbol(operation),
                display_font_lesser,
                SB_Display_Font,
                new Rectangle(0, 0, display.Width - 5, display.Height), sf4);

            pB_Display.Image = display;
            g.Dispose();
            sf_main_text.Dispose();
            sf_memory.Dispose();
            sf_operation.Dispose();
            sf4.Dispose();
            SB_Display_Font.Dispose();
            SB_Display_Bg.Dispose();
        }

        private void newNumber()
        {
            lastDigit = 0;
            comma = false;
            drawComma = false;
            Zero = false;
            next = false;
            theX = false;
            value = 0;
            Zeros = 0;
        }

        static private string operation_symbol(Operation symbol)
        {
            string txt;
            switch (symbol)
            {
                case Operation.Addition:
                    txt = "+";
                    break;
                case Operation.Substraction:
                    txt = "-";
                    break;
                case Operation.Multiplication:
                    txt = "*";
                    break;
                case Operation.Division:
                    txt = "÷";
                    break;
                case Operation.Power:
                    txt = "^";
                    break;
                case Operation.None:
                    txt = " ";
                    break;
                case Operation.Equal:
                    txt = "=";
                    break;
                case Operation.X:
                    txt = "x";
                    break;
                case Operation.Bracket_Left:
                    txt = "(";
                    break;
                case Operation.Bracket_Right:
                    txt = ")";
                    break;
                case Operation.Cosinus:
                    txt = "cos";
                    break;
                case Operation.Cosinus_Hiperbolide:
                    txt = "cosh";
                    break;
                case Operation.Sinus:
                    txt = "sin";
                    break;
                case Operation.Sinus_Hiperbolide:
                    txt = "sinh";
                    break;
                case Operation.Tanges:
                    txt = "tan";
                    break;
                case Operation.Tanges_Hiperbolide:
                    txt = "tanh";
                    break;
                case Operation.Log:
                    txt = "log";
                    break;
                case Operation.Exponenta:
                    txt = "Exp";
                    break;
                case Operation.Intinger:
                    txt = "Int";
                    break;
                case Operation.Ln:
                    txt = "ln";
                    break;
                case Operation.Root:
                    txt = "√";
                    break;
                case Operation.Factorial:
                    txt = "!";
                    break;
                case Operation.Modulo:
                    txt = "%";
                    break;
                default:
                    txt = "";
                    break;
            }
            return txt;
        }

        private void put_operator(Operation symbol, double x = 0)
        {
            if (!next) equationC.Add(value);
            if (theX) equationC.Add(Operation.X);
            equationC.Add(symbol, x);
            theX = false;
            calc();
            print();
        }

        private void put_digit(double d)
        {
            if (next) newNumber();

            if (!comma)
            {
                value *= 10;
                value += value < 0 ? -d : d;
            }
            else if (d == 0)
            {
                --lastDigit;
                Zeros++;
                Zero = true;
                if (value - (long)value == 0) drawComma = true;
                print();
            }
            else value += value < 0 ? -d * Math.Pow(10, --lastDigit) : d * Math.Pow(10, --lastDigit);
        }

        private double facotrial(double d)
        {
            if (d == 0) return 1;
            if (d <= 1 && d >= -1) return d;
            return d * facotrial(d - 1);
        }

        #region Numbers

        private void b_0_Click(object sender, EventArgs e)
        {
            put_digit(0);
        }

        private void b_1_Click(object sender, EventArgs e)
        {
            put_digit(1);
        }

        private void b_2_Click(object sender, EventArgs e)
        {
            put_digit(2);
        }

        private void b_3_Click(object sender, EventArgs e)
        {
            put_digit(3);
        }

        private void b_4_Click(object sender, EventArgs e)
        {
            put_digit(4);
        }

        private void b_5_Click(object sender, EventArgs e)
        {
            put_digit(5);
        }

        private void b_6_Click(object sender, EventArgs e)
        {
            put_digit(6);
        }

        private void b_7_Click(object sender, EventArgs e)
        {
            put_digit(7);
        }

        private void b_8_Click(object sender, EventArgs e)
        {
            put_digit(8);
        }

        private void b_9_Click(object sender, EventArgs e)
        {
            put_digit(9);
        }

        private void b_comma_Click(object sender, EventArgs e)
        {
            if (!comma)
            {
                comma = true;
                drawComma = true;
                print();
            }
        }

        #endregion

        private void b_pn_Click(object sender, EventArgs e)
        {
            _value = -_value;
            print();
        }

        private void b_C_Click(object sender, EventArgs e)
        {
            newNumber();
            next = true;
            tmp = 0;
            brascet_balance = 0;
            first = true;
            operation = Operation.None;
            equationC.Clear();
            print();
        }

        private void b_CE_Click(object sender, EventArgs e)
        {
            newNumber();
            next = true;
        }

        private void b_erase_Click(object sender, EventArgs e)
        {
            if (drawComma)
            {
                drawComma = false;
                comma = false;
                print();
            }
            else if (lastDigit == 0) value = Math.Truncate(value / 10);
            else
            {
                lastDigit++;
                value = (int)(value * Math.Pow(10, -lastDigit)) * Math.Pow(10, lastDigit);
                if (lastDigit == 0) drawComma = true;
                print();
            }
        }

        #region Operation
        private void b_add_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Addition);
        }

        private void b_substract_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Substraction);
        }

        private void b_multiplie_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Multiplication);
        }

        private void b_divide_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Division);
        }

        private void b_pow_y_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Power);
        }

        private void b_square_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Power);
            equationC.Add(2);
            print();
        }

        private void b_cube_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Power);
            equationC.Add(3);
            print();
        }

        private void b_modulo_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Modulo);
        }

        private void b_ten_pow_x_Click(object sender, EventArgs e)
        {
            if (equationC.Add(10))
            {
                equationC.Add(Operation.Power);
            }
            print();
        }

        private void b_exp_Click(object sender, EventArgs e)
        {
            equationC.Add(Math.E);
            print();
        }
        //
        //
        //
        private void b_inverse_Click(object sender, EventArgs e)
        {
            value = 1 / value;
        }

        private void b_Ln_Click(object sender, EventArgs e)
        {
            if (Inv)
            {
                put_operator(Operation.Exponenta);
            }
            else put_operator(Operation.Ln);
        }

        private void b_sqrt_Click(object sender, EventArgs e)
        {
            if (naukowyToolStripMenuItem.Checked)
            {
                put_operator(Operation.Root, 2);
            }
            else
                value = Math.Sqrt(value);
        }

        private void b_procent_Click(object sender, EventArgs e)
        {
            value = tmp * (value / 100);
        }

        private void b_equal_Click(object sender, EventArgs e)
        {
            if (brascet_balance == 0)
            {
                if (!next) equationC.Add(value);
                if (theX) equationC.Add(Operation.X);
                next = false;
                first = false;
                if (naukowyToolStripMenuItem.Checked)
                {

                    operation = Operation.Equal;
                    calc();
                    value = equationC.answer();
                }
                else
                {
                    calc();
                    operation = Operation.Equal;
                }
                print();
                operation = Operation.None;
                tmp = 0;
                brascet_balance = 0;
                first = true;
            }
            else
            {
                SystemSounds.Beep.Play();//dodać timer z migotaniem

            }
        }
        #endregion

        #region Memory
        private void b_MC_Click(object sender, EventArgs e)
        {
            if (!isMemory && value == 16.09) menuStrip1.Visible = true;
            isMemory = false;
            print();

        }

        private void b_MR_Click(object sender, EventArgs e)
        {
            value = memory;
            string s = value.ToString();
            if (s.IndexOf(",") > 0)
            {
                s = s.Remove(0, s.IndexOf(",") + 1);
                lastDigit = -s.Length;
                comma = true;
                drawComma = false;
                Zeros = 0;
            }
            else
            {
                lastDigit = 0;
                comma = false;
                drawComma = false;
                Zeros = 0;
            }
        }

        private void b_MS_Click(object sender, EventArgs e)
        {
            memory = value;
            print();
        }

        private void b_Mp_Click(object sender, EventArgs e)
        {
            memory += value;
            print();
        }

        private void b_Mn_Click(object sender, EventArgs e)
        {
            memory -= value;
            print();
        }

        #endregion

        #region trigonometry

        private void b_pie_Click(object sender, EventArgs e)
        {
            if (next) newNumber();
            value = Math.PI;
        }

        private void b_sin_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Sinus);
        }

        private void b_sinh_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Sinus_Hiperbolide);
        }

        private void b_cosh_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Cosinus_Hiperbolide);
        }

        private void b_cos_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Cosinus);
        }

        private void b_tanh_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Tanges_Hiperbolide);
        }

        private void b_tan_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Tanges);
        }

        #endregion

        #region Menu Strip
        private void standardowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (standardowyToolStripMenuItem.Checked) return;
            naukowyToolStripMenuItem.CheckState = CheckState.Unchecked;
            programistyToolStripMenuItem.CheckState = CheckState.Unchecked;
            standardowyToolStripMenuItem.CheckState = CheckState.Indeterminate;
            p_Advanced_Pad.Visible = false;
            b_procent.Enabled = true; ;
            this.Width -= p_Advanced_Pad.Width + 6;
            p_Main.Width -= p_Advanced_Pad.Width + 6;
            print();


        }

        private void naukowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (naukowyToolStripMenuItem.Checked) return;
            naukowyToolStripMenuItem.CheckState = CheckState.Indeterminate;
            programistyToolStripMenuItem.CheckState = CheckState.Unchecked;
            standardowyToolStripMenuItem.CheckState = CheckState.Unchecked;
            this.Width += p_Advanced_Pad.Width + 6;
            p_Main.Width += p_Advanced_Pad.Width + 6;
            p_Advanced_Pad.Visible = true;
            b_procent.Enabled = false;
            print();
        }

        private void programistyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (programistyToolStripMenuItem.Checked) return;
            naukowyToolStripMenuItem.CheckState = CheckState.Unchecked;
            programistyToolStripMenuItem.CheckState = CheckState.Indeterminate;
            standardowyToolStripMenuItem.CheckState = CheckState.Unchecked;
        }

        private void wykresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wykresToolStripMenuItem.Checked)
            {
                wykresToolStripMenuItem.Checked = false;
                p_Graph.Visible = false;
                this.Width -= p_Graph.Width;
            }
            else
            {
                wykresToolStripMenuItem.Checked = true;
                this.Width += p_Graph.Width;
                p_Graph.Visible = true;
                tB_Amplitude.Text = amplitude.ToString();
                tB_Pitch.Text = pitch.ToString();
                tB_OffSetX.Text = offSetX.ToString();
                tB_OffSetY.Text = offSetY.ToString();
            }
            drawGraph();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Focus) return;
            switch (e.KeyChar)
            {
                case '0':
                    b_0.PerformClick();
                    break;
                case '1':
                    b_1.PerformClick();
                    break;
                case '2':
                    b_2.PerformClick();
                    break;
                case '3':
                    b_3.PerformClick();
                    break;
                case '4':
                    b_4.PerformClick();
                    break;
                case '5':
                    b_5.PerformClick();
                    break;
                case '6':
                    b_6.PerformClick();
                    break;
                case '7':
                    b_7.PerformClick();
                    break;
                case '8':
                    b_8.PerformClick();
                    break;
                case '9':
                    b_9.PerformClick();
                    break;
                case '/':
                    b_divide.PerformClick();
                    break;
                case '*':
                    b_multiply.PerformClick();
                    break;
                case '+':
                    b_add.PerformClick();
                    break;
                case '-':
                    b_substract.PerformClick();
                    break;
                case '=':
                    b_equal.PerformClick();
                    break;
                case (char)8:
                    b_erase.PerformClick();
                    break;
                case ',':
                case '.':
                    b_comma.PerformClick();
                    break;
                case '%':
                    b_procent.PerformClick();
                    break;
            }
        }
        #endregion

        #region Graph

        private void drawGraph()
        {
            graph.Dispose();
            graph = new Bitmap(pB_graph.Width, pB_graph.Height);
            Graphics g;
            g = Graphics.FromImage(graph);
            Pen Pen_AxisX = new Pen(AxisX);
            Pen Pen_AxisY = new Pen(AxisY);
            Pen Pen_Graph = new Pen(Graph);
            Pen Pen_Grid = new Pen(Color.FromName("ControlLight"));
            Pen Pen_Show = new Pen(Color.Gray);

            SolidBrush Brush_Values = new SolidBrush(Graph);
            SolidBrush Brush_White = new SolidBrush(Color.White);

            float gridX = graph.Width / 21;
            float gridY = graph.Height / 21;
            float equationI = (float)equationC.answer(showX);

            float folost = -(float)equationC.answer(showX) * amplitude + offSetY + graph.Height / 2f;
            if (folost > int.MaxValue) folost = int.MaxValue / 2f;
            if (folost < int.MinValue) folost = int.MinValue / 2f;

            float equationI1;
            //
            //Grid Vertival
            //
            g.FillRectangle(Brush_White, 0, 0, graph.Width, graph.Height);

            for (int i = -graph.Width / 2; i < graph.Width / 2; i++)
            {
                if ((i + offSetX) % 20 == 0)
                {
                    g.DrawLine(Pen_Grid, i + graph.Width / 2f, 0, i + graph.Width / 2f, graph.Height);
                }
            }
            //
            //Grid Horizontal & Labels
            //
            for (int i = graph.Height / 2; i > -graph.Width / 2; i--)
            {
                if ((i + offSetY) % 20 == 0)
                {
                    g.DrawLine(Pen_Grid, 0, -i + graph.Height / 2f, graph.Width, -i + graph.Height / 2f);

                    g.DrawLine(Pen_AxisY,
                        -offSetX + graph.Width / 2f - 7,
                        -i + graph.Height / 2f,
                        -offSetX + graph.Width / 2f + 7,
                        -i + graph.Height / 2f);

                    g.DrawString(((i + offSetY) / amplitude).ToString(),
                        graph_font,
                        Brush_Values,
                        -offSetX + graph.Width / 2f + 12,
                        -i + graph.Height / 2f - 10);
                }
            }
            //
            //LabelsX & Graph
            //
            if (-equationI * amplitude + offSetY + graph.Height / 2f < graph.Height &&
                -equationI * amplitude + offSetY + graph.Height / 2f > -graph.Height)
            {
                g.DrawLine(Pen_AxisY,
                           -offSetX + graph.Width / 2f,
                           -equationI * amplitude + offSetY + graph.Height / 2f,
                           (showX * pitch - offSetX) + graph.Width / 2f,
                           -equationI * amplitude + offSetY + graph.Height / 2f);
            }



            for (int i = -graph.Width / 2; i < graph.Width / 2; i++)
            {
                if ((i + offSetX) % 20f == 0)
                {
                    g.DrawLine(Pen_AxisX,
                        i + graph.Width / 2f,
                        offSetY + graph.Height / 2 + 7,
                        i + graph.Width / 2f,
                        offSetY + graph.Height / 2 - 7);

                    g.DrawString(((i + offSetX) / pitch).ToString(),
                       graph_font,
                       Brush_Values,
                       i + graph.Width / 2,
                       offSetY + graph.Height / 2f - 20);

                }

                if (((i + offSetX) / pitch) <= showX && ((i + 1 + offSetX) / pitch) >= showX)
                {
                    g.DrawLine(Pen_Show,
                        (showX * pitch - offSetX) + graph.Width / 2f,
                        folost,
                        (showX * pitch - offSetX) + graph.Width / 2f,
                        offSetY + graph.Height / 2f);
                }

                //
                //Graph
                //
                equationI = -(float)equationC.answer((i + offSetX) / pitch) * amplitude + offSetY + graph.Height / 2f;
                equationI1 = -(float)equationC.answer((i + 1 + offSetX) / pitch) * amplitude + offSetY + graph.Height / 2f;

                if (equationI < int.MaxValue / 2 &&
                equationI > int.MinValue / 2 &&
                equationI1 < int.MaxValue / 2 &&
                equationI1 > int.MinValue / 2)
                {
                    g.DrawLine(Pen_Graph,
                        i + graph.Width / 2f,
                        equationI,
                        (i + 1) + graph.Width / 2f,
                        equationI1);
                }
            }
            //
            //Axis
            //
            g.DrawLine(Pen_AxisX,
                        0,
                        offSetY + graph.Height / 2f,
                        graph.Width,
                        offSetY + graph.Height / 2f);

            g.DrawLine(Pen_AxisY,
                        -offSetX + graph.Width / 2f,
                        0,
                        -offSetX + graph.Width / 2f,
                        graph.Height);
            //
            //Show Box
            //
            if (cB_Show.Checked)
            {
                string s = showX + ";" + equationC.answer(showX);
                SizeF sSize = g.MeasureString(s, graph_font);

                g.FillRectangle(Brush_Values,
                    (showX * pitch - offSetX) + graph.Width / 2f + 20,
                    folost,
                    sSize.Width + 12, sSize.Height + 12);

                g.FillRectangle(Brush_White,
                    (showX * pitch - offSetX) + graph.Width / 2f + 20 + 3,
                    folost + 3,
                    sSize.Width + 6, sSize.Height + 6);

                g.DrawString(s,
                    graph_font,
                    Brush_Values,
                    (showX * pitch - offSetX) + graph.Width / 2f + 20 + 6,
                    folost + 6);
            }
            pB_graph.Image = graph;
            Pen_AxisX.Dispose();
            Pen_AxisY.Dispose();
            Pen_Graph.Dispose();
            Pen_Grid.Dispose();
            Brush_Values.Dispose();
            g.Dispose();
        }

        private void b_X_Click(object sender, EventArgs e)
        {
            if (next)
            {
                newNumber();
                next = true;
            }
            theX = !theX;
            print();
        }

        private void cB_Show_CheckedChanged(object sender, EventArgs e)
        {
            drawGraph();
        }

        private void tB_Amplitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            float i;
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:

                    if (float.TryParse(tB_Amplitude.Text, out i))
                    {
                        amplitude = float.Parse(tB_Amplitude.Text, CultureInfo.CreateSpecificCulture("fr-FR"));
                        drawGraph();

                    }
                    else tB_Amplitude.Text = amplitude.ToString();
                    break;
                case (char)Keys.Escape:
                    tB_Amplitude.Text = amplitude.ToString();
                    break;
            }
        }

        private void tB_Pitch_KeyPress(object sender, KeyPressEventArgs e)
        {
            float i;
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:

                    if (float.TryParse(tB_Pitch.Text, out i))
                    {
                        if (float.Parse(tB_Pitch.Text, CultureInfo.InvariantCulture.NumberFormat) != 0)
                            pitch = float.Parse(tB_Pitch.Text, CultureInfo.CreateSpecificCulture("fr-FR"));
                        else tB_Pitch.Text = pitch.ToString();
                        drawGraph();

                    }
                    else tB_Pitch.Text = pitch.ToString();
                    break;
                case (char)Keys.Escape:
                    tB_Pitch.Text = pitch.ToString();
                    break;
            }
        }

        private void tB_OffSetX_KeyPress(object sender, KeyPressEventArgs e)
        {
            int i;
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:

                    if (int.TryParse(tB_OffSetX.Text, out i))
                    {
                        offSetX = int.Parse(tB_OffSetX.Text, CultureInfo.InvariantCulture.NumberFormat);
                        drawGraph();

                    }
                    else tB_OffSetX.Text = offSetX.ToString();
                    break;
                case (char)Keys.Escape:
                    tB_OffSetX.Text = offSetX.ToString();
                    break;
            }
        }

        private void tB_OffSetY_KeyPress(object sender, KeyPressEventArgs e)
        {
            int i;
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:

                    if (int.TryParse(tB_OffSetY.Text, out i))
                    {
                        offSetY = int.Parse(tB_OffSetY.Text, CultureInfo.InvariantCulture.NumberFormat);
                        drawGraph();

                    }
                    else tB_OffSetY.Text = offSetY.ToString();
                    break;
                case (char)Keys.Escape:
                    tB_OffSetY.Text = offSetY.ToString();
                    break;
            }
        }

        private void tB_showX_KeyPress(object sender, KeyPressEventArgs e)
        {
            float i;
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:

                    if (float.TryParse(tB_showX.Text, out i))
                    {
                        showX = float.Parse(tB_showX.Text, CultureInfo.CreateSpecificCulture("fr-FR"));
                        drawGraph();

                    }
                    else tB_showX.Text = showX.ToString();
                    break;
                case (char)Keys.Escape:
                    tB_showX.Text = showX.ToString();
                    break;
            }
        }

        private void tB_showY_KeyPress(object sender, KeyPressEventArgs e)
        {
            float i;
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:

                    if (float.TryParse(tB_showY.Text, out i))
                    {
                        showY = float.Parse(tB_showY.Text, CultureInfo.CreateSpecificCulture("fr-FR"));
                        drawGraph();

                    }
                    else tB_showY.Text = showY.ToString();
                    break;
                case (char)Keys.Escape:
                    tB_showY.Text = showY.ToString();
                    break;
            }
        }

        private void pB_graph_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                grab_point = new Point(MousePosition.X, MousePosition.Y);
                timer1.Enabled = true;
            }
        }

        private void pB_graph_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            offSetX += grab_point.X - MousePosition.X;
            offSetY += -(grab_point.Y - MousePosition.Y);
            grab_point = new Point(MousePosition.X, MousePosition.Y);
            drawGraph();
        }

        #region Focus
        private void tB_OffSetX_Enter(object sender, EventArgs e)
        {
            FocusOX = true;
        }

        private void tB_OffSetX_Leave(object sender, EventArgs e)
        {
            FocusOX = false;
            int i;
            if (int.TryParse(tB_OffSetX.Text, out i))
            {
                offSetX = int.Parse(tB_OffSetX.Text, CultureInfo.InvariantCulture.NumberFormat);
                drawGraph();

            }
            else tB_OffSetX.Text = offSetX.ToString();
        }

        private void tB_OffSetY_Enter(object sender, EventArgs e)
        {
            FocusOY = true;
        }

        private void tB_OffSetY_Leave(object sender, EventArgs e)
        {
            FocusOY = false;
            int i;
            if (int.TryParse(tB_OffSetY.Text, out i))
            {
                offSetY = int.Parse(tB_OffSetY.Text, CultureInfo.InvariantCulture.NumberFormat);
                drawGraph();

            }
            else tB_OffSetY.Text = offSetY.ToString();
        }

        private void tB_Pitch_Enter(object sender, EventArgs e)
        {
            FocusP = true;
        }

        private void tB_Pitch_Leave(object sender, EventArgs e)
        {
            FocusP = false;
            float i;
            if (float.TryParse(tB_Pitch.Text, out i))
            {
                if (float.Parse(tB_Pitch.Text, CultureInfo.InvariantCulture) != 0)
                    pitch = float.Parse(tB_Pitch.Text, CultureInfo.CreateSpecificCulture("fr-FR"));
                else tB_Pitch.Text = pitch.ToString();
                drawGraph();

            }
            else tB_Pitch.Text = pitch.ToString();
        }

        private void tB_Amplitude_Enter(object sender, EventArgs e)
        {
            FocusA = true;
        }

        private void tB_Amplitude_Leave(object sender, EventArgs e)
        {
            FocusA = false;
            float i;
            if (float.TryParse(tB_Amplitude.Text, out i))
            {
                amplitude = float.Parse(tB_Amplitude.Text, CultureInfo.CreateSpecificCulture("fr-FR"));
                drawGraph();

            }
            else tB_Amplitude.Text = amplitude.ToString();
        }

        private void tB_showX_Leave(object sender, EventArgs e)
        {
            FocusSX = false;
            float i;
            if (float.TryParse(tB_showX.Text, out i))
            {
                showX = float.Parse(tB_showX.Text, CultureInfo.CreateSpecificCulture("fr-FR"));
                drawGraph();

            }
            else tB_showX.Text = showX.ToString();
        }

        private void tB_showY_Leave(object sender, EventArgs e)
        {
            FocusSY = false;
            float i;
            if (float.TryParse(tB_showY.Text, out i))
            {
                showY = float.Parse(tB_showY.Text, CultureInfo.CreateSpecificCulture("fr-FR"));
                drawGraph();

            }
            else tB_showY.Text = showY.ToString();
        }

        private void tB_showY_Enter(object sender, EventArgs e)
        {
            FocusSY = true;
        }

        private void tB_showX_Enter(object sender, EventArgs e)
        {
            FocusSX = true;
        }

        #endregion

        #endregion

        private void b_Y_Click(object sender, EventArgs e)
        {
            try
            {
                ComServerWP3Lib.Agent agent = new ComServerWP3Lib.Agent();
                ComServerWP3Lib.IMesh mesh = (ComServerWP3Lib.IMesh)agent;
               
                ComServerWP3Lib._Data data;
                data.sizeX = 256;
                data.sizeY = 256;
                unsafe
                {

                    float* pArray = stackalloc float[256 * 256];
                    for (int x = 0; x < 256; ++x)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            pArray[x + y * 256] = (float)equationC.answer(Math.Sqrt(Math.Pow(x - 128,2) + Math.Pow(y - 128,2)));
                        }
                    }
                    IntPtr intPtr = new IntPtr((void*)pArray);
                    data.points = intPtr;
                    mesh.SetData(data);

                    for (int x = 0; x < 256; ++x)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            mesh.SetCell(pArray[x + y * 256], x + y * 256);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected COM exception: " + ex.Message);
            }
        }

        private void b_left_bracket_Click(object sender, EventArgs e)
        {
            brascet_balance++;
            put_operator(Operation.Bracket_Left);
        }

        private void b_right_bracket_Click(object sender, EventArgs e)
        {
            if (brascet_balance > 0)
            {
                brascet_balance--;
                put_operator(Operation.Bracket_Right);
                next = true;
                print();
            }
        }

        private void b_Inv_Click(object sender, EventArgs e)
        {
            Inv = !Inv;
        }

        private void b_int_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Intinger);
        }

        private void b_cbrt_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Root, 3);
        }

        private void grupowanieLiczbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grupowanieLiczbToolStripMenuItem.Checked = !grupowanieLiczbToolStripMenuItem.Checked;
            print();
        }

        private void b_root_Click(object sender, EventArgs e)
        {
            put_operator(Operation.Root, value);
        }
    }
}
