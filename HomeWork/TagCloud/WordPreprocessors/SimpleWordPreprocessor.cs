﻿using System.Collections.Generic;
using System.Linq;
using TagCloud.BoringWordsRepositories;
using TagCloud.Readers;

namespace TagCloud.WordPreprocessors
{
    public class SimpleWordPreprocessor : IWordPreprocessor
    {
        private readonly IReader wordsReader;
        private readonly IBoringWordsStorage boringWordsStorage;
        private Result<HashSet<string>> boringWords;

        public SimpleWordPreprocessor(IReader wordsReader, IBoringWordsStorage boringWordsStorage)
        {
            this.wordsReader = wordsReader;
            this.boringWordsStorage = boringWordsStorage;
        }

        public Result<IEnumerable<string>> GetPreprocessedWords()
        {
            boringWords = boringWordsStorage.GetBoringWords();
            var readWords = wordsReader.ReadWords();
            return !boringWords.IsSuccess || !readWords.IsSuccess
                ? readWords.RefineError("Error when reading words: ", true).RefineError(
                        boringWords.RefineError("Error when reading boring words: ", true).Error)
                : readWords.Then(words => words
                        .Select(word => word.ToLower())
                        .Where(word => !boringWords.Value.Contains(word)));
        }
    }
}
