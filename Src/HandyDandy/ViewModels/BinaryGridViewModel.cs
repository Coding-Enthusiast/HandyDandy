// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using HandyDandy.Services;

namespace HandyDandy.ViewModels
{
    public class BinaryGridViewModel : GeneratorVM
    {
        public BinaryGridViewModel() : this(OutputType.Bip39Mnemonic, MnemonicLength.Twelve)
        {
        }

        public BinaryGridViewModel(OutputType ot, MnemonicLength mnLen)
        {
            Stream = new TernaryStream(ot, mnLen);
            Buttons = Stream.Next(Stream.TotalBitSize);
        }

        public override TernaryStream Stream { get; }
        public Ternary[] Buttons { get; private set; }
    }
}
