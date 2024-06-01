// Services/MatchDataService.cs
using DataHandling.Models;
using DataHandling.Repositories;
using System.Collections.Generic;

namespace DataHandling.Services
{
    public class MatchDataService
    {
        private readonly MatchDataRepository _repository;

        public MatchDataService(MatchDataRepository repository)
        {
            _repository = repository;
        }

        public void SaveMatchData(IEnumerable<MatchDetails> matchDataList)
        {
            _repository.SaveMatchData(matchDataList);
        }

        public IEnumerable<MatchDetails> GetMatchData()
        {
            return _repository.LoadMatchData();
        }
    }
}
