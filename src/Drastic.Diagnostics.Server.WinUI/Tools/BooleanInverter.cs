// <copyright file="BooleanInverter.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.UI.Xaml.Data;

namespace Drastic.Diagnostics.Server.WinUI.Tools
{
    public class BooleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
