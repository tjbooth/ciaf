using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cirrious.Conference.Core.ViewModels;
using Cirrious.Conference.Core.ViewModels.SessionLists;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Maps;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;

namespace Cirrious.Conference.UI.WP7.Views
{
    public abstract class BaseMapView : BaseView<MapViewModel>
    {
    }

    public partial class MapView : BaseMapView
    {
        public MapView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                SetupMap();
            }
        }

        private void SetupMap()
        {
            //Map.CredentialsProvider = new ApplicationIdCredentialsProvider(Private.BingKey);
            Map.ZoomLevel = 10;
            var hotelLocation = new GeoCoordinate(Convert.ToDouble(ViewModel.SharedTextSource.GetText("Latitude")),
                                                  Convert.ToDouble(ViewModel.SharedTextSource.GetText("Longitude")));
            Map.Center = hotelLocation;
            Map.Layers.Clear();
            MapLayer mapLayer = new MapLayer();

            // Draw marker for current position
            if (hotelLocation != null)
            {
                //DrawAccuracyRadius(mapLayer);
                DrawMapMarker(hotelLocation, Colors.Red, mapLayer);
            }

            Map.Layers.Add(mapLayer);

        }
        private void DrawMapMarker(GeoCoordinate coordinate, Color color, MapLayer mapLayer)
        {
            // Create a map marker
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0, 0));
            polygon.Points.Add(new Point(0, 50));
            polygon.Points.Add(new Point(25, 0));
            polygon.Fill = new SolidColorBrush(color);

            // Enable marker to be tapped for location information
            polygon.Tag = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            polygon.MouseLeftButtonUp += new MouseButtonEventHandler(Marker_Click);

            // Create a MapOverlay and add marker
            MapOverlay overlay = new MapOverlay();
            overlay.Content = polygon;
            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0.0, 1.0);
            mapLayer.Add(overlay);
        }
        private ReverseGeocodeQuery MyReverseGeocodeQuery = null;
        private void Marker_Click(object sender, MouseButtonEventArgs e)
        {
            Polygon p = (Polygon)sender;
            GeoCoordinate geoCoordinate = (GeoCoordinate)p.Tag;
            MyReverseGeocodeQuery = new ReverseGeocodeQuery();
            MyReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(geoCoordinate.Latitude, geoCoordinate.Longitude);
            MapsDirectionsTask mapsDirectionsTask = new MapsDirectionsTask();

            // If you set the geocoordinate parameter to null, the label parameter is used as a search term.
            LabeledMapLocation spaceNeedleLML = new LabeledMapLocation("Chilled in a Field", MyReverseGeocodeQuery.GeoCoordinate);

            mapsDirectionsTask.End = spaceNeedleLML;

            // If mapsDirectionsTask.Start is not set, the user's current location is used as the start point.

            mapsDirectionsTask.Show();

        }

    }
}