using System.Diagnostics.CodeAnalysis;

namespace ForetrexToolbox.AIR
{
  class Airport
  {
    internal Airport( string name, string iata, double lat, double lon, double ele)
    {
      Name = name;
      Iata = iata;
      Latitude = lat;
      Longitude = lon;
      Elevation = ele;
    }
    public string Name { get; private set; }
    public string Iata { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double Elevation { get; private set; }
  }
}
