using System;
using System.Collections.Generic;

namespace CommandUndoRedo
{
	public class DropoutStack<T> : LinkedList<T>
	{
		int _maxLength = int.MaxValue;
		public int MaxLength {get {return _maxLength;} set {SetMaxLength(value);}}

		public DropoutStack() {}
		public DropoutStack(int maxLength)
		{
			this.MaxLength = maxLength;
		}

		public void Push(T item)
		{
			if(this.Count > 0 && this.Count + 1 > MaxLength)
			{
				this.RemoveLast();
			}

			if(this.Count + 1 <= MaxLength)
			{
				this.AddFirst(item);
			}
		}

		public T Pop()
		{
			T item = this.First.Value;
			this.RemoveFirst();
			return item;
		}

		void SetMaxLength(int max)
		{
			_maxLength = max;

			if(this.Count > _maxLength)
			{
				int leftover = this.Count - _maxLength;
				for(int i = 0; i < leftover; i++)
				{
					this.RemoveLast();
				}
			}
		}
	}
}
