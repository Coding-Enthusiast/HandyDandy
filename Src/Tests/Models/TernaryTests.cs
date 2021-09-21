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

            Assert.Equal(TernaryState.Unset, t1.State);
            Assert.Equal(TernaryState.Unset, t2.State);
            Assert.Equal(TernaryState.Unset, t3.State);
        }

        [Fact]
        public void RaisePropertyChangedEventTest()
        {
            var t = new Ternary();

            Assert.PropertyChanged(t, nameof(t.IsEnabled), () => t.IsEnabled = false);
            Assert.PropertyChanged(t, nameof(t.State), () => t.State = TernaryState.One);
            
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
            t.State = initialState;
            t.ChangeState();
            Assert.Equal(t.State, expected);
        }

        [Theory]
        [InlineData(TernaryState.Unset, true, TernaryState.One)]
        [InlineData(TernaryState.Unset, false, TernaryState.Zero)]
        [InlineData(TernaryState.One, true, TernaryState.One)]
        [InlineData(TernaryState.One, false, TernaryState.Zero)]
        [InlineData(TernaryState.Zero, true, TernaryState.One)]
        [InlineData(TernaryState.Zero, false, TernaryState.Zero)]
        public void SetState_BoolTest(TernaryState initialState, bool b, TernaryState expected)
        {
            var t = new Ternary();
            t.State = initialState;
            t.SetState(b);
            Assert.Equal(t.State, expected);
        }

        [Theory]
        [InlineData(TernaryState.Unset, 1, TernaryState.One)]
        [InlineData(TernaryState.Unset, 0, TernaryState.Zero)]
        [InlineData(TernaryState.One, 1, TernaryState.One)]
        [InlineData(TernaryState.One, 0, TernaryState.Zero)]
        [InlineData(TernaryState.Zero, 1, TernaryState.One)]
        [InlineData(TernaryState.Zero, 0, TernaryState.Zero)]
        public void SetState_IntTest(TernaryState initialState, int i, TernaryState expected)
        {
            var t = new Ternary();
            t.State = initialState;
            t.SetState(i);
            Assert.Equal(t.State, expected);
        }

        [Theory]
        [InlineData(TernaryState.Unset, 0)]
        [InlineData(TernaryState.Zero, 0)]
        [InlineData(TernaryState.One, 1)]
        public void ToBitTest(TernaryState state, int expected)
        {
            var t = new Ternary();
            t.State = state;
            int actual = t.ToBit();
            Assert.Equal(expected, actual);
        }
    }
}
