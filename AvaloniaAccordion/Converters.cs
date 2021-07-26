/*
    AvaloniaAccordion - An accordion-like expandable control for Avalonia.
    Copyright (C) 2021  Giorgio Bianchini
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 3.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Avalonia.Data.Converters;
using System;
using System.Globalization;
using Avalonia.Controls;

namespace AvaloniaAccordion
{
    /// <summary>
    /// Converts <see cref="Accordion.ArrowPositions"/> into <see cref="bool"/>s.
    /// </summary>
    public class AccordionArrowPositionConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Accordion.ArrowPositions pos && parameter is string s && int.TryParse(s, out int i))
            {
                return (((int)pos) & i) != 0;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
            {
                return Accordion.ArrowPositions.Both;
            }
            else
            {
                return Accordion.ArrowPositions.Neither;
            }
        }
    }

    /// <summary>
    /// Converts the accordion status into an angle for the accordion arrows.
    /// </summary>
    public class AccordionAngleConverter : IValueConverter
    {
        internal bool ReturnLastValue { private get; set; } = false;

        private double LastValue = 0;

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ReturnLastValue)
            {
                return LastValue;
            }

            if (value is bool b && b)
            {
                LastValue = 180;
                return 180;
            }
            else
            {
                LastValue = 0;
                return 0;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d && d != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    /// <summary>
    /// Converts the accordion status into the height of the container.
    /// </summary>
    public class AccordionHeightConverter : IValueConverter
    {
        internal Grid ContentGrid { private get; set; } = null;
        internal bool ReturnLastValue { private get; set; } = false;

        private double LastValue = 0;

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ReturnLastValue)
            {
                return LastValue;
            }

            if (value is bool b && b)
            {
                if (ContentGrid != null)
                {
                    ContentGrid.Measure(new Avalonia.Size(ContentGrid.Bounds.Width, double.PositiveInfinity));

                    LastValue = ContentGrid.DesiredSize.Height;
                    return LastValue;
                }
                else
                {
                    LastValue = 0;
                    return 0;
                }
            }
            else
            {
                LastValue = 0;
                return 0;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d && d != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
