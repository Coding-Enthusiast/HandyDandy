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
        [Fact]
        public void ConstructorTest()
        {
            var t1 = new Ternary();
            var t2 = new Ternary(true);
            var t3 = new Ternary(false);

            Assert.True(t1.IsEnabled);
            Assert.True(t2.IsEnabled);
            Assert.False(t3.IsEnabled);

            Assert.Equal(TernaryState.Unset, t1.State.Value);
            Assert.Equal(TernaryState.Unset, t2.State.Value);
            Assert.Equal(TernaryState.Unset, t3.State.Value);
        }

        [Fact]
        public void RaisePropertyChangedEventTest()
        {
            var t = new Ternary();

            Assert.PropertyChanged(t, nameof(t.IsEnabled), () => t.IsEnabled = false);
            Assert.PropertyChanged(t, nameof(t.State), () => t.State = new DescriptiveEnum<TernaryState>(TernaryState.One));
            
            Assert.False(t.IsEnabled);
            Assert.PropertyChanged(t, nameof(t.State), () => t.ChangeState());

            // State change should raise the event regardless of button enabled state
            Assert.PropertyChanged(t, nameof(t.IsEnabled), () => t.IsEnabled = true);
            Assert.True(t.IsEnabled);
            Assert.PropertyChanged(t, nameof(t.State), () => t.ChangeState());
        }

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
