using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Provides <see cref="T:Pegasustan.Domain.Language"/> extensions.
    /// </summary>
    public static class LanguageExtensions
    {
        /// <summary>
        /// Finds a <see cref="T:Pegasustan.Domain.Language" /> by its code.
        /// </summary>
        /// <param name="code">The language code.</param>
        /// <returns>A <see cref="T:Pegasustan.Domain.Language" /> reference if it is found, otherwise default value - null.</returns>
        public static Language FindByCode(this IEnumerable<Language> languages, string code)
        {
            return languages.SingleOrDefault(language =>
                language.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
    }
}