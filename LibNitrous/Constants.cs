﻿/* 
 * LibNitrous is a simple but powerful hacking library for files commonly used in the Nintendo DS ROM filesystem.
 * Copyright (C) 2010  Will Kirkby
 * Read LicenseInformation.txt for more information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNitrous
{
    static class Constants
    {
        public const uint GENERIC_HEADER_CONSTANT = 0xFFFE0001;
        public const ushort GENERIC_HEADER_SIZE = 0x10;
    }
}
