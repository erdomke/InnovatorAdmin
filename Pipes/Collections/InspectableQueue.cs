using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pipes.Collections
{
  /// <summary>Represents a first-in, first-out collection of objects.</summary>
  /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
  /// <filterpriority>1</filterpriority>
  [Serializable]
  public class InspectableQueue<T> : IEnumerable<T>, ICollection, IEnumerable
  {
    /// <summary>Enumerates the elements of a <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
    [Serializable]
    public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
    {
      private InspectableQueue<T> _q;
      private int _index;
      private int _version;
      private T _currentElement;
      /// <summary>Gets the element at the current position of the enumerator.</summary>
      /// <returns>The element in the <see cref="T:System.Collections.Generic.Queue`1" /> at the current position of the enumerator.</returns>
      /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
      public T Current
      {
        get
        {
          if (this._index < 0)
          {
            if (this._index == -1)
            {
              throw new InvalidOperationException();
            }
            else
            {
              throw new InvalidOperationException();
            }
          }
          return this._currentElement;
        }
      }
      /// <summary>Gets the element at the current position of the enumerator.</summary>
      /// <returns>The element in the collection at the current position of the enumerator.</returns>
      /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
      object IEnumerator.Current
      {
        get
        {
          if (this._index < 0)
          {
            if (this._index == -1)
            {
              throw new InvalidOperationException();
            }
            else
            {
              throw new InvalidOperationException();
            }
          }
          return this._currentElement;
        }
      }
      internal Enumerator(InspectableQueue<T> q)
      {
        this._q = q;
        this._version = this._q._version;
        this._index = -1;
        this._currentElement = default(T);
      }
      /// <summary>Releases all resources used by the <see cref="T:System.Collections.Generic.Queue`1.Enumerator" />.</summary>
      public void Dispose()
      {
        this._index = -2;
        this._currentElement = default(T);
      }
      /// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
      /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
      /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
      public bool MoveNext()
      {
        if (this._version != this._q._version) throw new InvalidOperationException();
        if (this._index == -2)
        {
          return false;
        }
        this._index++;
        if (this._index == this._q._size)
        {
          this._index = -2;
          this._currentElement = default(T);
          return false;
        }
        this._currentElement = this._q.GetElement(this._index);
        return true;
      }
      /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
      /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
      void IEnumerator.Reset()
      {
        if (this._version != this._q._version) throw new InvalidOperationException();
        this._index = -1;
        this._currentElement = default(T);
      }
    }
    private const int _MinimumGrow = 4;
    private const int _ShrinkThreshold = 32;
    private const int _GrowFactor = 200;
    private const int _DefaultCapacity = 4;
    private T[] _array;
    private int _head;
    private int _tail;
    private int _size;
    private int _version;
    [NonSerialized]
    private object _syncRoot;
    private static T[] _emptyArray = new T[0];
    /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
    /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
    public int Count
    {
      get
      {
        return this._size;
      }
    }
    /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
    /// <returns>true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.  In the default implementation of <see cref="T:System.Collections.Generic.Queue`1" />, this property always returns false.</returns>
    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }
    /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
    /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.  In the default implementation of <see cref="T:System.Collections.Generic.Queue`1" />, this property always returns the current instance.</returns>
    object ICollection.SyncRoot
    {
      get
      {
        if (this._syncRoot == null)
        {
          Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
        }
        return this._syncRoot;
      }
    }
    /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.Queue`1" /> class that is empty and has the default initial capacity.</summary>
    public InspectableQueue()
    {
      this._array = InspectableQueue<T>._emptyArray;
    }
    /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.Queue`1" /> class that is empty and has the specified initial capacity.</summary>
    /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Queue`1" /> can contain.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="capacity" /> is less than zero.</exception>
    public InspectableQueue(int capacity)
    {
      if (capacity < 0)
      {
        throw new ArgumentOutOfRangeException("capacity", "Non negative number required.");
      }
      this._array = new T[capacity];
      this._head = 0;
      this._tail = 0;
      this._size = 0;
    }
    /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.Queue`1" /> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.</summary>
    /// <param name="collection">The collection whose elements are copied to the new <see cref="T:System.Collections.Generic.Queue`1" />.</param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="collection" /> is null.</exception>
    public InspectableQueue(IEnumerable<T> collection)
    {
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }
      this._array = new T[4];
      this._size = 0;
      this._version = 0;
      using (IEnumerator<T> enumerator = collection.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          this.Enqueue(enumerator.Current);
        }
      }
    }
    /// <summary>Removes all objects from the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
    /// <filterpriority>1</filterpriority>
    public void Clear()
    {
      if (this._head < this._tail)
      {
        Array.Clear(this._array, this._head, this._size);
      }
      else
      {
        Array.Clear(this._array, this._head, this._array.Length - this._head);
        Array.Clear(this._array, 0, this._tail);
      }
      this._head = 0;
      this._tail = 0;
      this._size = 0;
      this._version++;
    }
    /// <summary>Copies the <see cref="T:System.Collections.Generic.Queue`1" /> elements to an existing one-dimensional <see cref="T:System.Array" />, starting at the specified array index.</summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.Queue`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="array" /> is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="arrayIndex" /> is less than zero.</exception>
    /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.Queue`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
      if (array == null) throw new ArgumentNullException("array");
      if (arrayIndex < 0 || arrayIndex > array.Length)
      {
        throw new ArgumentOutOfRangeException("arrayIndex");
      }
      int num = array.Length;
      if (num - arrayIndex < this._size)
      {
        throw new ArgumentException();
      }
      int num2 = (num - arrayIndex < this._size) ? (num - arrayIndex) : this._size;
      if (num2 == 0)
      {
        return;
      }
      int num3 = (this._array.Length - this._head < num2) ? (this._array.Length - this._head) : num2;
      Array.Copy(this._array, this._head, array, arrayIndex, num3);
      num2 -= num3;
      if (num2 > 0)
      {
        Array.Copy(this._array, 0, array, arrayIndex + this._array.Length - this._head, num2);
      }
    }
    /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="array" /> is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="index" /> is less than zero.</exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="array" /> is multidimensional.-or-<paramref name="array" /> does not have zero-based indexing.-or-The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.-or-The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null) throw new ArgumentNullException("array");
      if (array.Rank != 1) throw new ArgumentException();
      if (array.GetLowerBound(0) != 0) throw new ArgumentException();
      int length = array.Length;
      if (index < 0 || index > length) throw new ArgumentOutOfRangeException("index");
      if (length - index < this._size) throw new ArgumentException();
      int num = (length - index < this._size) ? (length - index) : this._size;
      if (num == 0)
      {
        return;
      }
      try
      {
        int num2 = (this._array.Length - this._head < num) ? (this._array.Length - this._head) : num;
        Array.Copy(this._array, this._head, array, index, num2);
        num -= num2;
        if (num > 0)
        {
          Array.Copy(this._array, 0, array, index + this._array.Length - this._head, num);
        }
      }
      catch (ArrayTypeMismatchException)
      {
        throw new ArgumentException();
      }
    }
    /// <summary>Adds an object to the end of the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.Queue`1" />. The value can be null for reference types.</param>
    public void Enqueue(T item)
    {
      if (this._size == this._array.Length)
      {
        int num = (int)((long)this._array.Length * 200L / 100L);
        if (num < this._array.Length + 4)
        {
          num = this._array.Length + 4;
        }
        this.SetCapacity(num);
      }
      this._array[this._tail] = item;
      this._tail = (this._tail + 1) % this._array.Length;
      this._size++;
      this._version++;
    }
    /// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
    /// <returns>An <see cref="T:System.Collections.Generic.Queue`1.Enumerator" /> for the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
    public InspectableQueue<T>.Enumerator GetEnumerator()
    {
      return new InspectableQueue<T>.Enumerator(this);
    }
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return new InspectableQueue<T>.Enumerator(this);
    }
    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new InspectableQueue<T>.Enumerator(this);
    }
    /// <summary>Removes and returns the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
    /// <returns>The object that is removed from the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
    /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Queue`1" /> is empty.</exception>
    public T Dequeue()
    {
      if (this._size == 0)
      {
        throw new InvalidOperationException();
      }
      T result = this._array[this._head];
      this._array[this._head] = default(T);
      this._head = (this._head + 1) % this._array.Length;
      this._size--;
      this._version++;
      return result;
    }
    /// <summary>Returns the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" /> without removing it.</summary>
    /// <returns>The object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
    /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Queue`1" /> is empty.</exception>
    public T Peek()
    {
      if (this._size == 0)
      {
        throw new InvalidOperationException();
      }
      return this._array[this._head];
    }
    /// <summary>Determines whether an element is in the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
    /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.Queue`1" />; otherwise, false.</returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.Queue`1" />. The value can be null for reference types.</param>
    public bool Contains(T item)
    {
      int num = this._head;
      int size = this._size;
      EqualityComparer<T> @default = EqualityComparer<T>.Default;
      while (size-- > 0)
      {
        if (item == null)
        {
          if (this._array[num] == null)
          {
            return true;
          }
        }
        else
        {
          if (this._array[num] != null && @default.Equals(this._array[num], item))
          {
            return true;
          }
        }
        num = (num + 1) % this._array.Length;
      }
      return false;
    }
    public T GetElement(int i)
    {
      if (i < 0 || i >= this.Count) throw new ArgumentOutOfRangeException("i");
      return this._array[(this._head + i) % this._array.Length];
    }
    /// <summary>Copies the <see cref="T:System.Collections.Generic.Queue`1" /> elements to a new array.</summary>
    /// <returns>A new array containing elements copied from the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
    public T[] ToArray()
    {
      T[] array = new T[this._size];
      if (this._size == 0)
      {
        return array;
      }
      if (this._head < this._tail)
      {
        Array.Copy(this._array, this._head, array, 0, this._size);
      }
      else
      {
        Array.Copy(this._array, this._head, array, 0, this._array.Length - this._head);
        Array.Copy(this._array, 0, array, this._array.Length - this._head, this._tail);
      }
      return array;
    }
    private void SetCapacity(int capacity)
    {
      T[] array = new T[capacity];
      if (this._size > 0)
      {
        if (this._head < this._tail)
        {
          Array.Copy(this._array, this._head, array, 0, this._size);
        }
        else
        {
          Array.Copy(this._array, this._head, array, 0, this._array.Length - this._head);
          Array.Copy(this._array, 0, array, this._array.Length - this._head, this._tail);
        }
      }
      this._array = array;
      this._head = 0;
      this._tail = ((this._size == capacity) ? 0 : this._size);
      this._version++;
    }
    /// <summary>Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.Queue`1" />, if that number is less than 90 percent of current capacity.</summary>
    public void TrimExcess()
    {
      int num = (int)((double)this._array.Length * 0.9);
      if (this._size < num)
      {
        this.SetCapacity(this._size);
      }
    }
  }
}
