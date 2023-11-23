using System;

namespace Utils.Analytics
{
    public class NumericAccumulator
    {
        private static DateTime mInitialTime = new DateTime(1970, 1, 1, 0, 0, 0);

        public ulong TotalValue { get; private set; } = 0;
        // Current bps
        private ulong mValuePerSecond;
        public ulong PerSecond
        {
            get
            {
                checkPerSecond();
                return mValuePerSecond;
            }
        }
        // Last bps
        private ulong mLastCount;
        public ulong LastCount => mLastCount;
        // Measure time
        private int mLastSecond = 0;

        private object mLocker = new object();
        public event Action<ulong> OnPerSecondFlushed;

        public string Name { get; private set; } = "";

        public NumericAccumulator()
        {
            TotalValue = 0;
            mLastSecond = DateTime.Now.Second;
        }

        public NumericAccumulator(string name)
        {
            Name = name;
            TotalValue = 0;
            mLastSecond = DateTime.Now.Second;
        }

        public void Accumulate(int value)
        {
            lock (mLocker)
            {
                checkPerSecond();

                if (value < 0)
                {
                    return;
                }

                TotalValue += (ulong)value;
                mValuePerSecond += (ulong)value;
            }
        }

        private void checkPerSecond()
        {
            int currentSecond = (int)(DateTime.UtcNow - mInitialTime).TotalSeconds;

            if (mLastSecond < currentSecond)
            {
                mLastSecond = currentSecond;
                mLastCount = mValuePerSecond;
                OnPerSecondFlushed?.Invoke(mValuePerSecond);
                mValuePerSecond = 0;
            }
        }

        public void Reset()
        {
            lock (mLocker)
            {
                TotalValue = 0;
                mValuePerSecond = 0;
            }
        }

        public override string ToString()
        {
            return $"[{Name}/total : {TotalValue}][{Name}/ps : {PerSecond}]";
        }
    }
}
