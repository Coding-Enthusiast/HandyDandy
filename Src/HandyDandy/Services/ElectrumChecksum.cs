// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Hashing;
using System;
using System.Text;

namespace HandyDandy.Services
{
    public class ElectrumChecksum : IChecksum
    {
        private readonly HmacSha512 hmac = new();

        public byte[] Compute(Span<byte> data)
        {
            byte[] hash = hmac.ComputeHash(data, Encoding.UTF8.GetBytes("Seed version"));
            
            return hash;
        }
    }
}
