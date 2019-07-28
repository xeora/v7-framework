using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xeora.Web.Directives;
using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Application
{
    internal class Parser : IDisposable
    {
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

        private Parser(Action<IDirective> resultHandler, string rawValue, ArgumentCollection arguments)
        {
            this._ResultHandler = resultHandler;
            this._Reader = new StringReader(rawValue);
            this._Arguments = arguments;

            this._SingleCache = new StringBuilder();
            this._ContentCache = new StringBuilder();
            this._Crumb = string.Empty;
        }

        public void Dispose() =>
            this._Reader.Close();

        private class Content
        {
            public string DirectiveId { get; set; }
            public string DirectiveType { get; set; }
        }

        private void Parse()
        {
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

            int separatorIndex = this.SearchForContentSeparator(this._ContentCache.ToString(), directiveId, openingPattern);
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
                this.SearchForContentClosing(ref nestedCount, line, directiveId, openingPattern);
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
        
        private int SearchForContentSeparator(string capturedContent, string directiveId, Regex openingPattern)
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

        private int SearchForContentClosing(ref int nestedCount, string line, string directiveId, Regex openingPattern)
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
            string rawValue = this._SingleCache.ToString();
            int lastNewLineIndex = 
                rawValue.LastIndexOf(Environment.NewLine, StringComparison.InvariantCulture);
            if (lastNewLineIndex > -1)
                rawValue = rawValue.Remove(lastNewLineIndex, Environment.NewLine.Length);

            MatchCollection singlesMatches =
                RegularExpression.Current.SingleCapturePattern.Matches(rawValue);

            if (singlesMatches.Count == 0)
            {
                this._ResultHandler.Invoke(
                    new Static(rawValue));
                return;
            }

            int lastIndex = 0;

            IEnumerator smEnum = singlesMatches.GetEnumerator();

            while (smEnum.MoveNext())
            {
                Match singleMatch = (Match)smEnum.Current;

                if (singleMatch.Index - lastIndex > 0)
                    this._ResultHandler.Invoke(
                        new Static(rawValue.Substring(lastIndex, singleMatch.Index - lastIndex)));

                string singleValue = this.ClearTags(singleMatch.Value);

                switch (DirectiveHelper.CaptureDirectiveType(singleMatch.Value))
                {
                    case DirectiveTypes.Property:
                        this._ResultHandler.Invoke(
                            new Property(singleValue, this._Arguments));

                        break;
                    case DirectiveTypes.Control:
                        this._ResultHandler.Invoke(
                            new Control(singleValue, null));

                        break;
                    case DirectiveTypes.Template:
                        this._ResultHandler.Invoke(
                            new Template(singleValue, null));

                        break;
                    case DirectiveTypes.Translation:
                        this._ResultHandler.Invoke(
                            new Translation(singleValue, null));

                        break;
                    case DirectiveTypes.HashCodePointedTemplate:
                        this._ResultHandler.Invoke(
                            new HashCodePointedTemplate(singleValue, null));

                        break;
                    case DirectiveTypes.Execution:
                        this._ResultHandler.Invoke(
                            new Execution(singleValue, null));

                        break;
                }

                lastIndex = singleMatch.Index + singleMatch.Length;
            }

            if (rawValue.Length - lastIndex > 1)
                this._ResultHandler.Invoke(
                    new Static(rawValue.Substring(lastIndex)));
        }

        private void HandleContent(Content content)
        {
            string rawValue = 
                this._ContentCache.ToString();
            rawValue = this.ClearTags(rawValue);

            string directiveRawValue =
                $"${(string.IsNullOrEmpty(content.DirectiveType) ? content.DirectiveId : content.DirectiveType)}:";
            switch (DirectiveHelper.CaptureDirectiveType(directiveRawValue))
            {
                case DirectiveTypes.Control:
                    this._ResultHandler.Invoke(
                        new Control(rawValue, null)); 

                    break;
                case DirectiveTypes.ControlAsync:
                    this._ResultHandler.Invoke(
                        new ControlAsync(rawValue, null));
                    break;
                case DirectiveTypes.InLineStatement:
                    this._ResultHandler.Invoke(
                        new InLineStatement(rawValue, null));

                    break;
                case DirectiveTypes.UpdateBlock:
                    this._ResultHandler.Invoke(
                        new UpdateBlock(rawValue, null));

                    break;
                case DirectiveTypes.EncodedExecution:
                    this._ResultHandler.Invoke(
                        new EncodedExecution(rawValue, null));

                    break;
                case DirectiveTypes.MessageBlock:
                    this._ResultHandler.Invoke(
                        new MessageBlock(rawValue, null));

                    break;
                case DirectiveTypes.PermissionBlock:
                    this._ResultHandler.Invoke(
                        new PermissionBlock(rawValue, null));

                    break;
                case DirectiveTypes.PartialCache:
                    this._ResultHandler.Invoke(
                        new PartialCache(rawValue, null));

                    break;
                case DirectiveTypes.ReplaceableTranslation:
                    this._ResultHandler.Invoke(
                        new ReplaceableTranslation(rawValue, null));

                    break;
                case DirectiveTypes.AsyncGroup:
                    this._ResultHandler.Invoke(
                        new AsyncGroup(rawValue, null));

                    break;
            }
        }

        private string ClearTags(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue) || 
                rawValue.Length <= 2 || 
                rawValue[0] != '$' ||
                rawValue[rawValue.Length - 1] != '$') return rawValue;
            
            rawValue = rawValue.Substring(1, rawValue.Length - 2);
            rawValue = rawValue.Trim();

            return rawValue;
        }
    }
}