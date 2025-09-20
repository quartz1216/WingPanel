using System.Collections;
using System.Collections.Generic;

namespace WingPanel.Core.Utilities;

public sealed class MruList<T> : IEnumerable<T>
{
    private readonly LinkedList<T> _items = new();
    private readonly IEqualityComparer<T> _comparer;

    public MruList()
        : this(EqualityComparer<T>.Default)
    {
    }

    public MruList(IEqualityComparer<T> comparer)
    {
        _comparer = comparer;
    }

    public int Count => _items.Count;

    public void Touch(T item)
    {
        var node = FindNode(item);
        if (node is not null)
        {
            node.Value = item;
            _items.Remove(node);
            _items.AddFirst(node);
        }
        else
        {
            _items.AddFirst(item);
        }
    }

    public IReadOnlyList<T> ToSnapshot() => new List<T>(_items);

    private LinkedListNode<T>? FindNode(T item)
    {
        for (var node = _items.First; node is not null; node = node.Next)
        {
            if (_comparer.Equals(node.Value, item))
            {
                return node;
            }
        }

        return null;
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
