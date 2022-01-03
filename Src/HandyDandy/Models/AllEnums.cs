// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.ComponentModel;

namespace HandyDandy.Models
{
    [Flags]
    public enum InputType
    {
        BinaryGrid = 1 << 0,
        GroupedBinary = 1 << 1,
        Keyboard = 1 << 2
    }

    [Flags]
    public enum OutputType
    {
        PrivateKey = 1 << 8,
        Bip39Mnemonic = 1 << 9,
        ElectrumMnemonic = 1 << 10,
    }

    [Flags]
    public enum MnemonicLength
    {
        [Description("12 words (128 bits)")]
        Twelve = 1 << 16,
        [Description("15 words (160 bits)")]
        Fifteen = 1 << 17,
        [Description("18 words (192 bits)")]
        Eighteen = 1 << 18,
        [Description("21 words (224 bits)")]
        TwentyOne = 1 << 19,
        [Description("24 words (256 bits)")]
        TwentyFour = 1 << 20
    }

    public enum TernaryState
    {
        [Description("?")]
        Unset,
        [Description("0")]
        Zero,
        [Description("1")]
        One
    }
}
