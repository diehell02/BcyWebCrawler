// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcyWebCrawler.Core
{
    public class Image
    {
        public long ImageId { get; internal set; }

        public string FileName { get; internal set; } = string.Empty;

        public long PostId { get; internal set; }

        public Post? Post { get; internal set; }
    }
}
