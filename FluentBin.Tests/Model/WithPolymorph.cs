using System;
using System.IO;

namespace FluentBin.Tests.Model
{
    class WithPolymorph : IWritable, IEquatable<WithPolymorph>
    {
        public IWritable Value { get; set; }
        public IWritable[] Values { get; set; }

        public void WriteTo(BinaryWriter bw)
        {
            Value.WriteTo(bw);
            foreach (var writable in Values)
            {
                writable.WriteTo(bw);
            }
        }

        public bool Equals(WithPolymorph other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value, other.Value) && Values.ElementsEqual(other.Values);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value != null ? Value.GetHashCode() : 0) * 397) ^ (Values != null ? Values.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
    /*
        
            [StructLayout(LayoutKind.Sequential)]
            struct Writable : IWritable
            {
                public Int16 Int16Value { get; set; }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(Int16Value);
                }
            }

            class WritableCollection : IList<IWritable>, IWritable
            {
                private readonly List<IWritable> _list = new List<IWritable>(); 

                public IEnumerator<IWritable> GetEnumerator()
                {
                    return _list.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                public void Add(IWritable item)
                {
                    _list.Add(item);
                }

                public void Clear()
                {
                    _list.Clear();
                }

                public bool Contains(IWritable item)
                {
                    return _list.Contains(item);
                }

                public void CopyTo(IWritable[] array, int arrayIndex)
                {
                    _list.CopyTo(array, arrayIndex);
                }

                public bool Remove(IWritable item)
                {
                    return _list.Remove(item);
                }

                public int Count { get { return _list.Count; } }
                public bool IsReadOnly { get { return ((ICollection<IWritable>)_list).IsReadOnly; } }
                public int IndexOf(IWritable item)
                {
                    return _list.IndexOf(item);
                }

                public void Insert(int index, IWritable item)
                {
                    _list.Insert(index, item);
                }

                public void RemoveAt(int index)
                {
                    _list.RemoveAt(index);
                }

                public IWritable this[int index]
                {
                    get { return _list[index]; }
                    set { _list[index] = value; }
                }

                public void WriteTo(BinaryWriter bw)
                {
                    foreach (var writable in _list)
                    {
                        writable.WriteTo(bw);
                    }
                }
            }
    */
}