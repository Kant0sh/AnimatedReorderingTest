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

namespace AnimatedReorderingTest
{
    /// <summary>
    /// Interaction logic for ReorderingStack.xaml
    /// </summary>
    public partial class ReorderingStack : ItemsControl
    {
        public ReorderingStack()
        {
            AllowDrop = true;
            InitializeComponent();
            Drop += ReorderingStack_Drop;
            Label lbl1 = new Label();
            lbl1.Content = "Hallo";
            lbl1.MouseDown += Lbl_MouseDown;
            lbl1.DragEnter += Lbl_DragEnter;
            lbl1.DragLeave += Lbl_DragLeave;
            Label lbl2 = new Label();
            lbl2.Content = "Welt!";
            lbl2.MouseDown += Lbl_MouseDown;
            lbl2.DragEnter += Lbl_DragEnter;
            lbl2.DragLeave += Lbl_DragLeave;
            AddChild(lbl1);
            AddChild(lbl2);
            Loaded += ReorderingStack_Loaded;
        }

        private void Lbl_DragLeave(object sender, DragEventArgs e)
        {

        }

        private void Lbl_DragEnter(object sender, DragEventArgs e)
        {
            if (sender == e.Data.GetData("obj")) return;
            Console.WriteLine("sender: " + sender);
            Console.WriteLine("data: " + e.Data.GetData("obj"));
            int i = 0;
            if(Items.IndexOf(e.Data.GetData("obj")) < Items.IndexOf(sender)) i = 1;
            Items.Remove(e.Data.GetData("obj"));
            Items.Insert(Items.IndexOf(sender) + i, e.Data.GetData("obj"));
            InvalidateVisual();
            Items.Refresh();
            Console.WriteLine("#####");
        }

        private void ReorderingStack_Drop(object sender, DragEventArgs e)
        {

        }

        private void Lbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject();
            data.SetData("obj", sender);
            DragDrop.DoDragDrop(sender as DependencyObject, data, DragDropEffects.Move);
        }

        private void ReorderingStack_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
