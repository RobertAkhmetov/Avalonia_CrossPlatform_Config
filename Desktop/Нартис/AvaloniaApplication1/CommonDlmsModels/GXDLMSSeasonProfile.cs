//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSSeasonProfile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSeasonProfile()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSeasonProfile(string name, GXDateTime start, string weekName)
        {
            if (name != null)
            {
                Name = ASCIIEncoding.ASCII.GetBytes(name);
            }
            Start = start;
            if (weekName != null)
            {
                WeekName = ASCIIEncoding.ASCII.GetBytes(weekName);
            }
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSeasonProfile(byte[] name, GXDateTime start, byte[] weekName)
        {
            Name = name;
            Start = start;
            WeekName = weekName;
        }

        /// <summary>
        /// Name of season profile.
        /// </summary>
        /// <remarks>
        /// Some manufacturers are using non ASCII names.
        /// </remarks>
        public byte[] Name
        {
            get;
            set;
        }

        /// <summary>
        /// Season Profile start time.
        /// </summary>
        public GXDateTime Start
        {
            get;
            set;
        }

        /// <summary>
        /// Week name of season profile.
        /// </summary>
        /// <remarks>
        /// Some manufacturers are using non ASCII names.
        /// </remarks>
        public byte[] WeekName
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Translator.ToHex(Name) + " " + Start.ToFormatString() + " " + Translator.ToHex(WeekName);
        }



    }

    public static class Translator
    {
        public static string ToHex(byte[] bytes, bool addSpace)
        {
            return ToHex(bytes, addSpace, 0, bytes == null ? 0 : bytes.Length);
        }

        public static string ToHex(byte[] bytes)
        {
            return ToHex(bytes, true);
        }

        public static string ToHex(byte[] bytes, bool addSpace, int index, int count)
        {
            if (bytes == null || bytes.Length == 0 || count == 0)
            {
                return string.Empty;
            }
            char[] str = new char[count * 3];
            int tmp;
            int len = 0;
            for (int pos = 0; pos != count; ++pos)
            {
                tmp = (bytes[index + pos] >> 4);
                str[len] = (char)(tmp > 9 ? tmp + 0x37 : tmp + 0x30);
                ++len;
                tmp = (bytes[index + pos] & 0x0F);
                str[len] = (char)(tmp > 9 ? tmp + 0x37 : tmp + 0x30);
                ++len;
                if (addSpace)
                {
                    str[len] = ' ';
                    ++len;
                }
            }
            if (addSpace)
            {
                --len;
            }
            return new string(str, 0, len);
        }
    }
}
