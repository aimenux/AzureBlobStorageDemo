﻿using System.Collections.Generic;

namespace Lib
{
    public interface IBlobModel
    {
        string Name { get; }
        object Content { get; }
        Dictionary<string, string> Metadata { get; }
    }
}