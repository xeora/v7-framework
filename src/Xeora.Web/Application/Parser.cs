using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xeora.Web.Directives;
using Xeora.Web.Directives.Elements;
using Xeora.Web.Exceptions;
using Xeora.Web.Global;
using Property = Xeora.Web.Directives.Elements.Property;

namespace Xeora.Web.Application
{
    internal class Parser : IDisposable
    {
        private static readonly ConcurrentDictionary<string, IList<DirectiveFactory>> _ParserCache = new ();
        
        public static void Reset() =>
            Parser._ParserCache.Clear();
        
        public static void Parse(Action<IDirective> resultHandler, string rawValue, ArgumentCollection arguments)
        {
            Parser parser = new Parser(resultHandler, rawValue, arguments);
            try
            {
                parser.Parse();
            }
            finally
            {
                parser.Dispose();
            }
        }

        private readonly Action<IDirective> _ResultHandler;
        private readonly StringReader _Reader;
        private readonly ArgumentCollection _Arguments;
        private readonly StringBuilder _SingleCache;
        private readonly StringBuilder _ContentCache;
        private string _Crumb;

        private string Id { get; }
        private IList<DirectiveFactory> Directives { get; }
        
        private Parser(Action<IDirective> resultHandler, string rawValue, ArgumentCollection arguments)
        {
            this._ResultHandler = resultHandler;
            this._Reader = new StringReader(rawValue);
            this._Arguments = arguments;

            this._SingleCache = new StringBuilder();
            this._ContentCache = new StringBuilder();
            this._Crumb = string.Empty;

            this.Id = CalculateId(rawValue);
            this.Directives = new List<DirectiveFactory>();
        }

        private static string CalculateId(string rawValue)
        {
            System.Security.Cryptography.SHA512 sha512Cypher = 
                System.Security.Cryptography.SHA512.Create();
            
            byte[] hashedValue = 
                sha512Cypher.ComputeHash(Encoding.UTF8.GetBytes(rawValue));

            return Convert.ToHexString(hashedValue);
        }
        
        public void Dispose() =>
            this._Reader.Close();
        
        private class Content
        {
            public string DirectiveId { get; set; }
            public string DirectiveType { get; set; }
        }

        private class DirectiveFactory
        {
            public DirectiveFactory(DirectiveTypes type, string rawValue)
            {
                this.DirectiveType = type;
                this.RawValue = rawValue;
            }
            
            private DirectiveTypes DirectiveType { get; }
            private string RawValue { get; }

            public IDirective Make(ArgumentCollection arguments)
            {
                switch (DirectiveType)
                {
                    case DirectiveTypes.AsyncGroup:
                        return new AsyncGroup(this.RawValue, null);
                    case DirectiveTypes.Control:
                        return new Control(this.RawValue, null);
                    case DirectiveTypes.ControlAsync:
                        return new ControlAsync(this.RawValue, null);
                    case DirectiveTypes.EncodedExecution:
                        return new EncodedExecution(this.RawValue, null);
                    case DirectiveTypes.Execution:
                        return new Execution(this.RawValue, null);
                    case DirectiveTypes.HashCodePointedTemplate:
                        return new HashCodePointedTemplate(this.RawValue, null);
                    case DirectiveTypes.InLineStatement:
                        return new InLineStatement(this.RawValue, null);
                    case DirectiveTypes.MessageBlock:
                        return new MessageBlock(this.RawValue, null);
                    case DirectiveTypes.PartialCache:
                        return new PartialCache(this.RawValue, null);
                    case DirectiveTypes.PermissionBlock:
                        return new PermissionBlock(this.RawValue, null);
                    case DirectiveTypes.Template:
                        return new Template(this.RawValue, null);
                    case DirectiveTypes.Translation:
                        return new Translation(this.RawValue, null);
                    case DirectiveTypes.ReplaceableTranslation:
                        return new ReplaceableTranslation(this.RawValue, null);
                    case DirectiveTypes.UpdateBlock:
                        return new UpdateBlock(this.RawValue, null);
                    case DirectiveTypes.Property:
                        return new Property(this.RawValue, arguments);
                    case DirectiveTypes.Static:
                        return new Static(this.RawValue);
                    default:
                        // DirectiveTypes.Single:     This is the most top directive which is not allowed to be used internally.
                        // DirectiveTypes.Undefined:  Takes no action for parsing
                        throw new GrammarException();
                }
            }
        }

        private void Parse()
        {
            // Check cache first and skip parsing if it is exists...
            if (Parser._ParserCache.TryGetValue(this.Id, out IList<DirectiveFactory> factories))
            {
                foreach (DirectiveFactory factory in factories)
                    this._ResultHandler.Invoke(factory.Make(this._Arguments));
                return;
            }
            
            int cursorIndex = 0;

            while (this._Reader.Peek() > -1)
            {
                string line = 
                    this._Reader.ReadLine();
                this._SingleCache.AppendLine(line);

                do
                {
                    Content content = this.FindContent(cursorIndex, line);

                    if (content == null)
                        break;

                    if (this._SingleCache.Length > 0)
                    {
                        this.HandleSingles();
                        cursorIndex += this._SingleCache.Length;
                        this._SingleCache.Clear();
                    }

                    this.HandleContent(content);
                    cursorIndex += this._ContentCache.Length;
                    this._ContentCache.Clear();

                    if (string.IsNullOrEmpty(this._Crumb)) continue;
                    
                    this._SingleCache.AppendLine(this._Crumb);
                    line = this._Crumb;
                } while (this._SingleCache.Length > 0);
            }

            if (this._SingleCache.Length > 0)
                this.HandleSingles();

            // Cache parse for future use...
            Parser._ParserCache.TryAdd(this.Id, this.Directives);
        }

        private Content FindContent(int index, string line)
        {
            Match coMatch =
                RegularExpression.Current.ContentOpeningPattern.Match(line);
            if (!coMatch.Success) return null;

            this._SingleCache.Remove(
                this._SingleCache.Length - (line.Length + Environment.NewLine.Length), 
                line.Length + Environment.NewLine.Length
            );
            this._SingleCache.Append(line.Substring(0, coMatch.Index));

            string modifierText =
                $"~{this._SingleCache.Length + index}";
            string directiveId =
                coMatch.Result("${DirectiveId}");
            string directiveType =
                coMatch.Result("${DirectiveType}");
            Regex openingPattern =
                RegularExpression.Current.SpecificContentOpeningPattern(directiveId, directiveType);

            this._ContentCache.Append(
                line.Substring(coMatch.Index, coMatch.Length)
            );
            this._ContentCache.Insert(
                coMatch.Length - 2,
                modifierText
            );
            line = line.Substring(coMatch.Index + coMatch.Length);

            int nestedCount = 0;
            while (!this.FindContentTail(ref nestedCount, line, directiveId, modifierText, openingPattern))
            {
                if (this._Reader.Peek() == -1)
                    return null;

                line = this._Reader.ReadLine();
            }

            int separatorIndex = 
                Parser.SearchForContentSeparator(this._ContentCache.ToString(), directiveId, openingPattern);
            if (separatorIndex > -1)
                this._ContentCache.Insert(separatorIndex + directiveId.Length + 2, modifierText);

            return new Content
            {
                DirectiveId = directiveId,
                DirectiveType = directiveType
            };
        }

        private bool FindContentTail(ref int nestedCount, string line, string directiveId, string modifierText, Regex openingPattern)
        {
            if (string.IsNullOrEmpty(line))
                return false;

            int ccIndex =
                Parser.SearchForContentClosing(ref nestedCount, line, directiveId, openingPattern);
            if (ccIndex > -1 && nestedCount == 0)
            {
                this._ContentCache.Append(line, 0, ccIndex);
                this._ContentCache.Insert(
                    this._ContentCache.Length - 1,
                    modifierText
                );
                this._Crumb = line.Substring(ccIndex);

                return true;
            }

            this._ContentCache.AppendLine(line);

            return false;
        }
        
        private static int SearchForContentSeparator(string capturedContent, string directiveId, Regex openingPattern)
        {
            string ccSearch =
                $"}}:{directiveId}:{{";
            int ccIndex = capturedContent.IndexOf(ccSearch, StringComparison.InvariantCulture);

            if (ccIndex == -1)
                return -1;

            MatchCollection contentOpeningMatches =
                openingPattern.Matches(capturedContent, 1);

            if (contentOpeningMatches.Count == 0)
                return ccIndex;

            IEnumerator comEnum =
                contentOpeningMatches.GetEnumerator();
            int nestedCount = 0;

            while (comEnum.MoveNext())
            {
                Match contentOpeningMatch = (Match)comEnum.Current;

                if (contentOpeningMatch.Index >= ccIndex) return ccIndex;
                
                nestedCount++;
            }

            for (int i = 0; i < nestedCount; i++)
            {
                ccIndex = capturedContent.IndexOf(ccSearch, ccIndex + 1, StringComparison.InvariantCulture);
                if (ccIndex == -1) break;

                nestedCount--;
            }

            return ccIndex;
        }

        private static int SearchForContentClosing(ref int nestedCount, string line, string directiveId, Regex openingPattern)
        {
            Match coMatch =
                openingPattern.Match(line);
            string ccSearch =
                $"}}:{directiveId}$";
            int ccIndex = line.IndexOf(ccSearch, StringComparison.InvariantCulture);

            if (coMatch.Success && (ccIndex == -1 || ccIndex > coMatch.Index))
            {
                nestedCount++;
                return -1;
            }

            if (ccIndex > -1 && nestedCount > 0)
            {
                nestedCount--;
                return -1;
            }

            if (ccIndex == -1)
                return -1;

            return ccIndex + ccSearch.Length;
        }

        private void HandleSingles()
        {
            DirectiveFactory directiveFactory;
            
            string rawValue = this._SingleCache.ToString();
            int lastNewLineIndex = 
                rawValue.LastIndexOf(Environment.NewLine, StringComparison.InvariantCulture);
            if (lastNewLineIndex > -1)
                rawValue = rawValue.Remove(lastNewLineIndex, Environment.NewLine.Length);

            MatchCollection singlesMatches =
                RegularExpression.Current.SingleCapturePattern.Matches(rawValue);

            if (singlesMatches.Count == 0)
            {
                directiveFactory =
                    new DirectiveFactory(DirectiveTypes.Static, rawValue);
                this.Directives.Add(directiveFactory);
                this._ResultHandler.Invoke(directiveFactory.Make(this._Arguments));
                return;
            }

            int lastIndex = 0;

            IEnumerator smEnum = singlesMatches.GetEnumerator();

            while (smEnum.MoveNext())
            {
                Match singleMatch = (Match)smEnum.Current;

                if (singleMatch.Index - lastIndex > 0)
                {
                    directiveFactory = 
                        new DirectiveFactory(
                            DirectiveTypes.Static, 
                            rawValue.Substring(lastIndex, singleMatch.Index - lastIndex)
                        );
                    this.Directives.Add(directiveFactory);
                    this._ResultHandler.Invoke(directiveFactory.Make(this._Arguments));
                }

                directiveFactory = null;
                
                string singleValue = Parser.ClearTags(singleMatch.Value);
                DirectiveTypes directiveType =
                    DirectiveHelper.CaptureDirectiveType(singleMatch.Value);

                switch (directiveType)
                {
                    case DirectiveTypes.Property:
                    case DirectiveTypes.Control:
                    case DirectiveTypes.Template:
                    case DirectiveTypes.Translation:
                    case DirectiveTypes.HashCodePointedTemplate:
                    case DirectiveTypes.Execution:
                        directiveFactory =
                            new DirectiveFactory(directiveType, singleValue);
                        break;
                }

                if (directiveFactory != null)
                {
                    this.Directives.Add(directiveFactory);
                    this._ResultHandler.Invoke(directiveFactory.Make(this._Arguments));
                }

                lastIndex = singleMatch.Index + singleMatch.Length;
            }

            if (rawValue.Length - lastIndex <= 1) return;
            
            directiveFactory =
                new DirectiveFactory(DirectiveTypes.Static, rawValue[lastIndex..]);
            this.Directives.Add(directiveFactory);
            this._ResultHandler.Invoke(directiveFactory.Make(this._Arguments));
        }

        private void HandleContent(Content content)
        {
            string rawValue = 
                this._ContentCache.ToString();
            rawValue = Parser.ClearTags(rawValue);

            DirectiveFactory directiveFactory = null;
            string directiveRawValue =
                $"${(string.IsNullOrEmpty(content.DirectiveType) ? content.DirectiveId : content.DirectiveType)}:";
            DirectiveTypes directiveType =
                DirectiveHelper.CaptureDirectiveType(directiveRawValue);
            
            switch (directiveType)
            {
                case DirectiveTypes.Control:
                case DirectiveTypes.ControlAsync:
                case DirectiveTypes.InLineStatement:    
                case DirectiveTypes.UpdateBlock:
                case DirectiveTypes.EncodedExecution:
                case DirectiveTypes.MessageBlock:
                case DirectiveTypes.PermissionBlock:
                case DirectiveTypes.PartialCache:
                case DirectiveTypes.ReplaceableTranslation:
                case DirectiveTypes.AsyncGroup:
                    directiveFactory = 
                        new DirectiveFactory(directiveType, rawValue);
                    break;
            }

            if (directiveFactory == null) return;
            
            this.Directives.Add(directiveFactory);
            this._ResultHandler.Invoke(directiveFactory.Make(this._Arguments));
        }

        private static string ClearTags(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue) || 
                rawValue.Length <= 2 || 
                rawValue[0] != '$' ||
                rawValue[^1] != '$') return rawValue;
            
            rawValue = rawValue.Substring(1, rawValue.Length - 2);
            rawValue = rawValue.Trim();

            return rawValue;
        }
    }
}