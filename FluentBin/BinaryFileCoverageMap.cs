using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBin
{
    public class BinaryFileCoverageMap
    {
        private readonly List<Tuple<long, int>> _readBytesRequests = new List<Tuple<long, int>>();

        public void AddReadRequest(long position, int count)
        {
            _readBytesRequests.Add(new Tuple<long, int>(position, count));
        }

        public BinaryFileCoverageMapEntry[] Entries
        {
            get
            {
                var entries = new List<BinaryFileCoverageMapEntry>();
                long currentPos = 0;
                int currentCount = 0;
                foreach (var tuple in _readBytesRequests.OrderBy(r => r.Item1))
                {
                    if (tuple.Item1 >= currentPos
                        && tuple.Item1 <= currentPos + currentCount)
                    {
                        currentCount = (int)(Math.Max(currentPos + currentCount, tuple.Item1 + tuple.Item2) - currentPos);
                    }
                    else
                    {
                        if (currentCount > 0)
                        {
                            entries.Add(new BinaryFileCoverageMapEntry
                            {
                                Position = currentPos,
                                Length = currentCount
                            });
                        }
                        currentPos = tuple.Item1;
                        currentCount = tuple.Item2;
                    }
                }

                return entries.ToArray();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public long CoverageTotal { get { return Entries.Sum(c => c.Length); } }
    }
}
