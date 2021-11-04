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
        public BinaryGridViewModel() : this(8, OutputType.PrivateKey)
        {
        }

        public BinaryGridViewModel(int len, OutputType ot)
        {
            if (len <= 0 || len % 8 != 0)
                throw new ArgumentException("Bit length must be divisible by 8.", nameof(len));

            Stream = new TernaryStream(len, 0, null, false, ot);
            Buttons = Stream.Next(len);
        }

        public override TernaryStream Stream { get; }
        public Ternary[] Buttons { get; private set; }
    }
}
