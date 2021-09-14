// Autarkysoft Tests
// Copyright (c) 2021 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using Xunit;

namespace Tests.Models
{
    public class TernaryTests
    {
        [Theory]
        [InlineData(TernaryState.Unset, TernaryState.One)]
        [InlineData(TernaryState.Zero, TernaryState.One)]
        [InlineData(TernaryState.One, TernaryState.Zero)]
        public void ChangeStateTest(TernaryState initialState, TernaryState expected)
        {
            var t = new Ternary();
            t.State.Value = initialState;
            t.ChangeState();
            Assert.Equal(t.State.Value, expected);
        }

        [Theory]
        [InlineData(TernaryState.Unset, 0)]
        [InlineData(TernaryState.Zero, 0)]
        [InlineData(TernaryState.One, 1)]
        public void ToBitTest(TernaryState state, int expected)
        {
            var t = new Ternary();
            t.State.Value = state;
            int actual = t.ToBit();
            Assert.Equal(expected, actual);
        }
    }
}
