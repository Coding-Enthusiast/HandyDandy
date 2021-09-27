// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Hashing;
using System;

namespace HandyDandy.Services
{
    public class Bip39Checksum : IChecksum
    {
        public Bip39Checksum(int checksumSize)
        {
            this.checksumSize = checksumSize;
        }

        private readonly int checksumSize;
        private readonly Sha256 sha = new();

        public byte[] Compute(Span<byte> data)
        {
            byte[] hash = sha.ComputeHash(data.ToArray());
            byte CS = (byte)(hash[0] >> (8 - checksumSize));
            byte[] res = new byte[checksumSize];
            for (int i = 0; i < checksumSize; i++)
            {
                res[^(i + 1)] = (byte)((CS >> i) & 1);
            }
            return res;
        }
    }
}
