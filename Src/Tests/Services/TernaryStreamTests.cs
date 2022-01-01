// Autarkysoft Tests
// Copyright (c) 2021 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using HandyDandy.Services;
using Xunit;

namespace Tests.Services
{
    public class TernaryStreamTests
    {
        [Theory]
        [InlineData(OutputType.PrivateKey, (MnemonicLength)1000, 256, 32, 256)]
        [InlineData(OutputType.ElectrumMnemonic, (MnemonicLength)1000, 132, 16, 132)]
        [InlineData(OutputType.Bip39Mnemonic, MnemonicLength.Twelve, 128, 16, 132)]
        [InlineData(OutputType.Bip39Mnemonic, MnemonicLength.Fifteen, 160, 20, 165)]
        [InlineData(OutputType.Bip39Mnemonic, MnemonicLength.Eighteen, 192, 24, 198)]
        [InlineData(OutputType.Bip39Mnemonic, MnemonicLength.TwentyOne, 224, 28, 231)]
        [InlineData(OutputType.Bip39Mnemonic, MnemonicLength.TwentyFour, 256, 32, 264)]
        public void ConstructorTest(OutputType ot, MnemonicLength len, int dataBitSize, int dataByteSize, int totalBitSize)
        {
            var stream = new TernaryStream(ot, len);

            Assert.Equal(ot, stream.OutType);
            Assert.Equal(0, stream.ReadPosition);
            Assert.Equal(0, stream.SetPosition);
            Assert.Equal(dataBitSize, stream.DataBitSize);
            Assert.Equal(dataByteSize, stream.DataByteSize);
            Assert.Equal(totalBitSize, stream.TotalBitSize);
            Assert.Equal(0, stream.SetBitCount);
            Assert.False(stream.IsAllSet);
            Assert.Equal(totalBitSize, stream.Items.Length);

            for (int i = 0; i < dataBitSize; i++)
            {
                Assert.True(stream.Items[i].IsEnabled);
                Assert.Equal(TernaryState.Unset, stream.Items[i].State);
            }
            for (int i = dataBitSize; i < totalBitSize; i++)
            {
                Assert.False(stream.Items[i].IsEnabled);
                Assert.Equal(TernaryState.Unset, stream.Items[i].State);
            }
        }

        [Fact]
        public void SetNextTest()
        {
            var stream = new TernaryStream(4, 1);

            Assert.True(stream.Items[0].IsEnabled);
            Assert.Equal(TernaryState.Unset, stream.Items[0].State);
            Assert.True(stream.SetNext(true));
            Assert.Equal(TernaryState.One, stream.Items[0].State);
            Assert.False(stream.IsAllSet);
            Assert.Equal(1, stream.SetBitCount);

            Assert.True(stream.Items[1].IsEnabled);
            Assert.Equal(TernaryState.Unset, stream.Items[1].State);
            Assert.True(stream.SetNext(false));
            Assert.Equal(TernaryState.Zero, stream.Items[1].State);
            Assert.False(stream.IsAllSet);
            Assert.Equal(2, stream.SetBitCount);

            Assert.True(stream.Items[2].IsEnabled);
            Assert.Equal(TernaryState.Unset, stream.Items[2].State);
            Assert.False(stream.SetNext(false));
            Assert.Equal(TernaryState.Zero, stream.Items[2].State);
            Assert.True(stream.IsAllSet);
            Assert.Equal(3, stream.SetBitCount);

            Assert.False(stream.Items[3].IsEnabled);
        }
    }
}
