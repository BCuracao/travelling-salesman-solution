using System.Globalization;
using System.Xml.Linq;

//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
//::  This class represents a city object that holds a name         :::
//::  and lat / lon coordinates                                     :::
//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
internal class CityFactory
{

    public string location { get; private set; }
    public double lat { get; private set; }
    public double lon { get; private set; }

    public CityFactory(XElement el)
    {
        location = el.Attribute("Standort").Value.ToString();
        lat = double.Parse(el.Attribute("Breitengrad").Value, CultureInfo.InvariantCulture);
        lon = double.Parse(el.Attribute("Laengengrad").Value, CultureInfo.InvariantCulture);
    }
}