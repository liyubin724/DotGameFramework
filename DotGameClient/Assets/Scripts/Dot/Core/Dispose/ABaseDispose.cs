using System;

namespace Dot.Core.Dispose
{
    /// <summary>
    /// 需要实现Dispose模式的，可以通过继承实现
    /// </summary>
    public abstract class ABaseDispose : IDisposable
    {
        private bool disposed = false;

        ~ABaseDispose()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if(disposing)
            {
                DisposeManagedResource();
            }

            DisposeUnmanagedResource();
            disposed = true;
        }
        //清理托管资源
        protected abstract void DisposeManagedResource();
        //清理非托管资源
        protected abstract void DisposeUnmanagedResource();

    }
}
