﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using LegendsViewer.Legends.Enums;
using LegendsViewer.Legends.WorldObjects;

namespace LegendsViewer.Legends.Parser
{
    public class HistoryParser : IDisposable
    {
        private readonly BackgroundWorker _worker;
        private readonly World _world;
        private readonly StreamReader _historyReader;
        private readonly StringBuilder _log;

        private string _currentLine;
        private Entity _currentCiv;

        public HistoryParser(BackgroundWorker worker, World world, string historyFile)
        {
            _worker = worker;
            _world = world;
            _historyReader = new StreamReader(historyFile, Encoding.GetEncoding("windows-1252"));
            _log = new StringBuilder();
            _worker.ReportProgress(0, "\nParsing History File...");
        }

        private bool CivStart()
        {
            return !_currentLine.StartsWith(" ");
        }

        private void SkipAnimalPeople()
        {
            while (!_currentLine.Contains(",") || _currentLine.StartsWith(" "))
            {
                ReadLine();
            }
        }

        private void SkipToNextCiv()
        {
            while (!_historyReader.EndOfStream && !CivStart())
            {
                ReadLine();
            }
        }

        private void ReadLine()
        {
            _currentLine = _historyReader.ReadLine();
        }

        private bool ReadCiv()
        {
            string civName = _currentLine.Substring(0, _currentLine.IndexOf(",", StringComparison.Ordinal));

            CreatureInfo civRace = _world.GetCreatureInfo(
                _currentLine.Substring(_currentLine.IndexOf(",", StringComparison.Ordinal) + 2, 
                _currentLine.Length - _currentLine.IndexOf(",", StringComparison.Ordinal) - 2).ToLower());

            var entities = _world.Entities.Where(entity => string.Compare(entity.Name, civName, StringComparison.OrdinalIgnoreCase) == 0).ToList();

            if (entities.Count == 1)
            {
                _currentCiv = entities.First();
            }
            else if (!entities.Any())
            {
                _world.ParsingErrors.Report($"Couldn\'t Find Entity by Name:\n{civName}");
            }
            else
            {
                var possibleEntities = entities.Where(entity => entity.Race == civRace).ToList();

                if (possibleEntities.Count == 1)
                {
                    _currentCiv = possibleEntities.First();
                }
                else if (!possibleEntities.Any())
                {
                    _world.ParsingErrors.Report($"Couldn\'t Find Entity by Name and Race:\n{civName}");
                }
                else
                {
                    var possibleCivilizations = possibleEntities.Where(entity => entity.Type == EntityType.Civilization).ToList();

                    if (possibleCivilizations.Count == 1)
                    {
                        _currentCiv = possibleEntities.First();
                    }
                    else
                    {
#if DEBUG
                        _world.ParsingErrors.Report($"Ambiguous Civilizations ({entities.Count}) Name:\n{civName}");
#endif
                    }
                }
            }
            if (_currentCiv != null)
            {
                _currentCiv.Race = civRace;
                foreach (Entity group in _currentCiv.Groups)
                {
                    group.Race = _currentCiv.Race;
                }

                _currentCiv.IsCiv = true;
                _currentCiv.Type = EntityType.Civilization;
            }
            
            ReadLine();

            return true;
        }

        private void ReadWorships()
        {
            if (_currentLine.Contains("Worship List"))
            {
                ReadLine();

                while (_currentLine != null && _currentLine.StartsWith("  "))
                {
                    if (_currentCiv != null)
                    {
                        string deityName = Formatting.InitCaps(Formatting.ReplaceNonAscii(_currentLine.Substring(2, _currentLine.IndexOf(",", StringComparison.Ordinal) - 2)));

                        string deityNameToMatch = deityName.Replace("'", "`");

                        var deities = _world.HistoricalFigures.Where(h => h.Name.Equals(deityNameToMatch, StringComparison.OrdinalIgnoreCase) && (h.Deity || h.Force)).ToList();

                        if (deities.Count == 1)
                        {
                            var deity = deities.First();
                            
                            deity.WorshippedBy = _currentCiv;

                            _currentCiv.Worshipped.Add(deity);
                        }
                        else if (deities.Count == 0)
                        {
                            _world.ParsingErrors.Report($"Couldn\'t Find Deity:\n{deityName}, Deity of {_currentCiv.Name}");
                        }
#if DEBUG
                        else
                        {
                            _world.ParsingErrors.Report($"Ambiguous ({deities.Count}) Deity Name:\n{deityName}, Deity of {_currentCiv.Name}");
                        }
#endif
                    }

                    ReadLine();
                }
            }
        }

        private bool LeaderStart()
        {
            return !_historyReader.EndOfStream && _currentLine.Contains(" List") && !_currentLine.Contains("[*]") && !_currentLine.Contains("%");
        }

        private void ReadLeaders()
        {
            while (LeaderStart())
            {
                string leaderType = Formatting.InitCaps(_currentLine.Substring(1, _currentLine.IndexOf("List", StringComparison.Ordinal) - 2));

                _currentCiv?.LeaderTypes.Add(leaderType);
                _currentCiv?.Leaders.Add(new List<HistoricalFigure>());

                ReadLine();

                while (_currentLine != null && _currentLine.StartsWith("  "))
                {
                    if (_currentCiv != null && _currentLine.Contains("[*]"))
                    {
                        int starIndex = _currentLine.IndexOf("[*]", StringComparison.Ordinal);

                        string leaderName = Formatting.ReplaceNonAscii(
                            _currentLine.Substring(
                               starIndex + 4,
                               _currentLine.IndexOf("(b", StringComparison.Ordinal) - starIndex - 5)
                            );

                        string leaderNameToMatch = leaderName.Replace("'", "`");

                        var leaders = _world.HistoricalFigures.Where(hf => hf.Name.Equals(leaderNameToMatch, StringComparison.OrdinalIgnoreCase)).ToList();
                        
                        if (leaders.Count == 1)
                        {
                            var leader = leaders.First();
                            int reignBegan = Convert.ToInt32(_currentLine.Substring(_currentLine.IndexOf(":", StringComparison.Ordinal) + 2, 
                                _currentLine.IndexOf("), ", StringComparison.Ordinal) - _currentLine.IndexOf(":", StringComparison.Ordinal) - 2));
                            if (_currentCiv.Leaders[_currentCiv.LeaderTypes.Count - 1].Count > 0) //End of previous leader's reign
                            {
                                _currentCiv.Leaders[_currentCiv.LeaderTypes.Count - 1].Last().Positions.Last().Ended = reignBegan - 1;
                            }

                            if (leader.Positions.Count > 0 && leader.Positions.Last().Ended == -1) //End of leader's last leader position (move up rank etc.)
                            {
                                HistoricalFigure.Position lastPosition = leader.Positions.Last();
                                lastPosition.Ended = reignBegan;
                                lastPosition.Length = lastPosition.Began - reignBegan;
                            }
                            HistoricalFigure.Position newPosition = new HistoricalFigure.Position(_currentCiv, reignBegan, -1, leaderType);
                            if (leader.DeathYear != -1)
                            {
                                newPosition.Ended = leader.DeathYear;
                                newPosition.Length = leader.DeathYear - newPosition.Ended;
                            }
                            else
                            {
                                newPosition.Length = _world.Events.Last().Year - newPosition.Began;
                            }

                            leader.Positions.Add(newPosition);
                            _currentCiv.Leaders[_currentCiv.LeaderTypes.Count - 1].Add(leader);
                        }
                        else if (leaders.Count == 0)
                        {
                            _world.ParsingErrors.Report($"Couldn\'t Find Leader:\n{leaderName}, Leader of {_currentCiv.Name}");
                        }
#if DEBUG
                        else
                        {
                            _world.ParsingErrors.Report($"Ambiguous ({leaders.Count}) Leader Name:\n{leaderName}, Leader of {_currentCiv.Name}");
                        }
#endif
                    }

                    ReadLine();
                }
            }
        }

        public string Parse()
        {
            _worker.ReportProgress(0, "... Civilization Infos");

            _world.Name = Formatting.ReplaceNonAscii(_historyReader.ReadLine());
            _world.Name += ", " + _historyReader.ReadLine();

            ReadLine();

            SkipAnimalPeople();

            while (!_historyReader.EndOfStream)
            {
                if (ReadCiv())
                {
                    ReadWorships();
                    ReadLeaders();
                }
                else
                {
                    SkipToNextCiv();
                }
            }

            if (_currentLine != null && CivStart())
                ReadCiv();

            _historyReader.Close();

            return _log.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _historyReader.Dispose();
            }
        }
    }
}
