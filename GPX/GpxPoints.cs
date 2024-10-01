using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ForetrexToolbox.GPX
{
  class GpxPoints : GpxData
  {
    protected GpxPoints()
    {
      // protected constructor, use FromFile() or FromList() instead
    }

    #region public methods
    [RequiresUnreferencedCode("Calls ForetrexToolbox.GPX.Serializer<T>.DeserializeStream(Stream)")]
    public static GpxPoints FromFile(FileInfo fi, GPXVersion ver)
    {
      GpxPoints gpxData = new GpxPoints();
      switch (ver)
      {
        case GPXVersion.v10:
          Serializer<Gpx10.gpx> serializer10 = new Serializer<Gpx10.gpx>();
          gpxData._gpx10 = serializer10.DeserializeStream(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
          break;
        case GPXVersion.v11:
          Serializer<Gpx11.gpxType> serializer11 = new Serializer<Gpx11.gpxType>();
          gpxData._gpx11 = serializer11.DeserializeStream(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
          break;
        default:
          throw new ArgumentException("Unsupported GPX Version");
      }
      return gpxData;
    }
    public static GpxPoints FromList( List<Gpx10.gpxWpt> lst)
    {
      GpxPoints gpxData = new GpxPoints();
      gpxData._gpx10 = new Gpx10.gpx();
      gpxData._gpx10.author = "Not Sure";
      gpxData._gpx10.wpt = lst.ToArray();
      return gpxData;
    }

    [RequiresUnreferencedCode("Calls ForetrexToolbox.GPX.GpxPoints.ModifyElevation(gpxWpt)")]
    public void ProcessWayPoints(int count, bool prefix, bool rating)
    {
      if (_gpx10 != null)
      {
        if (_gpx10.wpt.Length > count)
        {
          _gpx10.wpt = _gpx10.wpt.Where((val, idx) => (idx < count)).ToArray(); ;
        }
        if (prefix)
        {
          foreach (Gpx10.gpxWpt wpt in _gpx10.wpt)
          {
            if (wpt.name.StartsWith("GC", StringComparison.OrdinalIgnoreCase) && wpt.name.Length <= 7)
            {
              ModifyName(wpt);
            }
          }
        }
        if (rating)
        {
          foreach (Gpx10.gpxWpt wpt in _gpx10.wpt)
          {
            ModifyElevation(wpt);
          }
        }
        foreach (Gpx10.gpxWpt wpt in _gpx10.wpt)
        {
          wpt.Any = null;
        }
      }
    }

    #endregion

    #region private methods
    private static void ModifyName(Gpx10.gpxWpt wpt)
    {
      string? pre = null;
      switch (wpt.type)
      {
        case "Geocache|Traditional Cache":
          pre = "T-";
          break;
        case "Geocache|Multi-cache":
          pre = "M-";
          break;
        case "Geocache|Unknown Cache":
          pre = "U-";
          break;
        case "Geocache|Event Cache":
          pre = "E-";
          break;
        case "Geocache|Earthcache":
          pre = "V-";
          break;
        case "Geocache|Wherigo Cache":
          pre = "W-";
          break;
        case "Geocache|Letterbox Hybrid":
          pre = "L-";
          break;
        default:
          // do not change the name
          break;
      }
      if (!String.IsNullOrEmpty(pre))
      {
        wpt.name = pre + wpt.name.Substring(2);
      }
    }
    [RequiresUnreferencedCode("Calls ForetrexToolbox.GPX.Serializer<T>.Deserialize(String)")]
    private static void ModifyElevation(Gpx10.gpxWpt wpt)
    {
      try
      {
        Serializer<Gpx10.cache> serializer = new Serializer<Gpx10.cache>();
        Gpx10.cache groundspeak = serializer.Deserialize(wpt.Any[0].OuterXml)!;
        int d = (int)(float.Parse(groundspeak.difficulty, CultureInfo.InvariantCulture) * 10.0);
        int t = (int)(float.Parse(groundspeak.terrain, CultureInfo.InvariantCulture) * 10.0);
        wpt.ele = d * 100 + t;
        wpt.eleSpecified = true;
      }
      catch (Exception exc)
      {
        throw new Exception("Error: Could not read Groundspeak GPX extension", exc);
      }
    }
    #endregion
  }
}
