// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Data.Converters;
using HandyDandy.Models;
using System;
using System.Globalization;

namespace HandyDandy.MVVM.Converters
{
    public class TernaryToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TernaryState state)
            {
                return state switch
                {
                    TernaryState.Unset => "?",
                    TernaryState.Zero => "0",
                    TernaryState.One => "1",
                    _ => throw new NotImplementedException(),
                };
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
