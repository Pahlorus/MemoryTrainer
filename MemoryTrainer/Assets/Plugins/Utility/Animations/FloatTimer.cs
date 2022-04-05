namespace Utility.Animations
{
    public struct FloatTimer
    {
        public float Value => _value;
        private float _value;

        public void Init(float value) => _value = value;
        public bool Count(float dt, float max, bool reset = true)
        {
            _value += dt;
            if (_value >= max)
            {
                if (reset) 
                    _value -= max;
                return true;
            }
            return false;
        }

        public bool Ready(float max)
        {
            return _value >= max;
        }
    }
}