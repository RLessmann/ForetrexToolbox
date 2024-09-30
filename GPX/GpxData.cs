using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ForetrexToolbox
{
  class GpxData
  {
    public enum GPXVersion
    {
      v10,
      v11,
    };
    private Gpx10.gpx? _gpx10;
    private Gpx11.gpxType? _gpx11;

    protected GpxData()
    {
      // protected constructor, use FromFile() instead
    }

    #region public methods
    [RequiresUnreferencedCode("Calls ForetrexToolbox.GpxSerializer<T>.DeserializeStream(Stream)")]
    public static GpxData FromFile(FileInfo fi, GPXVersion ver)
    {
      GpxData gpxData = new GpxData();
      switch (ver)
      {
        case GPXVersion.v10:
          GpxSerializer<Gpx10.gpx> serializer10 = new GpxSerializer<Gpx10.gpx>();
          gpxData._gpx10 = serializer10.DeserializeStream(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
          break;
        case GPXVersion.v11:
          GpxSerializer<Gpx11.gpxType> serializer11 = new GpxSerializer<Gpx11.gpxType>();
          gpxData._gpx11 = serializer11.DeserializeStream(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
          break;
        default:
          throw new ArgumentException("Unsupported GPX Version");
      }
      return gpxData;
    }
    public static GpxData FromList( List<Gpx10.gpxWpt> lst)
    {
      GpxData gpxData = new GpxData();
      gpxData._gpx10 = new Gpx10.gpx();
      gpxData._gpx10.author = "ralph.lessmann@googlemail.com";
      gpxData._gpx10.wpt = lst.ToArray();
      return gpxData;
    }

    [RequiresUnreferencedCode("Calls ForetrexToolbox.GpxData.ModifyElevation(gpxWpt)")]
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

    [RequiresUnreferencedCode("Calls ForetrexToolbox.GpxData.ToFile10(FileInfo)")]
    public void ToFile(FileInfo? fi)
    {
      if(fi == null)
      {
        throw new ArgumentNullException("No output file specified");
      }
      if (_gpx10 != null)
      {
        ToFile10(fi);
      }
      else if (_gpx11 != null)
      {
        ToFile11(fi);
      }
      else
      {
        throw new ArgumentNullException("No data to save");
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
    [RequiresUnreferencedCode("Calls ForetrexToolbox.GpxSerializer<T>.Deserialize(String)")]
    private static void ModifyElevation(Gpx10.gpxWpt wpt)
    {
      try
      {
        GpxSerializer<Gpx10.cache> serializer = new GpxSerializer<Gpx10.cache>();
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

    [RequiresUnreferencedCode("Calls ForetrexToolbox.GpxSerializer<T>.SerializeStream(T, Stream)")]
    private void ToFile10(FileInfo fi)
    {
      GpxSerializer<Gpx10.gpx> serializer10 = new GpxSerializer<Gpx10.gpx>();
      serializer10.SerializeStream(_gpx10!, fi.Open(FileMode.Create, FileAccess.Write, FileShare.Write));
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fi")]
    private static void ToFile11(FileInfo fi)
    {
      throw new NotImplementedException("GPX 1.1 is currently not supported");
    }
    #endregion
  }
}
