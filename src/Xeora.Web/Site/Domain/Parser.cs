using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xeora.Web.Directives;
using Xeora.Web.Directives.Elements;
using Xeora.Web.Exception;
using Xeora.Web.Global;

namespace Xeora.Web.Site
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
            public string DirectiveID { get; set; }
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

                    if (!string.IsNullOrEmpty(this._Crumb))
                    {
                        this._SingleCache.AppendLine(this._Crumb);
                        line = this._Crumb;
                    }
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
                string.Format("~{0}", this._SingleCache.Length + index);
            string directiveID =
                coMatch.Result("${DirectiveID}");
            string directiveType =
                coMatch.Result("${DirectiveType}");
                
            this._ContentCache.AppendLine(
                line.Substring(coMatch.Index)
            );
            this._ContentCache.Insert(
                coMatch.Value.Length - 2,
                modifierText
            );
            line = line.Substring(coMatch.Index + coMatch.Value.Length);

            if (!this.FindContentTail(line, directiveID, modifierText))
                throw new ParseException();

            return new Content
            {
                DirectiveID = directiveID,
                DirectiveType = directiveType,
            };
        }

        private bool FindContentTail(string crumb, string directiveID, string modifierText)
        {
            int nestedCount = 0;
            int ccIndex =
                this.SearchForContentClosing(ref nestedCount, crumb, directiveID);
            if (ccIndex > -1)
            {
                this._ContentCache.Remove(
                    this._ContentCache.Length - (crumb.Length + Environment.NewLine.Length) + ccIndex,
                    crumb.Length + Environment.NewLine.Length - ccIndex
                );
                this._ContentCache.Insert(
                    this._ContentCache.Length - 1,
                    modifierText
                );
                this._Crumb = crumb.Substring(ccIndex);

                return true;
            }

            while (this._Reader.Peek() > -1)
            {
                crumb = this._Reader.ReadLine();
                this._ContentCache.AppendLine(crumb);

                ccIndex = this.SearchForContentSeparator(ref nestedCount, crumb, directiveID);
                if (ccIndex > -1)
                {
                    this._ContentCache.Insert(
                        this._ContentCache.Length - (crumb.Length + Environment.NewLine.Length) + ccIndex - 2,
                        modifierText
                    );
                }

                ccIndex = this.SearchForContentClosing(ref nestedCount, crumb, directiveID);
                if (ccIndex == -1) continue;

                this._ContentCache.Remove(
                    this._ContentCache.Length - (crumb.Length + Environment.NewLine.Length) + ccIndex, 
                    crumb.Length + Environment.NewLine.Length - ccIndex
                );
                this._ContentCache.Insert(
                    this._ContentCache.Length - 1,
                    modifierText
                );
                this._Crumb = crumb.Substring(ccIndex);

                return true;
            }

            return false;
        }

        private int SearchForContentSeparator(ref int nestedCount, string line, string directiveID)
        {
            string ccSearch =
                string.Format("}}:{0}:{{", directiveID);
            int ccIndex = line.IndexOf(ccSearch);

            if (ccIndex == -1)
                return -1;

            MatchCollection contentOpeningMatches =
                RegularExpression.Current.ContentOpeningPattern.Matches(line);

            if (contentOpeningMatches.Count == 0)
                return ccIndex + ccSearch.Length;

            Match contentOpeningMatch;
            string compareID;

            IEnumerator comEnum =
                contentOpeningMatches.GetEnumerator();

            while (comEnum.MoveNext())
            {
                contentOpeningMatch = (Match)comEnum.Current;
                compareID =
                    contentOpeningMatch.Result("${DirectiveID}");

                if (string.Compare(compareID, directiveID) != 0)
                    continue;

                if (contentOpeningMatch.Index < ccIndex)
                {
                    nestedCount++;
                    continue;
                }

                return ccIndex + ccSearch.Length;
            }

            for (int i = 0; i < nestedCount; i++)
            {
                ccIndex = line.IndexOf(ccSearch, ccIndex + 1);
                if (ccIndex == -1) break;

                nestedCount--;
            }

            if (ccIndex > -1)
                ccIndex += ccSearch.Length;

            return ccIndex;
        }

        private int SearchForContentClosing(ref int nestedCount, string line, string directiveID)
        {
            string ccSearch =
                string.Format("}}:{0}$", directiveID);
            int ccIndex = line.IndexOf(ccSearch);

            if (ccIndex == -1)
                return -1;

            MatchCollection contentOpeningMatches =
                RegularExpression.Current.ContentOpeningPattern.Matches(line);

            if (contentOpeningMatches.Count == 0)
                return ccIndex + ccSearch.Length;

            Match contentOpeningMatch;
            string compareID;

            IEnumerator comEnum = 
                contentOpeningMatches.GetEnumerator();

            while (comEnum.MoveNext())
            {
                contentOpeningMatch = (Match)comEnum.Current;
                compareID =
                    contentOpeningMatch.Result("${DirectiveID}");

                if (string.Compare(compareID, directiveID) != 0)
                    continue;

                if (contentOpeningMatch.Index < ccIndex)
                {
                    nestedCount++;
                    continue;
                }

                return ccIndex + ccSearch.Length;
            }

            for (int i = 0; i < nestedCount; i++)
            {
                ccIndex = line.IndexOf(ccSearch, ccIndex + 1);
                if (ccIndex == -1) break;

                nestedCount--;
            }

            if (ccIndex > -1)
                ccIndex += ccSearch.Length;

            return ccIndex;
        }

        private void HandleSingles()
        {
            string rawValue = this._SingleCache.ToString();
            int lastNewLineIndex = 
                rawValue.LastIndexOf(Environment.NewLine);
            if (lastNewLineIndex > -1)
                rawValue = rawValue.Remove(lastNewLineIndex, Environment.NewLine.Length);

            MatchCollection singlesMatches =
                RegularExpression.Current.SingleCapturePattern.Matches(rawValue);

            if (singlesMatches.Count == 0)
            {
                this._ResultHandler.Invoke(
                    new Renderless(rawValue));
                return;
            }

            int lastIndex = 0;
            Match singleMatch;

            IEnumerator smEnum = singlesMatches.GetEnumerator();

            while (smEnum.MoveNext())
            {
                singleMatch = (Match)smEnum.Current;

                if (singleMatch.Index - lastIndex > 0)
                    this._ResultHandler.Invoke(
                        new Renderless(rawValue.Substring(lastIndex, singleMatch.Index - lastIndex)));

                string singleValue = this.ClearTags(singleMatch.Value);

                switch (DirectiveHelper.CaptureDirectiveType(singleMatch.Value))
                {
                    case DirectiveTypes.Property:
                        this._ResultHandler.Invoke(
                            new Property(singleValue, this._Arguments));

                        break;
                    case DirectiveTypes.Control:
                        this._ResultHandler.Invoke(
                            new Control(rawValue, null));

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

                lastIndex = singleMatch.Index + singleMatch.Value.Length;
            }

            if (rawValue.Length - lastIndex > 1)
                this._ResultHandler.Invoke(
                    new Renderless(rawValue.Substring(lastIndex)));
        }

        private void HandleContent(Content content)
        {
            string rawValue = 
                this._ContentCache.ToString();
            rawValue = this.ClearTags(rawValue);

            string directiveRawValue = string.Format("${0}:", (string.IsNullOrEmpty(content.DirectiveType) ? content.DirectiveID : content.DirectiveType));
            switch (DirectiveHelper.CaptureDirectiveType(directiveRawValue))
            {
                case DirectiveTypes.Control:
                    this._ResultHandler.Invoke(
                        new Control(rawValue, null)); 

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
                case DirectiveTypes.PartialCache:
                    this._ResultHandler.Invoke(
                        new PartialCache(rawValue, null));

                    break;
                case DirectiveTypes.FormattableTranslation:
                    this._ResultHandler.Invoke(
                        new FormattableTranslation(rawValue, null));

                    break;
            }
        }

        private string ClearTags(string rawValue)
        {
            if (!string.IsNullOrEmpty(rawValue) &&
                rawValue.Length > 2 &&
                rawValue[0] == '$' &&
                rawValue[rawValue.Length - 1] == '$')
            {
                rawValue = rawValue.Substring(1, rawValue.Length - 2);
                rawValue = rawValue.Trim();
            }

            return rawValue;
        }
    }
}