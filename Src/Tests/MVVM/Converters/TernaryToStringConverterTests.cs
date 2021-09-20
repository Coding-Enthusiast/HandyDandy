// Autarkysoft Tests
// Copyright (c) 2021 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using HandyDandy.MVVM.Converters;
using Xunit;

namespace Tests.MVVM.Converters
{
    public class TernaryToStringConverterTests
    {
        [Theory]
        [InlineData(TernaryState.Unset, "?")]
        [InlineData(TernaryState.One, "1")]
        [InlineData(TernaryState.Zero, "0")]
        public void ConvertTest(TernaryState value, string expected)
        {
            var conv = new TernaryToStringConverter();
            object actual = conv.Convert(value, null, null, null);
            Assert.IsType<string>(actual);
            Assert.Equal(expected, (string)actual);
        }
    }
}
