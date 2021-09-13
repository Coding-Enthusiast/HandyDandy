// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System.ComponentModel;

namespace HandyDandy.Models
{
    public enum InputType
    {
        BinaryGrid,
        GroupedBinary,
    }

    public enum OutputType
    {
        PrivateKey,
        Bip39Mnemonic,
        ElectrumMnemonic,
    }

    public enum MnemonicLength
    {
        [Description("12 words (128 bits)")]
        Twelve,
        [Description("15 words (160 bits)")]
        Fifteen,
        [Description("18 words (192 bits)")]
        Eighteen,
        [Description("21 words (224 bits)")]
        TwentyOne,
        [Description("24 words (256 bits)")]
        TwentyFour
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
