// Autarkysoft Tests
// Copyright (c) 2021 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Media;
using HandyDandy.Models;
using HandyDandy.MVVM.Converters;
using System.Collections.Generic;
using Xunit;

namespace Tests.MVVM.Converters
{
    public class TernaryToColorConverterTests
    {
        public static IEnumerable<object[]> GetCases()
        {
            yield return new object[] { TernaryState.Unset, Brushes.Transparent };
            yield return new object[] { TernaryState.One, Brushes.LightGreen };
            yield return new object[] { TernaryState.Zero, Brushes.LightGreen };
        }
        [Theory]
        [MemberData(nameof(GetCases))]
        public void ConvertTest(TernaryState value, ISolidColorBrush expected)
        {
            var conv = new TernaryToColorConverter();
            object actual = conv.Convert(value, null, null, null);
            Assert.IsAssignableFrom<ISolidColorBrush>(actual);
            Assert.Equal(expected, (ISolidColorBrush)actual);
        }
    }
}
