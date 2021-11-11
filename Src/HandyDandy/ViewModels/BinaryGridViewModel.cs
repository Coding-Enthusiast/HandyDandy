// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using HandyDandy.Services;
using System;

namespace HandyDandy.ViewModels
{
    public class BinaryGridViewModel : GeneratorVM
    {
        public BinaryGridViewModel() : this(OutputType.Bip39Mnemonic, MnemonicLength.Twelve)
        {
        }

        public BinaryGridViewModel(OutputType ot, MnemonicLength mnLen)
        {
            int len = ot switch
            {
                OutputType.PrivateKey => 256,
                OutputType.Bip39Mnemonic => mnLen switch
                {
                    MnemonicLength.Twelve => 128,
                    MnemonicLength.Fifteen => 160,
                    MnemonicLength.Eighteen => 192,
                    MnemonicLength.TwentyOne => 224,
                    MnemonicLength.TwentyFour => 256,
                    _ => throw new NotImplementedException(),
                },
                // We only support the default 12-word Electrum mnemonics
                OutputType.ElectrumMnemonic => 136,
                _ => throw new NotImplementedException(),
            };

            Stream = new TernaryStream(len, ot);
            Buttons = Stream.Next(len);
        }

        public override TernaryStream Stream { get; }
        public Ternary[] Buttons { get; private set; }
    }
}
