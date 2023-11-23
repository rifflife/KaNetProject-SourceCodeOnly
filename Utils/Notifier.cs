using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utiles
{
	public class SubjectData<T>
	{
		public event Action OnChanged;
		public event Action<T> OnDataChanged;

		protected T mValue;
		public T Value
		{
			get
			{
				return mValue;
			}
			set
			{
				if (!mValue.Equals(value))
				{
					mValue = value;
					IsDirty = true;
					OnChanged?.Invoke();
					OnDataChanged?.Invoke(Value);
				}
			}
		}

		public bool IsDirty { get; private set; }

		public SubjectData(T value = default(T), bool isDirty = false)
		{
			mValue = value;

			if (isDirty)
			{
				IsDirty = true;
				OnChanged?.Invoke();
				OnDataChanged?.Invoke(mValue);
			}
		}

		public void SetPristine()
		{
			IsDirty = false;
		}

		public void SetValueWithoutEvent(T value)
		{
			mValue = value;
		}
	}

	public class ManualSubjectData<T>
	{
		public event Action OnChanged;
		public event Action<T> OnDataChanged;

		protected T mPreviousValue;
		public T Value;

		public bool IsDirty { get; private set; }

		public ManualSubjectData(T value = default(T), bool isDirty = false)
		{
			Value = value;
			mPreviousValue = Value;

			if (isDirty)
			{
				IsDirty = true;
				OnChanged?.Invoke();
				OnDataChanged?.Invoke(Value);
			}
		}

		public void EvaluateDirty()
		{
			if (!mPreviousValue.Equals(Value))
			{
				IsDirty = true;
				mPreviousValue = Value;
				OnChanged.Invoke();
				OnDataChanged.Invoke(Value);
			}
		}

		public void SetPristine()
		{
			IsDirty = false;
		}
	}
}
