using System;
using System.Collections.Generic;

namespace CommandUndoRedo
{
	public class UndoRedo
	{
		public int MaxUndoStored {get {return _undoCommands.MaxLength;} set {SetMaxLength(value);}}

		DropoutStack<ICommand> _undoCommands = new DropoutStack<ICommand>();
		DropoutStack<ICommand> _redoCommands = new DropoutStack<ICommand>();

		public UndoRedo() {}
		public UndoRedo(int maxUndoStored)
		{
			this.MaxUndoStored = maxUndoStored;
		}

		public void Clear()
		{
			_undoCommands.Clear();
			_redoCommands.Clear();
		}

		public void Undo()
		{
			if(_undoCommands.Count > 0)
			{
				ICommand command = _undoCommands.Pop();
				command.UnExecute();
				_redoCommands.Push(command);
			}
		}

		public void Redo()
		{
			if(_redoCommands.Count > 0)
			{
				ICommand command = _redoCommands.Pop();
				command.Execute();
				_undoCommands.Push(command);
			}
		}

		public void Insert(ICommand command)
		{
			if(MaxUndoStored <= 0) return;

			_undoCommands.Push(command);
			_redoCommands.Clear();
		}

		public void Execute(ICommand command)
		{
			command.Execute();
			Insert(command);
		}

		void SetMaxLength(int max)
		{
			_undoCommands.MaxLength = max;
			_redoCommands.MaxLength = max;
		}
	}
}
