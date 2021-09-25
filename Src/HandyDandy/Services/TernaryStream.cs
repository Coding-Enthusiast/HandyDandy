// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using System.ComponentModel;

namespace HandyDandy.Services
{
    public class TernaryStream
    {
        public TernaryStream(int binarySize, int disabledCount, IChecksum cs, bool dynamicChecksum)
        {
            checksum = cs;
            Items = new Ternary[binarySize];

            for (int i = 0; i < binarySize; i++)
            {
                bool dis = i < binarySize - disabledCount;
                Items[i] = new Ternary(dis);
                if (dynamicChecksum)
                {
                    Items[i].PropertyChanged += Item_PropertyChanged;
                }
            }
        }


        private readonly IChecksum checksum;
        private int position;
        public Ternary[] Items { get; private set; }


        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Ternary.State) || sender is not Ternary temp || !temp.IsEnabled)
            {
                return;
            }

            byte[] ba = ToBytes();
            byte[] cs = checksum.Compute(ba);
            for (int i = 0; i < cs.Length; i++)
            {
                Items[^(cs.Length - i)].SetState(cs[i]);
            }
        }

        public byte[] ToBytes()
        {
            byte[] ba = new byte[Items.Length / 8];
            for (int i = 0, j = 0; i < ba.Length; i++, j += 8)
            {
                ba[i] = (byte)(Items[j].ToBit() << 7 |
                               Items[j + 1].ToBit() << 6 |
                               Items[j + 2].ToBit() << 5 |
                               Items[j + 3].ToBit() << 4 |
                               Items[j + 4].ToBit() << 3 |
                               Items[j + 5].ToBit() << 2 |
                               Items[j + 6].ToBit() << 1 |
                               Items[j + 7].ToBit() << 0);
            }
            return ba;
        }

        public Ternary[] Next(int count)
        {
            Ternary[] result = new Ternary[count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Items[i + position];
            }
            position += count;
            return result;
        }
    }
}
