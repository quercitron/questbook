namespace FastSearch
{
    public class StateFactory
    {
        private int _currentStateId;

        public State Create()
        {
            _currentStateId++;
            return new State { Id = _currentStateId };
        }
    }
}
