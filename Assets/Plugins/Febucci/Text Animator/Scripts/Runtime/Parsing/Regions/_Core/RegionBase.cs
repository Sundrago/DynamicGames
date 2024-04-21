using System;
using UnityEngine;

namespace Febucci.UI.Core.Parsing
{
    public abstract class RegionBase
    {
        public readonly string tagId;
        public TagRange[] ranges;

        public RegionBase(string tagId)
        {
            this.tagId = tagId;
            ranges = Array.Empty<TagRange>();
        }

        public RegionBase(string tagId, params TagRange[] ranges)
        {
            this.tagId = tagId;
            this.ranges = ranges;
        }

        public RegionBase(string tagId, params Vector2Int[] ranges)
        {
            this.tagId = tagId;
            var len = tagId.Length + 2; //plus opening/closing symbols
            this.ranges = new TagRange[ranges.Length];
            for (var i = 0; i < this.ranges.Length; i++)
                this.ranges[i] = new TagRange(ranges[i]);
        }
    }
}