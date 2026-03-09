// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerExtensions.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Extension methods for converting OxyPlot data types to and from string and XML representations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Wpf.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Xml.Linq;
    using OxyPlot;

    /// <summary>
    /// Provides extension methods for converting OxyPlot data types (DataPoint, ScreenVector, ScreenPoint)
    /// and WPF Vector to and from string and XML representations used in plot settings serialization.
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// Converts a <see cref="ScreenVector"/> to a formatted string representation.
        /// </summary>
        /// <param name="sv">The screen vector to convert.</param>
        /// <returns>A string in the format "X, Y" using invariant culture.</returns>
        public static string ToPrettyText(this ScreenVector sv)
        {
            return sv.X.ToString("G17", CultureInfo.InvariantCulture) + ", " + sv.Y.ToString("G17", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a formatted string to create a <see cref="ScreenVector"/>.
        /// </summary>
        /// <param name="svString">The string in "X, Y" format.</param>
        /// <returns>A <see cref="ScreenVector"/> parsed from the string, or default if parsing fails.</returns>
        public static ScreenVector FromPrettyVectorText(this string svString)
        {
            var svStringSplit = svString.Split(new[] { ", " }, StringSplitOptions.None);
            if (svStringSplit.Length != 2) return default(ScreenVector);

            double x, y;
            if (double.TryParse(svStringSplit[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) == false) return default(ScreenVector);
            if (double.TryParse(svStringSplit[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y) == false) return default(ScreenVector);
            return new ScreenVector(x, y);
        }

        /// <summary>
        /// Converts a <see cref="DataPoint"/> to a formatted string representation.
        /// </summary>
        /// <param name="dp">The data point to convert.</param>
        /// <returns>A string in the format "X, Y" using invariant culture.</returns>
        public static string ToPrettyText(this DataPoint dp)
        {
            return dp.X.ToString("G17", CultureInfo.InvariantCulture) + ", " + dp.Y.ToString("G17", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a formatted string to create a <see cref="DataPoint"/>.
        /// </summary>
        /// <param name="dpString">The string in "X, Y" format.</param>
        /// <returns>A <see cref="DataPoint"/> parsed from the string, or <see cref="DataPoint.Undefined"/> if parsing fails.</returns>
        public static DataPoint FromPrettyDataText(this string dpString)
        {
            var dpStringSplit = dpString.Split(new[] { ", " }, StringSplitOptions.None);
            if (dpStringSplit.Length != 2) return DataPoint.Undefined;

            double x, y;
            if (double.TryParse(dpStringSplit[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) == false) return DataPoint.Undefined;
            if (double.TryParse(dpStringSplit[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y) == false) return DataPoint.Undefined;
            return new DataPoint(x, y);
        }

        /// <summary>
        /// Converts a <see cref="DataPoint"/> to an XML element for serialization.
        /// </summary>
        /// <param name="dp">The data point to convert.</param>
        /// <returns>An <see cref="XElement"/> representing the data point with X and Y attributes.</returns>
        public static XElement ToXElement(this DataPoint dp)
        {
            var dpElement = new XElement("DataPoint");
            dpElement.SetAttributeValue("X", dp.X.ToString("G17", CultureInfo.InvariantCulture));
            dpElement.SetAttributeValue("Y", dp.Y.ToString("G17", CultureInfo.InvariantCulture));

            return dpElement;
        }

        /// <summary>
        /// Parses an XML element to create a <see cref="DataPoint"/>.
        /// </summary>
        /// <param name="dpElement">The XML element containing X and Y attributes.</param>
        /// <returns>A <see cref="DataPoint"/> parsed from the element, or <see cref="DataPoint.Undefined"/> if parsing fails.</returns>
        public static DataPoint PointFromXElement(this XElement dpElement)
        {
            if (dpElement.Name != "DataPoint") return DataPoint.Undefined;
            var xAttr = dpElement.Attribute("X");
            var yAttr = dpElement.Attribute("Y");
            if (xAttr == null) return DataPoint.Undefined;
            if (yAttr == null) return DataPoint.Undefined;

            double x, y;
            if (double.TryParse(xAttr.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out x) == false) return DataPoint.Undefined;
            if (double.TryParse(yAttr.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out y) == false) return DataPoint.Undefined;
            return new DataPoint(x, y);
        }

        /// <summary>
        /// Converts a <see cref="ScreenPoint"/> to a formatted string representation.
        /// </summary>
        /// <param name="sp">The screen point to convert.</param>
        /// <returns>A string in the format "X, Y" using invariant culture.</returns>
        public static string ToPrettyText(this ScreenPoint sp)
        {
            return sp.X.ToString("G17", CultureInfo.InvariantCulture) + ", " + sp.Y.ToString("G17", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a formatted string to create a <see cref="ScreenPoint"/>.
        /// </summary>
        /// <param name="spString">The string in "X, Y" format.</param>
        /// <returns>A <see cref="ScreenPoint"/> parsed from the string, or <see cref="ScreenPoint.Undefined"/> if parsing fails.</returns>
        public static ScreenPoint FromPrettyScreenText(this string spString)
        {
            var spStringSplit = spString.Split(new[] { ", " }, StringSplitOptions.None);
            if (spStringSplit.Length != 2) return ScreenPoint.Undefined;

            double x, y;
            if (double.TryParse(spStringSplit[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) == false) return ScreenPoint.Undefined;
            if (double.TryParse(spStringSplit[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y) == false) return ScreenPoint.Undefined;
            return new ScreenPoint(x, y);
        }

        /// <summary>
        /// Converts a <see cref="Vector"/> to a formatted string representation.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>A string in the format "X, Y" using invariant culture.</returns>
        public static string ToPrettyText(this Vector v)
        {
            return v.X.ToString("G17", CultureInfo.InvariantCulture) + ", " + v.Y.ToString("G17", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a formatted string to create a <see cref="Vector"/>.
        /// </summary>
        /// <param name="vString">The string in "X, Y" format.</param>
        /// <returns>A <see cref="Vector"/> parsed from the string, or default if parsing fails.</returns>
        public static Vector FromPrettyVectorString(this string vString)
        {
            var vStringSplit = vString.Split(new[] { ", " }, StringSplitOptions.None);
            if (vStringSplit.Length != 2) return default(Vector);

            double x, y;
            if (double.TryParse(vStringSplit[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) == false) return default(Vector);
            if (double.TryParse(vStringSplit[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y) == false) return default(Vector);
            return new Vector(x, y);
        }

        /// <summary>
        /// Converts a list of <see cref="DataPoint"/> values to an XML element for serialization.
        /// </summary>
        /// <param name="dataPoints">The list of data points to convert.</param>
        /// <param name="name">The name for the parent XML element.</param>
        /// <returns>An <see cref="XElement"/> containing all data points as child elements.</returns>
        public static XElement ToXElement(this IList<DataPoint> dataPoints, string name)
        {
            var el = new XElement(name);

            foreach (var dp in dataPoints)
            {
                el.Add(dp.ToXElement());
            }

            return el;
        }

        /// <summary>
        /// Parses an XML element to create a list of <see cref="DataPoint"/> values.
        /// </summary>
        /// <param name="dpelements">The XML element containing DataPoint child elements.</param>
        /// <returns>A list of <see cref="DataPoint"/> values parsed from the element.</returns>
        /// <remarks>
        /// Supports both "DataPoints" (legacy) and "Points" element names for backward compatibility.
        /// </remarks>
        public static IList<DataPoint> PointsFromXElement(this XElement dpelements)
        {
            if (dpelements.Name != "DataPoints" && dpelements.Name != "Points") return new List<DataPoint>();

            var dpList = new List<DataPoint>();
            if (dpelements.Name == "DataPoints") // Backwards Compatibility
            {
                foreach (var dp in dpelements.Elements("DataPoint"))
                {
                    dpList.Add(dp.PointFromXElement());
                }
            }
            else if (dpelements.Name == "Points")
            {
                foreach (var dp in dpelements.Elements("DataPoint"))
                {
                    dpList.Add(dp.PointFromXElement());
                }
            }

            return dpList;
        }
    }
}
