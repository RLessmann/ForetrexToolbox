using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForetrexToolbox.KML
{
  internal class KmlPoint
  {
    public KmlPoint(string name, double lat, double lon, double ele)
    {
      Name = name;
      Latitude = lat;
      Longitude = lon;
      Elevation = ele;
    }

    public string Name { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double Elevation { get; private set; }
  }
}
