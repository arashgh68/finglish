 public class FingilishConvertor
    {
        Dictionary<string, string> Dictionary;
        Dictionary<string, string> Begining;
        Dictionary<string, string> Middle;
        Dictionary<string, string> Ending;
        Dictionary<string, int> WordFreq;

        public FingilishConvertor()
        {
            Dictionary = new Dictionary<string, string>();
            Begining = new Dictionary<string, string>();
            Middle = new Dictionary<string, string>();
            Ending = new Dictionary<string, string>();
            WordFreq = new Dictionary<string, int>();
            var dictFile = File.ReadAllLines("dict.txt");
            var begFile = File.ReadAllLines("beginning.txt");
            var midFile = File.ReadAllLines("middle.txt");
            var endFile = File.ReadAllLines("ending.txt");
            var freqFile = File.ReadAllLines("persian-word-freq.txt");

            foreach (string line in dictFile)
            {
                var word = line.Split(' ');
                if (word.Length == 2) { Dictionary.Add(word[0], word[1]); }
            }

            foreach (string line in begFile)
            {
                var word = line.Split(' ');
                Begining.Add(word[0], line.Substring(word[0].Length + 1));
            }

            foreach (string line in midFile)
            {
                var word = line.Split(' ');
                Middle.Add(word[0], line.Substring(word[0].Length + 1));
            }

            foreach (string line in endFile)
            {
                var word = line.Split(' ');
                Ending.Add(word[0], line.Substring(word[0].Length + 1));
            }

            foreach (string line in freqFile)
            {
                if (!line.StartsWith("#"))
                {
                    try
                    {
                        var word = line.Split('\t')[0];
                        int count = int.Parse(line.Split('\t')[1]);
                        if (!WordFreq.ContainsKey(word))
                        {
                            WordFreq.Add(word, count);
                        }
                    }
                    catch { }
                }
            }
        }
        private List<string> replaceLetter(string word)
        {
            List<string> result = new List<string>();
            if (word == "a")
            {
                result.Add("A");
            }
            else if (word.Length == 1)
            {
                result.Add(word);
            }
            else if (word == "aa")
            {
                result.Add("A");
            }
            else if (word == "ee")
            {
                result.Add("i");
            }
            else if (word == "ei")
            {
                result.Add("ei");
            }
            else if (word == "oo" || word == "ou")
            {
                result.Add("u");
            }
            else if (word == "kha")
            {
                result.Add("kh");
                result.Add("a");
            }  
            else if(word == "kh"|| word == "gh" || word == "ch" || word == "sh" || word == "zh" || word == "ck")
            {
                result.Add(word);
            }
            else if (word.Length == 2 && word.Substring(0, 1) == word.Substring(1, 1))
            {
                result.Add(word.Substring(0, 1));
            }
            else if (word.Substring(0, 2) == "aa")
            {
                result.Add("A");
                result.AddRange(replaceLetter(word.Substring(2)));
            }
            else if (word.Substring(0, 2) == "ee")
            {
                result.Add("i");
                result.AddRange(replaceLetter(word.Substring(2)));
            }
            else if (word.Substring(0, 2) == "oo" || word.Substring(0, 2) == "ou")
            {
                result.Add("u");
                result.AddRange(replaceLetter(word.Substring(2)));
            }
            else if (word.Substring(0, 2) == "kh" || word.Substring(0, 2) == "gh" || word.Substring(0, 2) == "ch" || word.Substring(0, 2) == "sh" || word.Substring(0, 2) == "zh" || word.Substring(0, 2) == "ck")
            {
                result.Add(word.Substring(0, 2));
                result.AddRange(replaceLetter(word.Substring(2)));
            }
            else if (word.Length >= 2 && word.Substring(0, 1) == word.Substring(1, 1))
            {
                result.Add(word.Substring(0, 1));
                result.AddRange(replaceLetter(word.Substring(2)));
            }
            else
            {
                result.Add(word.Substring(0, 1));
                result.AddRange(replaceLetter(word.Substring(1)));
            }

            return result;
        }

        public string Convert(string phrase,int maxChar = 15)
        {

            string result = "";
            foreach (var word in phrase.Split(' '))
            {
                var lWord = word.ToLower();
                if (lWord.Length > maxChar)
                {
                    continue;
                }
                if (Dictionary.ContainsKey(lWord))
                {
                    result += Dictionary[lWord] + ' ';
                }
                else
                {
                    var wordArray = replaceLetter(lWord);
                    var possibleWords = CreateWords(wordArray);
                    var wordCount = getFrequency(possibleWords);
                    wordCount = wordCount.OrderByDescending(d => d.Value).ToDictionary(d => d.Key, d => d.Value);
                    result += wordCount.FirstOrDefault().Key + " ";
                }
            }

            return result;
        }

        private Dictionary<string, int> getFrequency(List<string> words)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (result.ContainsKey(word))
                {
                    continue;
                }
                int count = 0;
                if (WordFreq.TryGetValue(word, out count))
                {
                    result.Add(word, count);
                }
            }
            return result;
        }

        private List<string> CreateWords(List<string> word)
        {
            List<string> possibleChars = new List<string>();
            for (int i = 0; i < word.Count; i++)
            {
                if (i == 0)
                {
                    possibleChars.Add(Begining.GetValueOrDefault(word[i].ToString()));
                }
                else if (i == word.Count - 1)
                {
                    possibleChars.Add(Ending.GetValueOrDefault(word[i].ToString()));
                }
                else
                {
                    possibleChars.Add(Middle.GetValueOrDefault(word[i].ToString()));
                }
            }

            //List<string> words = new List<string>();
            //int posword = 1;
            //foreach(var pos in posibbleChars)
            //{
            //    posword *= pos.Split(' ').Length;
            //}


            //for(int i = 0; i < posword; i++)
            //{

            //}

            List<string> words = CombineLetter(possibleChars).ToList();

            return words;
        }
        private IEnumerable<string> CombineLetter(List<string> posChar)
        {


            if (posChar.Count == 0 || posChar.First()==null)
            {
                yield break;
            }
            foreach (var c in posChar.First().Split(' '))
            {
                if (posChar.Count > 1)
                {
                    foreach (var restWord in CombineLetter(posChar.TakeLast(posChar.Count - 1).ToList()))
                    {
                        if (c == "nothing")
                        {
                            yield return restWord;
                        }
                        else
                        {
                            yield return c + restWord;
                        }
                    }
                }
                else
                {
                    yield return "" + c;
                }
            }
        }
    }
