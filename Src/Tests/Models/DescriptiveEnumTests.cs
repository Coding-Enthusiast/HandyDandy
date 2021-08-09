﻿// Autarkysoft Tests
// Copyright (c) 2020 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using System.ComponentModel;
using Xunit;

namespace Tests.Models
{
    public class DescriptiveEnumTests
    {
        public enum MockEnum
        {
            [Description("Foo desc.")]
            Foo,
            [Description("Bar desc.")]
            Bar,
            // No desc.
            FooBar
        }

        [Theory]
        [InlineData(MockEnum.Foo, "Foo desc.")]
        [InlineData(MockEnum.Bar, "Bar desc.")]
        [InlineData(MockEnum.FooBar, "FooBar")]
        [InlineData((MockEnum)123, "123")]
        public void ConstructorTest(MockEnum val, string expected)
        {
            var item = new DescriptiveEnum<MockEnum>(val);

            Assert.Equal(expected, item.Description);
            Assert.Equal(val, item.Value);
        }
    }
}
