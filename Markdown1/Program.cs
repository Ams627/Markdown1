using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Markdown1
{
    class TokenInfo
    {

    }

    class NlcInfo : TokenInfo
    {
        public string Nlc { get; private set; }
        public List<(string tvm, string ip)> NamesAndIps { get; set; }
    }

    enum Tokens
    {
        DotNlc, // start of every location definition. .nlc 5883 rp02(192.168.207.1) rp03(192.168.207.2)
        Nlc,
        Crs,
        Route,
        TicketType,
        Date,
        DateTime,
        Days,
        DotQuicks,
        DotPops,
        DotBands,
        DotHolidays,
        QuadDot,
        KvPair,
        NewLine,
        EOF
    }

    static class Patterns
    {
        public static string QuadDotIp => 
            @"(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\." +
            @"(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\." +
            @"(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\." +
            @"(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])";
        public static string TvmName => "[A-Za-z0-9]+";
        public static string Nlc => "[A-Z0-9]{3}[0-9]";
        public static string Crs => "[A-Z]{3}";
        //public static string DotNlc => ".nlc";
        //public static string DotQuick => ".quick";
        //public static string DotPops => ".pops";
        //public static string DotBands => ".bands";
        //public static string DotHolidays => ".holidays";
    }
    /// <summary>
    /// This is a lexical analyser. Do some reading before changing it! It is specifically designed to parse
    /// tokens for 'Customer Markdown Language' (custmd). Doing this directly in 
    /// https://en.wikipedia.org/wiki/Compilers:_Principles,_Techniques,_and_Tools
    /// https://en.wikipedia.org/wiki/Flex_(lexical_analyser_generator)
    /// https://en.wikipedia.org/wiki/Lex_(software)
    /// </summary>
    /// 
    internal class Lexer
    {
        private string _filename;
        public Lexer(string filename)
        {
            _filename = filename;
        }

        public IEnumerable<Tokens> GetTokens()
        {
            foreach (var line in File.ReadLines(_filename).Select(x=>x.Trim()))
            {
                var tokens = Regex.Split(line, "[ \t]");
                foreach (var token in tokens)
                {
                    if (token == ".nlc")
                    {
                        yield return Tokens.DotNlc;
                    }
                    else if (token == ".quicks")
                    {
                        yield return Tokens.DotQuicks;
                    }
                    else if (token == ".pops")
                    {
                        yield return Tokens.DotPops;
                    }
                    else if (token == ".bands")
                    {
                        yield return Tokens.DotBands;
                    }
                    else if (token == ".holidays")
                    {
                        yield return Tokens.DotHolidays;
                    }
                    else if (Regex.Match(token, ""))
                    {

                    }
                }
                yield return Tokens.NewLine;
            }
            yield return Tokens.EOF;
        }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {
            var match = Regex.Match("192.168.201.7", Patterns.QuadDotIp);
            Console.WriteLine("number of groups {0}", match.Groups.Count);
            foreach (var grp in match.Groups)
            {
                Console.WriteLine("group: {0}", grp);
            }
            Console.WriteLine();
            try
            {
                if (args.Length != 1)
                {
                    throw new Exception($"You must supply at least one custmd file.");
                }
                var lexer = new Lexer(args[0]);
                var tok = lexer.GetTokens().First();
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
