// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

namespace HandyDandy.Services
{
    public interface IChecksum
    {
        byte[] Compute(byte[] data);
    }
}
