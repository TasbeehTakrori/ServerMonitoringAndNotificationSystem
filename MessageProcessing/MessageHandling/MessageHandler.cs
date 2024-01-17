using MessageProcessing.Repository;

namespace MessageProcessing.MessageHandling
{
    internal class MessageHandler<T> : IMessageHandler<T>
    {
        private readonly IRepository<T> _repository;
        public MessageHandler(IRepository<T> repository)
        {
            _repository = repository;
        }
        public async Task HandleMessage(T message)
        {
            try
            {
                await _repository.SaveAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
