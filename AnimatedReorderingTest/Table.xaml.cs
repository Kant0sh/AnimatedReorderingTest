using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AnimatedReorderingTest
{
    /// <summary>
    /// Interaction logic for Table.xaml
    /// </summary>
    public partial class Table : Canvas
    {

        private int rowCount = 15;
        private double rowHeight = 100;
        private int moveWaitMillis = 200;

        public Table()
        {
            InitializeComponent();
            Loaded += Table_Loaded;
            Height = rowHeight * rowCount;
        }

        DispatcherTimer moveTimer;
        private void Table_Loaded(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < rowCount; i++)
            {
                TableRow tr = new TableRow();
                tr.Width = 500;
                tr.Height = rowHeight;
                tr.PropertyChanged += TableRow_PropertyChanged;
                tr.Row = i;
                tr.Click += Tr_Click;
                Children.Add(tr);
            }
            PreviewMouseMove += Table_PreviewMouseMove;
            MouseUp += Table_MouseUp;
            MouseLeave += Table_MouseLeave;
            moveTimer = new DispatcherTimer(DispatcherPriority.Render);
            moveTimer.Interval = TimeSpan.FromMilliseconds(moveWaitMillis);
            moveTimer.Tick += MoveTimer_Tick;
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            MoveRows(MoveUp);
        }

        private void Table_MouseLeave(object sender, MouseEventArgs e)
        {
            MoveRows(MoveUp);
            DropRow();
        }

        private void Table_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MoveRows(MoveUp);
            DropRow();
        }

        Point oldPos;
        Point startPos;
        int newRow;
        bool MoveUp;
        private void Table_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            double deltaY = 0;
            double startDeltaY = 0;
            if (clicked)
            {
                startDeltaY = e.GetPosition(this).Y - startPos.Y;
                if (oldPos != null) deltaY = e.GetPosition(this).Y - oldPos.Y;
                if (Math.Abs(startDeltaY) > SystemParameters.MinimumVerticalDragDistance)
                {
                    dragging = true;
                }
            }

            MoveUp = startDeltaY < 0;
            if (dragging)
            {
                SetTop(clickedRow, GetTop(clickedRow) + deltaY);
                moveTimer.Stop();
                moveTimer.Start();
            }
            oldPos = e.GetPosition(this);
        }

        private void MoveRows(bool up)
        {
            if (clickedRow != null)
            {
                int newNewRow = GetRow(clickedRow);
                int[] skippedRows;
                if (newRow > newNewRow) skippedRows = Enumerable.Range(newNewRow, newRow - newNewRow + 1).ToArray();
                else skippedRows = Enumerable.Range(newRow, newNewRow - newRow + 1).ToArray();
                foreach (int i in skippedRows)
                {
                    Console.Write(i);
                    TableRow tr = GetTableRowFromRow(i);
                    if (tr != null && tr != clickedRow && !tr.IsMoving)
                    {
                        if (!up)
                        {
                            GoToRow(tr, i - 1);
                        }
                        else
                        {
                            GoToRow(tr, i + 1);
                        }
                    }
                }
                Console.WriteLine();
                newRow = newNewRow;
                startPos = oldPos;
            }
        }

        public int GetRow(TableRow tr)
        {
            return (int)Math.Round((GetTop(tr) + (rowHeight / 2)) / rowHeight);
        }

        private TableRow GetTableRowFromRow(int row)
        {
            foreach(TableRow tr in Children)
            {
                if (tr.Row == row) return tr;
            }
            return null;
        }
        
        bool clicked;
        TableRow clickedRow;
        bool dragging;
        private void Tr_Click(object sender, object origSender, MouseButtonEventArgs e)
        {
            clicked = true;
            clickedRow = sender as TableRow;
            clickedRow.SetValue(ZIndexProperty, 1);
            newRow = clickedRow.Row;
            DropShadowEffect effect = new DropShadowEffect();
            effect.ShadowDepth = 0;
            effect.Opacity = 0.5;
            effect.Color = Colors.Black;
            effect.BlurRadius = 15;
            effect.Direction = 0;
            clickedRow.Effect = effect;
            startPos = e.GetPosition(this);
        }

        private void DropRow()
        {
            clicked = false;
            dragging = false;
            if (clickedRow != null)
            {
                clickedRow.SetValue(ZIndexProperty, 0);
                clickedRow.Effect = null;
                clickedRow.Row = newRow;
            }
            clickedRow = null;
        }

        private void GoToRow(TableRow tr, int row)
        {
            //Console.WriteLine("GoTo " + row);
            tr.IsMoving = true;
            DoubleAnimation anim = new DoubleAnimation(row * 100, TimeSpan.FromMilliseconds(moveWaitMillis));
            anim.Completed += (s, _) => fixRow(tr, row);
            anim.FillBehavior = FillBehavior.Stop;
            anim.AccelerationRatio = 0.7;
            anim.DecelerationRatio = 0.3;
            tr.BeginAnimation(Canvas.TopProperty, anim);
        }

        private void fixRow(TableRow tr, int row)
        {
            //Console.WriteLine("Fix " + row);
            tr.IsMoving = false;
            tr.Row = row;
        }

        private void TableRow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Row") SetTop(sender as TableRow, (sender as TableRow).Row * rowHeight);
        }
    }
}
