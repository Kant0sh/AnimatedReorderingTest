using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimatedReorderingTest
{
    /// <summary>
    /// Interaction logic for TableRow.xaml
    /// </summary>
    public partial class TableRow : Grid, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string pCaller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pCaller));
        }

        UIElement _clickHandle;
        public UIElement ClickHandle
        {
            get
            {
                return _clickHandle;
            }
            set
            {
                _clickHandle = value;
                ClickHandle.PreviewMouseDown += ClickHandle_PreviewMouseDown;
                OnPropertyChanged();
            }
        }

        private void ClickHandle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnClick(this, sender, e);
        }

        int _row;
        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                _row = value;
                OnPropertyChanged();
            }
        }

        bool _moving;
        public bool IsMoving
        {
            get
            {
                return _moving;
            }
            set
            {
                _moving = value;
                OnPropertyChanged();
            }
        }

        public TableRow()
        {
            InitializeComponent();
            ClickHandle = rec;
        }

        public TableRow(int Row)
        {
            InitializeComponent();
            this.Row = Row;
            ClickHandle = rec;
        }

        public delegate void ClickEvent(object sender, object origSender, MouseButtonEventArgs e);
        public event ClickEvent Click;
        protected virtual void OnClick(object sender, object origSender, MouseButtonEventArgs e)
        {
            Click?.Invoke(sender, origSender, e);
        }
    }
}
