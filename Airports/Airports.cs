using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace ForetrexToolbox
{
  class Airports : SortedList<double, Airport>, IDisposable
  {
    private int _maxCount;
    List<string> _selectedAirports;
    List<string> _selectedContinents;
    private double _lat;
    private double _lon;

    public Airports(int maxCount, List<string> selectedAirports, List<string> selectedContinents)
    {
      _maxCount = maxCount;
      _selectedAirports = selectedAirports;
      if (_selectedAirports.Count == 0)
      {
        _selectedAirports.Add(ConfigurationManager.AppSettings["AirportType"] ?? "");
      }
      _selectedContinents = selectedContinents;
      if (_selectedContinents.Count == 0)
      {
        _selectedContinents.Add(ConfigurationManager.AppSettings["Continent"] ?? "");
      }
      _lat = Double.Parse(ConfigurationManager.AppSettings["HomeLatitude"] ?? "0.00", CultureInfo.InvariantCulture); 
      _lon = Double.Parse(ConfigurationManager.AppSettings["HomeLongitude"] ?? "0.00", CultureInfo.InvariantCulture);
      SelectAirports();
    }
    public void Dispose()
    {
    }

    public List<Gpx10.gpxWpt> WayPoints
    {
      get
      {
        List<Gpx10.gpxWpt> lst = new List<Gpx10.gpxWpt>();
        for (int i = 0; i < _maxCount && i < Count; i++)
        {
          Gpx10.gpxWpt wp = new Gpx10.gpxWpt();
          Airport air = this[this.Keys[i]];
          wp.name = air.Iata;
          wp.desc = air.Name;
          wp.lat = (decimal)air.Latitude;
          wp.lon = (decimal)air.Longitude;
          wp.ele = (decimal)air.Elevation;
          wp.eleSpecified = true;
          lst.Add(wp);
        }
        return lst;
      }
    }

    private void SelectAirports()
    {
      Clear();
      using (Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ForetrexToolbox.Airports.Airports.csv"))
      {
        if (stream == null)
        {
          string[] tmp = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }
        else
        { 
          using (StreamReader file = new StreamReader(stream))
          {
            string? line;
            while ((line = file.ReadLine()) != null)
            {
              string[] parts = line.Split(new char[] { ',' });
              if( _selectedAirports.Count > 0)
              {
                bool found = false;
                foreach (string airport in _selectedAirports)
                {
                  if (parts[2].Trim(new char[] { '\"' }).StartsWith(airport, StringComparison.OrdinalIgnoreCase))
                  {
                    found = true;
                    break;
                  }
                }
                if (!found)
                {
                  continue;
                }
              }
              if (_selectedContinents.Count > 0 )
              {
                bool found = false;
                foreach( string continent in _selectedContinents )
                {
                  if(parts[7].Trim(new char[] { '\"' }).Equals(continent, StringComparison.OrdinalIgnoreCase))
                  {
                    found = true; 
                    break;
                  }
                }
                if(!found)
                {
                  continue;
                }
              }
              try
              {
                string name = parts[3].Trim(new char[] { '\"' });
                string iata = parts[13].Trim(new char[] { '\"' });
                double lat = Double.Parse(parts[4], CultureInfo.InvariantCulture);
                double lon = Double.Parse(parts[5], CultureInfo.InvariantCulture);
                double ele = String.IsNullOrEmpty(parts[6]) ? 0 : Double.Parse(parts[6], CultureInfo.InvariantCulture);
                double distance = Math.Sqrt(Math.Pow(_lat - lat, 2) + Math.Pow(_lon - lon, 2));
                if (!string.IsNullOrEmpty(iata))
                {
                  Add(distance, new Airport(name, iata, lat, lon, ele));
                }
              }
              catch (FormatException)
              {
                Trace.WriteLine(line);
              }
              catch (ArgumentException)
              {
                Trace.WriteLine(line);
              }
            }
          }
        }
      }
    }
  }
}
