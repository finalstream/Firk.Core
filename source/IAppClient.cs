using System;

namespace Firk.Core
{
    /// <summary>
    ///     �A�v���P�[�V�����̃N���C�A���g��\���܂��B
    /// </summary>
    public interface IAppClient : IDisposable
    {
        /// <summary>
        ///     �A�v���P�[�V�����ݒ���擾���܂��B
        /// </summary>
        IAppConfig AppConfig { get; }

        /// <summary>
        ///     �A�v���P�[�V�������������ς݂��ǂ����B
        /// </summary>
        bool IsInitialized { get; }
    }
}