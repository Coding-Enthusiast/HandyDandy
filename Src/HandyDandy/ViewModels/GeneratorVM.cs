// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Asymmetric.KeyPairs;
using Autarkysoft.Bitcoin.ImprovementProposals;
using HandyDandy.Models;
using HandyDandy.MVVM;
using HandyDandy.Services;
using System;
using System.ComponentModel;

namespace HandyDandy.ViewModels
{
    public abstract class GeneratorVM : ViewModelBase
    {
        protected void Stream_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(Result);
        }

        public abstract TernaryStream Stream { get; }

        public OutputType OutputType { get; protected set; }

        [DependsOnProperty(nameof(TernaryStream.IsAllSet))]
        public string Result
        {
            get
            {
                if (!Stream.IsAllSet)
                {
                    return "Not all bits are set. The result is still weak and can not be copied.";
                }

                byte[] bytes = Stream.ToBytes();
                if (OutputType == OutputType.PrivateKey)
                {
                    try
                    {
                        using PrivateKey temp = new(bytes);
                        return temp.ToWif(true);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        return $"The current bit stream is creating an out of range key.{Environment.NewLine}" +
                               $"{ex.Message}";
                    }
                    catch (Exception ex)
                    {
                        return $"An error occurred while converting the bit stream to a private key.{Environment.NewLine}" +
                               $"Error message: {ex.Message}";
                    }
                }
                else if (OutputType == OutputType.Bip39Mnemonic)
                {
                    try
                    {
                        using BIP0039 temp = new(bytes);
                        return temp.ToMnemonic();
                    }
                    catch (Exception ex)
                    {
                        return $"An error occurred while converting the bit stream to a BIP-39 mnemonic.{Environment.NewLine}" +
                               $"Error message: {ex.Message}";
                    }
                }
                else if (OutputType == OutputType.ElectrumMnemonic)
                {
                    try
                    {
                        using ElectrumMnemonic temp = new(bytes, ElectrumMnemonic.MnemonicType.SegWit);
                        return temp.ToMnemonic();
                    }
                    catch (Exception ex)
                    {
                        return $"An error occurred while converting the bit stream to an Electrum mnemonic.{Environment.NewLine}" +
                               $"Error message: {ex.Message}";
                    }
                }
                else
                {
                    return "Undefined.";
                }
            }
        }
    }
}
