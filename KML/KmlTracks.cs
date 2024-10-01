using ForetrexToolbox.AIR;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ForetrexToolbox.KML
{
  internal class KmlTracks
  {
    private List<List<KmlPoint>> _tracks = new List<List<KmlPoint>>();
    private double _angle = double.Parse(ConfigurationManager.AppSettings["ThinoutAngle"] ?? "0.5", CultureInfo.InvariantCulture);
    private int _strike = int.Parse(ConfigurationManager.AppSettings["ThinoutStrike"] ?? "10", CultureInfo.InvariantCulture);

    [RequiresUnreferencedCode("Calls ForetrexToolbox.Serializer<T>.Deserialize(String)")]
    public KmlTracks(FileInfo? fi)
    {
      if (fi != null && fi.Exists)
      {
        Serializer<kmlType> serializer = new Serializer<kmlType>();
        kmlType? kml = serializer.DeserializeStream(fi.OpenRead());
        if (kml != null &&
            kml.Document != null &&
            kml.Document.Placemark != null)
        {
          for (int i = 0; i < kml.Document.Placemark.Length; i++)
          {
            placemarkType p = kml.Document.Placemark[i];
            List<KmlPoint> pnts = new List<KmlPoint>();
            if (p.LineString != null && p.LineString.coordinates != null)
            {
              string[] values = p.LineString.coordinates.Split(new char[] { ',', '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
              for (int j = 0; j < values.Length - 3; j += 3)
              {
                string nam = string.Format(CultureInfo.InvariantCulture, "{0:0}{1:000}", i + 1, (j / 3) + 1);
                double lon = double.Parse(values[j], CultureInfo.InvariantCulture);
                double lat = double.Parse(values[j + 1], CultureInfo.InvariantCulture);
                double ele = double.Parse(values[j + 2], CultureInfo.InvariantCulture);
                pnts.Add(new KmlPoint(nam, lat, lon, ele));
              }
            }
            if (pnts.Count > 0)
            {
              _tracks.Add(pnts);
            }
          }
        }
      }
    }

    public List<List<Gpx10.gpxRteRtept>> Routes
    {
      get
      {
        List<List<Gpx10.gpxRteRtept>> lst = new List<List<Gpx10.gpxRteRtept>>();
        foreach (List<KmlPoint> track in _tracks)
        {
          List<Gpx10.gpxRteRtept> route = new List<Gpx10.gpxRteRtept>();
          foreach (KmlPoint point in track)
          {
            Gpx10.gpxRteRtept rp = new Gpx10.gpxRteRtept();
            rp.name = point.Name;
            rp.lat = (decimal)point.Latitude;
            rp.lon = (decimal)point.Longitude;
            rp.ele = (decimal)point.Elevation;
            rp.eleSpecified = true;
            route.Add(rp);
          }
          if (route.Count > 0)
          {
            lst.Add(route);
          }
        }
        return lst;
      }
    }

    public void Thinout()
    {
      foreach (List<KmlPoint> track in _tracks)
      {
        List<double> directions = new List<double>();
        int strike = 0;
        int removed = 0;
        for (int i = 1; i < track.Count; i++)
        {
          directions.Add((track[i].Longitude - track[i - 1].Longitude) / (track[i].Latitude - track[i - 1].Latitude));
        }
        for (int i=directions.Count-3; i >=0; i--)
        {
          if (Math.Abs(directions[i] - directions[i+1]) < _angle)
          {
            if (removed - i == 2)
            {
              strike++;
            }
            if (strike < _strike)
            {
              track.RemoveAt(i + 1);
              removed = i + 1;
            }
            else
            {
              strike = 0;
            }
          }
        }
      }
    }
  }
}
