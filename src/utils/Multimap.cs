using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiDelphi.utils
{

	/// <summary>
	/// Multimap: container with multiple values mapped to a key.
	/// Implemented as an extension of a standard Dictionary, Key => List[value]
	/// </summary>
	public class Multimap<TKey, TValue> : Dictionary<TKey, IList<TValue>>
	{
		public Multimap() : base() { }

		public Multimap(int allocSize) : base(allocSize) { }

		private void CheckNullKey(TKey key)
		{
			if (key == null)
				throw new ArgumentNullException("key " + typeof(TKey).ToString());
		}

		/// <summary>
		/// Adds the specified value under the specified key.
		/// </summary>
		public void Add(TKey key, TValue value)
		{
			CheckNullKey(key);

			IList<TValue> container = null;
			if (!this.TryGetValue(key, out container))
			{
				container = new List<TValue>();
				base.Add(key, container);
			}
			container.Add(value);
		}

		/// <summary>
		/// Removes the specified value for the specified key.
		/// </summary>
		/// <remarks>
		/// Also removes the IList values' container if no values are left.
		/// </remarks>
		public void Remove(TKey key, TValue value)
		{
			CheckNullKey(key);

			IList<TValue> container = null;
			if (this.TryGetValue(key, out container))
			{
				container.Remove(value);
				if (container.Count <= 0)
					this.Remove(key);
			}
		}

		/// <summary>
		/// Replace value for key, if set, else adds the value
		/// </summary>
		public void Replace(TKey key, TValue value)
		{
			CheckNullKey(key);
			this.Remove(key);
			this.Add(key, value);
		}

		/// <summary>
		/// Gets first value mapped to the specified key.
		/// </summary>
		public TValue Get(TKey key)
		{
			IList<TValue> values;
			if (this.TryGetValue(key, out values))
				return values[0];
			else
				return default(TValue);
		}

		/// <summary>
		/// Gets all the values mapped to the specified key.
		/// </summary>
		public IEnumerable<TValue> GetAll(TKey key)
		{
			IList<TValue> values;
			if (this.TryGetValue(key, out values))
				foreach (var v in values)
					yield return v;
			else
				yield break;
		}

		/// <summary>
		/// Determines whether this dictionary contains the specified value for the specified key.
		/// </summary>
		/// <returns>true if the value is stored for the specified key in this dictionary, false otherwise</returns>
		public bool ContainsValue(TKey key, TValue value)
		{
			CheckNullKey(key);

			IList<TValue> values = null;
			if (this.TryGetValue(key, out values))
				return values.Contains(value);
			return false;
		}

		/// <summary>
		/// Gets flat collection of all values stored in the map.
		/// </summary>
		public new IEnumerable<TValue> Values
		{
			get
			{
				foreach (var vl in base.Values)
					foreach (var v in vl)
						yield return v;
			}
		}

		/// <summary>
		/// Gets flat collection of all KeyPairs stored in the map.
		/// </summary>
		public IEnumerable<KeyValuePair<TKey, TValue>> KeyValueSet
		{
			get
			{
				foreach (var kp in this)
					foreach (var v in kp.Value)
						yield return new KeyValuePair<TKey, TValue>(kp.Key, v);
			}
		}

		/// <summary>
		/// Determines whether this dictionary contains the specified value
		/// </summary>
		/// <returns>true if the value is found, false otherwise</returns>
		public bool ContainsValue(TValue value)
		{
			foreach (var v in this.Values)
				if (v.Equals(value))
					return true;
			return false;
		}

		public new int Count { get { return base.Count; } }

		/// <summary>
		/// Item [ ] property. Gets a value for the key or adds a new value
		/// </summary>
		public new TValue this[TKey key]
		{
			get { return this.Get(key); }
			set { this.Add(key, value);	}
		}

		/// <summary>
		/// Merges the specified multivaluedictionary into this instance.
		/// </summary>
		/// <param name="toMergeWith">To merge with.</param>
		public void Merge(Multimap<TKey, TValue> toMergeWith)
		{
			if (toMergeWith == null)
				return;

			foreach (KeyValuePair<TKey, IList<TValue>> pair in toMergeWith)
				foreach (TValue value in pair.Value)
					this.Add(pair.Key, value);
		}

		/// <summary>
		/// Gets the values for the key specified. This method is useful if you want to avoid an exception
		/// for key value retrieval and you can't use TryGetValue (e.g. in lambdas)
		/// </summary>
		public IList<TValue> GetValues(TKey key, bool returnEmptySet)
		{
			CheckNullKey(key);

			IList<TValue> retVal = null;
			if (!base.TryGetValue(key, out retVal) && returnEmptySet)
				return new List<TValue>();
			else
				return retVal;
		}
	}
}
