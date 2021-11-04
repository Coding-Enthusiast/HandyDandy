// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Services;

namespace HandyDandy.ViewModels
{
    public abstract class GeneratorVM : ViewModelBase
    {
        public abstract TernaryStream Stream { get; }
    }
}
