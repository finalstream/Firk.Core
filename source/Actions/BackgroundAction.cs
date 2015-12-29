using System.Threading.Tasks;

namespace Firk.Core.Actions
{
    public abstract class BackgroundAction
    {
        protected abstract void InvokeCoreAsync();

        public Task InvokeAsync()
        {
            return Task.Run(() => InvokeCoreAsync());
        }
    }
}