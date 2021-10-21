﻿using NuGet.Versioning;
using privatenupkgserver.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace privatenupkgserver.Models.Nuget
{
    [TypeConverter(typeof(NuGetVersionStringTypeConverter))]
    public class NuGetVersionString : NuGetVersion
    {
        static NuGetVersionString()
        {
            Default = new NuGetVersionString(0, 0, 0);
        }

        public NuGetVersionString(string version) : base(version)
        {
        }

        public NuGetVersionString(NuGetVersion version) : base(version)
        {
        }

        public NuGetVersionString(Version version, string releaseLabel = null, string metadata = null) : base(version, releaseLabel, metadata)
        {
        }

        public NuGetVersionString(int major, int minor, int patch) : base(major, minor, patch)
        {
        }

        public NuGetVersionString(int major, int minor, int patch, string releaseLabel) : base(major, minor, patch, releaseLabel)
        {
        }

        public NuGetVersionString(int major, int minor, int patch, string releaseLabel, string metadata) : base(major, minor, patch, releaseLabel, metadata)
        {
        }

        public NuGetVersionString(int major, int minor, int patch, IEnumerable<string> releaseLabels, string metadata) : base(major, minor, patch, releaseLabels, metadata)
        {
        }

        public NuGetVersionString(int major, int minor, int patch, int revision) : base(major, minor, patch, revision)
        {
        }

        public NuGetVersionString(int major, int minor, int patch, int revision, string releaseLabel, string metadata) : base(major, minor, patch, revision, releaseLabel, metadata)
        {
        }

        public NuGetVersionString(int major, int minor, int patch, int revision, IEnumerable<string> releaseLabels, string metadata) : base(major, minor, patch, revision, releaseLabels, metadata)
        {
        }

        public NuGetVersionString(Version version, IEnumerable<string> releaseLabels, string metadata, string originalVersion) : base(version, releaseLabels, metadata, originalVersion)
        {
        }

        public static new NuGetVersionString Parse(string version)
        {
            return new NuGetVersionString(NuGetVersion.Parse(version));
        }

        public static explicit operator NuGetVersionString(string version)
        {
            return Parse(version);
        }

        public static bool operator ==(NuGetVersionString left, string right)
            => Equals(left, right);

        public static bool operator ==(string left, NuGetVersionString right)
            => Equals(right, left);

        public static bool operator !=(NuGetVersionString left, string right)
            => !Equals(left, right);

        public static bool operator !=(string left, NuGetVersionString right)
            => !Equals(right, left);

        public static bool operator >(NuGetVersionString left, string right)
            => left > (NuGetVersionString)right;

        public static bool operator >(string left, NuGetVersionString right)
            => (NuGetVersionString)left > right;

        public static bool operator >=(string left, NuGetVersionString right)
            => (NuGetVersionString)left >= right;

        public static bool operator >=(NuGetVersionString left, string right)
            => left >= (NuGetVersionString)right;

        public static bool operator <(NuGetVersionString left, string right)
            => left < (NuGetVersionString)right;

        public static bool operator <=(NuGetVersionString left, string right)
            => left <= (NuGetVersionString)right;

        public static bool operator <(string left, NuGetVersionString right)
            => (NuGetVersionString)left < right;

        public static bool operator <=(string left, NuGetVersionString right)
            => (NuGetVersionString)left <= right;

        public override bool Equals(object other)
            => Equals(this, other);

        public bool Equals(string other)
            => Equals((NuGetVersionString)other);

        public bool Equals(NuGetVersionString other)
            => base.Equals(other);

        public static bool Equals(NuGetVersionString left, object right)
        {
            if (right is NuGetVersionString sv)
            {
                return left.Equals(sv);
            }
            if (right is SemanticVersion v)
            {
                return left.Equals(v);
            }
            if (right is string s)
            {
                return left.Equals((NuGetVersionString)s);
            }
            return ReferenceEquals(left, right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static readonly NuGetVersionString Default;
    }
}