// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.MVVM;

namespace HandyDandy.Models
{
    public class Ternary : InpcBase
    {
        public Ternary() : this(true)
        {
        }

        public Ternary(bool isEnabled)
        {
            _enabled = isEnabled;
        }


        private TernaryState _state;
        public TernaryState State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        private bool _enabled = true;
        public bool IsEnabled
        {
            get => _enabled;
            set => SetField(ref _enabled, value);
        }

        public void ChangeState() => State = State == TernaryState.One ? TernaryState.Zero : TernaryState.One;

        public void SetState(bool bit) => State = bit ? TernaryState.One : TernaryState.Zero;

        public void SetState(int bit) => SetState(bit == 1);

        public int ToBit() => State == TernaryState.One ? 1 : 0;
    }
}
