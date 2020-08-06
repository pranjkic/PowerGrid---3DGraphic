using PZ3.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PZ3.Handlers
{
    public class TransformationHandler
    {
        private List<Viewport3D> shapes;
        private bool centered;
        private System.Windows.Point point;
        private bool rotating = false;
        private Quaternion rotation;
        private Quaternion rotationDelta;
        private double scale;
        private double scaleDelta;
        
        private Vector3D translate;
        private Vector3D translateDelta;
        public bool middlePressed = false;

        ScrollViewer scrollViewer;
        MainWindow window;
        Viewport3D viewport;

        public TransformationHandler(MainWindow element, ScrollViewer scrollViewer, Viewport3D viewport)
        {
            this.window = element;
            this.scrollViewer = scrollViewer;
            this.viewport = viewport;

            Reset();

            Attach();
            Slaves.Add(viewport);
        }

        #region scroll
        System.Windows.Point scrollMousePoint = new System.Windows.Point();
        double hOff = 1;
        double vEff = 1;
        public void scrollViewerPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scrollMousePoint = e.GetPosition(scrollViewer);
            hOff = scrollViewer.HorizontalOffset;
            vEff = scrollViewer.VerticalOffset;
            scrollViewer.CaptureMouse();
        }
        private void scrollViewerPreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (scrollViewer.IsMouseCaptured)
            {
                scrollViewer.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(scrollViewer).X));
                scrollViewer.ScrollToVerticalOffset(vEff + (scrollMousePoint.Y - e.GetPosition(scrollViewer).Y));
            }
        }

        private void scrollViewerPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scrollViewer.ReleaseMouseCapture();
        }

        private void scrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Point mouseAtImage = e.GetPosition(viewport);
            System.Windows.Point mouseAtScrollViewer = e.GetPosition(scrollViewer);

            ScaleTransform st = viewport.LayoutTransform as ScaleTransform;
            if (st == null)
            {
                st = new ScaleTransform();
                viewport.LayoutTransform = st;
            }

            if (e.Delta > 0)
            {
                st.ScaleX = st.ScaleY = st.ScaleX * 1.25;
                if (st.ScaleX > 64) st.ScaleX = st.ScaleY = 64;
            }
            else
            {
                st.ScaleX = st.ScaleY = st.ScaleX / 1.25;
                if (st.ScaleX < 1) st.ScaleX = st.ScaleY = 1;
            }
            #region [this step is critical for offset]
            scrollViewer.ScrollToHorizontalOffset(0);
            scrollViewer.ScrollToVerticalOffset(0);
            window.UpdateLayout();
            #endregion

            Vector offset = viewport.TranslatePoint(mouseAtImage, scrollViewer) - mouseAtScrollViewer;
            scrollViewer.ScrollToHorizontalOffset(offset.X);
            scrollViewer.ScrollToVerticalOffset(offset.Y);
            window.UpdateLayout();

            e.Handled = true;
        }
        #endregion

        public List<Viewport3D> Slaves
        {
            get { return shapes ?? (shapes = new List<Viewport3D>()); }
            set { shapes = value; }
        }

        public void Attach()
        {
            
            window.MouseRightButtonDown += MouseRightDownHandler;
            window.MouseDown += MiddleButtonDownHandler;
            window.MouseUp += MiddleButtonUpHandler;
            window.MouseMove += MouseMoveHandler;

            scrollViewer.PreviewMouseLeftButtonDown += scrollViewerPreviewMouseLeftButtonDown;
            scrollViewer.PreviewMouseMove += scrollViewerPreviewMouseMove;
            scrollViewer.PreviewMouseLeftButtonUp += scrollViewerPreviewMouseLeftButtonUp;
            scrollViewer.PreviewMouseWheel += scrollViewerPreviewMouseWheel;
        }
        private void Reset()
        {
            rotation = new Quaternion(0, 0, 0, 1);
            scale = 1;
            translate.X = 0;
            translate.Y = 0;
            translate.Z = 0;
            translateDelta.X = 0;
            translateDelta.Y = 0;
            translateDelta.Z = 0;

            rotationDelta = Quaternion.Identity;
            scaleDelta = 1;
            UpdateShapes(rotation, scale, translate);
        }
        
        private void UpdateShapes(Quaternion q, double s, Vector3D t)
        {
            try
            {
                if (shapes != null)
                {
                    foreach (var i in shapes)
                    {
                        var mv = i.Children[0] as ModelVisual3D;
                        var t3Dg = mv.Transform as Transform3DGroup;

                        var groupScaleTransform = t3Dg.Children[0] as ScaleTransform3D;
                        var groupRotateTransform = t3Dg.Children[1] as RotateTransform3D;
                        var groupTranslateTransform = t3Dg.Children[2] as TranslateTransform3D;

                        groupScaleTransform.ScaleX = s;
                        groupScaleTransform.ScaleY = s;
                        groupScaleTransform.ScaleZ = s;
                        groupRotateTransform.Rotation = new AxisAngleRotation3D(q.Axis, q.Angle);
                        groupTranslateTransform.OffsetX = t.X;
                        groupTranslateTransform.OffsetY = t.Y;
                        groupTranslateTransform.OffsetZ = t.Z;
                    }
                }
            }
            catch { }
        }

        public static ArrayList models = new ArrayList();
        private GeometryModel3D hitgeo;

        private void MouseRightDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton.Equals(MouseButtonState.Pressed))
            {
                System.Windows.Point mouseposition = e.GetPosition(viewport);
                Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
                Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

                PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
                RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);
   
                hitgeo = null;
                VisualTreeHelper.HitTest(viewport, null, HitResult, pointparams);
            }
        }       

        private HitTestResultBehavior HitResult(System.Windows.Media.HitTestResult rawresult)
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                DiffuseMaterial darkSide = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Colors.Red));
                bool gasit = false;
                int cnt = 0;
                for (int i = 0; i < models.Count; i++)
                {
                    if (models[i] == rayResult.ModelHit)
                    {
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                        if (MapHandler.Entities[i].GetType() != typeof(LineEntity))                            
                            MessageBox.Show(MapHandler.Entities[i].ToString());
                        else
                        {
                            Brush b = null;
                            foreach (var item in MapHandler.Entities)
                            {
                                if (item.Value.Id == ((LineEntity)MapHandler.Entities[i]).FirstEnd || item.Value.Id == ((LineEntity)MapHandler.Entities[i]).SecondEnd)
                                {
                                    if (cnt > 2)
                                        break;
                                    if (((DiffuseMaterial)((GeometryModel3D)TransformationHandler.models[item.Key]).Material).Brush == Brushes.ForestGreen)
                                    {
                                        cnt++;
                                        b = Brushes.ForestGreen;
                                        foreach (var child in window.MyModel.Children)
                                        {
                                            if (child == ScreenHandler.drawModels[item.Key])
                                            {
                                                ((GeometryModel3D)((ModelVisual3D)child).Content).Material = new DiffuseMaterial(Brushes.Pink);
                                            }
                                        }
                                    }
                                    else if (((DiffuseMaterial)((GeometryModel3D)TransformationHandler.models[item.Key]).Material).Brush == Brushes.Pink)
                                    {
                                        cnt++;
                                        b = Brushes.Pink;
                                        foreach (var child in window.MyModel.Children)
                                        {
                                            if (child == ScreenHandler.drawModels[item.Key])
                                            {
                                                ((GeometryModel3D)((ModelVisual3D)child).Content).Material = new DiffuseMaterial(Brushes.ForestGreen);
                                            }
                                        }
                                    }
                                    else if (((DiffuseMaterial)((GeometryModel3D)TransformationHandler.models[item.Key]).Material).Brush == Brushes.Red)
                                    {
                                        cnt++;
                                        b = Brushes.Red;
                                        foreach (var child in window.MyModel.Children)
                                        {
                                            if (child == ScreenHandler.drawModels[item.Key])
                                            {
                                                ((GeometryModel3D)((ModelVisual3D)child).Content).Material = new DiffuseMaterial(Brushes.Yellow);
                                            }
                                        }
                                    }
                                    else if (((DiffuseMaterial)((GeometryModel3D)TransformationHandler.models[item.Key]).Material).Brush == Brushes.Yellow)
                                    {
                                        cnt++;
                                        foreach (var child in window.MyModel.Children)
                                        {
                                            if (child == ScreenHandler.drawModels[item.Key])
                                            {
                                                ((GeometryModel3D)((ModelVisual3D)child).Content).Material = new DiffuseMaterial(Brushes.Red);
                                            }
                                        }
                                    }
                                    else if (((DiffuseMaterial)((GeometryModel3D)TransformationHandler.models[item.Key]).Material).Brush == Brushes.Blue)
                                    {
                                        cnt++;
                                        b = Brushes.Blue;
                                        foreach (var child in window.MyModel.Children)
                                        {
                                            if (child == ScreenHandler.drawModels[item.Key])
                                            {
                                                ((GeometryModel3D)((ModelVisual3D)child).Content).Material = new DiffuseMaterial(Brushes.Orange);
                                            }
                                        }
                                    }
                                    else if (((DiffuseMaterial)((GeometryModel3D)TransformationHandler.models[item.Key]).Material).Brush == Brushes.Orange)
                                    {
                                        cnt++;
                                        b = Brushes.Orange;
                                        foreach (var child in window.MyModel.Children)
                                        {
                                            if (child == ScreenHandler.drawModels[item.Key])
                                            {
                                                ((GeometryModel3D)((ModelVisual3D)child).Content).Material = new DiffuseMaterial(Brushes.Blue);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    if (cnt > 2)
                        break;
                }
                if (!gasit)
                {
                    hitgeo = null;
                }
            }
            return HitTestResultBehavior.Stop;
        }

        private void MiddleButtonDownHandler(object sender, MouseEventArgs e)
        {
            middlePressed = false;
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                middlePressed = true;
                e.Handled = true;

                var el = (UIElement)sender;
                point = e.MouseDevice.GetPosition(el);
                if (!centered)
                {
                    var camera = (ProjectionCamera)shapes[0].Camera;
                    centered = true;
                }
                rotating = (e.MiddleButton == MouseButtonState.Pressed);

                el.CaptureMouse();
            }
        }
        
        private void MiddleButtonUpHandler(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released && middlePressed)
            {
                e.Handled = true;

                if (rotating)
                    rotation = rotationDelta * rotation;
                else
                {
                    translate += translateDelta;
                    translateDelta.X = 0;
                    translateDelta.Y = 0;
                }
                var el = (UIElement)sender;
                el.ReleaseMouseCapture();
            }
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            var el = (UIElement)sender;

            if (el.IsMouseCaptured)
            {
                var delta = point - e.MouseDevice.GetPosition(el);
                var t = new Vector3D();

                delta /= 2;
                var q = rotation;

                if (rotating)
                {
                    var mouse = new Vector3D(delta.X, -delta.Y, 0);
                    var axis = Vector3D.CrossProduct(mouse, new Vector3D(0, 0, 1));
                    var len = axis.Length;
                    if (len < 0.00001)
                        rotationDelta = new Quaternion(new Vector3D(0, 0, 1), 0);
                    else
                        rotationDelta = new Quaternion(axis, len);

                    q = rotationDelta * rotation;
                }
                else
                {
                    delta /= 20;
                    translateDelta.X = delta.X * -1;
                    translateDelta.Y = delta.Y;
                }

                t = translate + translateDelta;

                UpdateShapes(q, scale * scaleDelta, t);
            }
        }
    }
}
    
