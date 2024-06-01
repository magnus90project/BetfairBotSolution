// Repositories/MatchDataRepository.cs
using DataHandling.Models;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace DataHandling.Repositories
{
    public class MatchDataRepository
    {
        private readonly string _filePath;

        public MatchDataRepository(string filePath)
        {
            _filePath = filePath;
        }

        public void SaveMatchData(IEnumerable<MatchDetails> matchDataList)
        {
            using (var writer = new StreamWriter(_filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(matchDataList);
            }
        }

        public IEnumerable<MatchDetails> LoadMatchData()
        {
            using (var reader = new StreamReader(_filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                return csv.GetRecords<MatchDetails>();
            }
        }
    }
}
