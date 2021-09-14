﻿// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.MVVM;

namespace HandyDandy.Models
{
    public class Ternary : InpcBase
    {
        private DescriptiveEnum<TernaryState> _state = new(TernaryState.Unset);
        public DescriptiveEnum<TernaryState> State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        public void ChangeState()
        {
            State = State.Value == TernaryState.One ?
                new DescriptiveEnum<TernaryState>(TernaryState.Zero) :
                new DescriptiveEnum<TernaryState>(TernaryState.One);
        }

        public int ToBit() => State.Value == TernaryState.One ? 1 : 0;
    }
}
