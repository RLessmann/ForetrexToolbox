using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForetrexToolbox.GPX
{
  internal class GpxRoute : GpxData
  {
    protected GpxRoute()
    {
      // protected constructor, use FromList() instead
    }

    public static GpxRoute FromList(string name, List<Gpx10.gpxRteRtept> lst)
    {
      GpxRoute gpxData = new GpxRoute();
      gpxData._gpx10 = new Gpx10.gpx();
      gpxData._gpx10.author = "Not Sure";
      gpxData._gpx10.rte = new Gpx10.gpxRte[1];
      gpxData._gpx10.rte[0] = new Gpx10.gpxRte();
      gpxData._gpx10.rte[0].name = name;
      gpxData._gpx10.rte[0].rtept = lst.ToArray();
      return gpxData;
    }
  }
}
