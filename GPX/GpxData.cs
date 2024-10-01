using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ForetrexToolbox.GPX
{
  class GpxData
  {
    public enum GPXVersion
    {
      v10,
      v11,
    };
    protected Gpx10.gpx? _gpx10;
    protected Gpx11.gpxType? _gpx11;

    protected GpxData()
    {
    }

    #region public methods
    [RequiresUnreferencedCode("Calls ForetrexToolbox.GPX.GpxData.ToFile10(FileInfo)")]
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

    [RequiresUnreferencedCode("Calls ForetrexToolbox.GPX.Serializer<T>.SerializeStream(T, Stream)")]
    private void ToFile10(FileInfo fi)
    {
      Serializer<Gpx10.gpx> serializer10 = new Serializer<Gpx10.gpx>();
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
