namespace Firk.Core.Actions
{
    /// <summary>
    ///     標準のアクションを表します。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGeneralAction<T>
    {
        /// <summary>
        ///     アクションを実行します。
        /// </summary>
        /// <param name="_"></param>
        void Invoke(T _);
    }
}