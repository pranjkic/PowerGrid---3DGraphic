using PZ3.Handlers;
using PZ3.Model;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PZ3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NetworkModel networkModel = new NetworkModel();
            networkModel = MapHandler.LoadModelToMap(networkModel, MyModel);

            TransformationHandler transformation = new TransformationHandler(this, scrollViewer, mainViewport);
        }

        List<ModelVisual3D> deleted03 = new List<ModelVisual3D>();
        List<ModelVisual3D> deleted35 = new List<ModelVisual3D>();
        List<ModelVisual3D> deleted5 = new List<ModelVisual3D>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (deleted5.Count == 0)
            {

                foreach (var item in MapHandler.Entities)
                {
                    if (item.Value.NumConnctions > 5)
                    {
                        if (item.Value.GetType() != typeof(LineEntity))
                        {
                            deleted5.Add(ScreenHandler.drawModels[item.Key]);
                            MyModel.Children.Remove(ScreenHandler.drawModels[item.Key]);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in deleted5)
                {
                    MyModel.Children.Add(item);
                }
                deleted5 = new List<ModelVisual3D>();

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (deleted03.Count == 0)
            {

                foreach (var item in MapHandler.Entities)
                {
                    if (item.Value.NumConnctions >= 0 && item.Value.NumConnctions < 3)
                    {
                        if (item.Value.GetType() != typeof(LineEntity))
                        {
                            deleted03.Add(ScreenHandler.drawModels[item.Key]);
                            MyModel.Children.Remove(ScreenHandler.drawModels[item.Key]);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in deleted03)
                {
                    MyModel.Children.Add(item);
                }
                deleted03 = new List<ModelVisual3D>();

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (deleted35.Count == 0)
            {

                foreach (var item in MapHandler.Entities)
                {
                    if (item.Value.NumConnctions >= 3 && item.Value.NumConnctions <= 5)
                    {
                        if (item.Value.GetType() != typeof(LineEntity))
                        {
                            deleted35.Add(ScreenHandler.drawModels[item.Key]);
                            MyModel.Children.Remove(ScreenHandler.drawModels[item.Key]);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in deleted35)
                {
                    MyModel.Children.Add(item);
                }
                deleted35 = new List<ModelVisual3D>();

            }
        }
    }
}
