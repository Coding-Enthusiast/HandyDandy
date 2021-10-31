// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Hashing;
using System;

namespace HandyDandy.Services
{
    public class WifChecksum : IChecksum
    {
        private readonly Sha256 hash = new();

        public byte[] Compute(Span<byte> data)
        {
            return hash.ComputeChecksum(data);
        }
    }
}
