using PZ3.Model;
using PZ3.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PZ3.Handlers
{
    public class MapHandler
    {
        public static Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();
        public static List<PointStack> Points = new List<PointStack>();

        private static bool HasEntityInRadius(double x, double y, out int index, out double xx, out double yy)
        {
            index = 0;
            bool retVal = false;
            xx = -1;
            yy = -1;
            double radius = 0.05;

            foreach (PointStack obj in Points)
            {
                if (x >= obj.X - radius && x <= obj.X + radius && y >= obj.Y - radius && y <= obj.Y + radius)
                {
                    xx = obj.X;
                    yy = obj.Y;
                    retVal = true;
                    break;
                }
                index++;
            }
            return retVal;
        }

        public static NetworkModel LoadModelToMap(NetworkModel networkModel, ModelVisual3D myModel)
        {
            networkModel = XmlHandler.Load<NetworkModel>(@"..\..\Geographic.xml");

            // SUBSTATIONS
            for (int i = 0; i < networkModel.Substations.Count; i++)
            {
                SubstationEntity s = networkModel.Substations[i];
                double latitude, longitude, mapX, mapY;

                CoordinatesHandler.ToLatLon(networkModel.Substations[i].X, networkModel.Substations[i].Y, 34, out latitude, out longitude);
                networkModel.Substations[i].X = latitude;
                networkModel.Substations[i].Y = longitude;

                CoordinatesHandler.FromCoordinatesToMapPosition(latitude, longitude, out mapX, out mapY);
                networkModel.Substations[i].MapX = mapX;
                networkModel.Substations[i].MapY = mapY;

                if (mapX != -1 && mapY != -1)
                {
                    double z = 0;

                    if (Points.Count != 0 || i != 0)
                    {
                        int index = 0;
                        double x;
                        double y;
                        if (HasEntityInRadius(mapX, mapY, out index, out x, out y))
                        {
                            s.MapX = x;
                            s.MapY = y;
                            mapX = x;
                            mapY = y;
                            Points[index].Count++;
                            z = 0.05 * Points[index].Count;
                        }
                        else
                        {
                            z = 0;
                            Points.Add(new PointStack(mapX, mapY, 0));
                        }
                    }
                    else
                    {
                        z = 0;
                        Points.Add(new PointStack(mapX, mapY, 0));
                    }

                    GeometryModel3D model3D = ScreenHandler.Make3DCube(mapX, mapY, z, 0.05, EntityType.Substation);
                    TransformationHandler.models.Add(model3D);

                    Entities.Add(TransformationHandler.models.Count - 1, s);

                    ScreenHandler.Draw3DCube(model3D, myModel);
                }
            }

            // NODES
            for (int i = 0; i < networkModel.Nodes.Count; i++)
            {
                NodeEntity n = networkModel.Nodes[i];
                double latitude, longitude, mapX, mapY;

                CoordinatesHandler.ToLatLon(networkModel.Nodes[i].X, networkModel.Nodes[i].Y, 34, out latitude, out longitude);
                networkModel.Nodes[i].X = latitude;
                networkModel.Nodes[i].Y = longitude;

                CoordinatesHandler.FromCoordinatesToMapPosition(latitude, longitude, out mapX, out mapY);
                networkModel.Nodes[i].MapX = mapX;
                networkModel.Nodes[i].MapY = mapY;

                if (mapX != -1 && mapY != -1)
                {
                    double z = 0;

                    if (i != 0)
                    {
                        int index = 0;
                        double x;
                        double y;
                        if (HasEntityInRadius(mapX, mapY, out index, out x, out y))
                        {
                            n.MapX = x;
                            n.MapY = y;
                            mapX = x;
                            mapY = y;
                            Points[index].Count++;
                            z = 0.05 * Points[index].Count;
                        }
                        else
                        {
                            z = 0;
                            Points.Add(new PointStack(mapX, mapY, 0));
                        }
                    }
                    else
                    {
                        z = 0;
                        Points.Add(new PointStack(mapX, mapY, 0));
                    }

                    GeometryModel3D model3D = ScreenHandler.Make3DCube(mapX, mapY, z, 0.05, EntityType.Node);
                    TransformationHandler.models.Add(model3D);

                    Entities.Add(TransformationHandler.models.Count - 1, n);

                    ScreenHandler.Draw3DCube(model3D, myModel);
                }
            }

            // SWITCHES
            for (int i = 0; i < networkModel.Switches.Count; i++)
            {
                SwitchEntity s = networkModel.Switches[i];
                double latitude, longitude, mapX, mapY;

                CoordinatesHandler.ToLatLon(networkModel.Switches[i].X, networkModel.Switches[i].Y, 34, out latitude, out longitude);
                networkModel.Switches[i].X = latitude;
                networkModel.Switches[i].Y = longitude;

                CoordinatesHandler.FromCoordinatesToMapPosition(latitude, longitude, out mapX, out mapY);
                networkModel.Switches[i].MapX = mapX;
                networkModel.Switches[i].MapY = mapY;

                if (mapX != -1 && mapY != -1)
                {
                    double z = 0;

                    if (i != 0)
                    {
                        int index = 0;
                        double x;
                        double y;
                        if (HasEntityInRadius(mapX, mapY, out index, out x, out y))
                        {
                            s.MapX = x;
                            s.MapY = y;
                            mapX = x;
                            mapY = y;
                            Points[index].Count++;
                            z = 0.05 * Points[index].Count;
                        }
                        else
                        {
                            z = 0;
                            Points.Add(new PointStack(mapX, mapY, 0));
                        }
                    }
                    else
                    {
                        z = 0;
                        Points.Add(new PointStack(mapX, mapY, 0));
                    }

                    GeometryModel3D model3D = ScreenHandler.Make3DCube(mapX, mapY, z, 0.05, EntityType.Switch);
                    TransformationHandler.models.Add(model3D);

                    Entities.Add(TransformationHandler.models.Count - 1, s);

                    ScreenHandler.Draw3DCube(model3D, myModel);
                }
            }

            // LINES
            int cnt = 0;

            for (int i = 0; i < networkModel.Lines.Count; i++)
            {
                Entity entityStart = null;
                Entity entityEnd = null;

                entityStart = Entities.Values.ToList().Where(x => x.Id == networkModel.Lines[i].FirstEnd).FirstOrDefault();
                entityEnd = Entities.Values.Where(x => x.Id == networkModel.Lines[i].SecondEnd).FirstOrDefault();


                if (entityStart == null || entityEnd == null)
                    continue;

                entityStart.NumConnctions++;
                entityEnd.NumConnctions++;
                cnt++;
                GeometryModel3D model3D = ScreenHandler.Make3DTube(entityStart.MapX + 0.025, entityStart.MapY + 0.025, entityEnd.MapX + 0.025, entityEnd.MapY + 0.025, 0, 0.05, EntityType.Switch);
                TransformationHandler.models.Add(model3D);
                ScreenHandler.Draw3DCube(model3D, myModel);
                Entities.Add(TransformationHandler.models.Count - 1, networkModel.Lines[i]);

            }

            return networkModel;
        }
    }
}
