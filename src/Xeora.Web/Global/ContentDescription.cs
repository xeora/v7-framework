using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xeora.Web.Global
{
    public class ContentDescription
    {
        private class PartCache
        {
            public string Content { get; set; }
            public List<string> Parts { get; set; }
            public string MessageTemplate { get; set; }
        }

        private static ConcurrentDictionary<string, PartCache> _PartsCache = 
            new ConcurrentDictionary<string, PartCache>();

        private const string MESSAGE_TEMPLATE_POINTER_TEXT = "!MESSAGETEMPLATE";

        public ContentDescription(string rawValue)
        {
            this.Parts = new List<string>();
            this.MessageTemplate = string.Empty;

            int modifierBeginIndex = rawValue.LastIndexOf('~');
            if (modifierBeginIndex == -1) return;

            string modifier =
                rawValue.Substring(modifierBeginIndex + 1);

            // Parse Block Content
            int firstContentEndIndex =
                rawValue.IndexOf(string.Format("{0}:{{", modifier), System.StringComparison.InvariantCulture);
            if (firstContentEndIndex == -1) return;

            string directiveIdentifier =
                rawValue.Substring(0, firstContentEndIndex);

            string blockContent = rawValue;
            bool isSpecialDirective = false;
            int colonIndex = 
                directiveIdentifier.IndexOf(':');

            if (colonIndex == -1) // Special Directive such as PC, MB, XF, AG
                isSpecialDirective = true;
            else // Common Directive such as DirectiveType:DirectiveID
            {
                blockContent = blockContent.Substring(colonIndex + 1);
                firstContentEndIndex -= colonIndex + 2; // 2 = 1: make it length + 1: skip colon
            }

            blockContent = this.CleanupParameters(firstContentEndIndex, blockContent);

            // ControlIDWithIndex is Like ControlID~INDEX
            string controlIDWithIndex = 
                blockContent.Substring(0, blockContent.IndexOf(":{", System.StringComparison.InvariantCulture));

            string openingTag = string.Format("{0}:{{", controlIDWithIndex);
            string closingTag = string.Format("}}:{0}", controlIDWithIndex);

            int idxCoreContStart =
                blockContent.IndexOf(openingTag, System.StringComparison.InvariantCulture) + openingTag.Length;
            int idxCoreContEnd =
                blockContent.LastIndexOf(closingTag, blockContent.Length, System.StringComparison.InvariantCulture);

            if (idxCoreContStart != openingTag.Length || idxCoreContEnd != (blockContent.Length - openingTag.Length))
                throw new Exception.ParseException();

            string coreContent = 
                blockContent.Substring(idxCoreContStart, idxCoreContEnd - idxCoreContStart);
            if (isSpecialDirective)
                coreContent = coreContent.Trim();

            if (this.TryCacheToPrepare(controlIDWithIndex, coreContent)) return;

            this.PrepareDesciption(coreContent, controlIDWithIndex, isSpecialDirective);
        }

        private string CleanupParameters(int firstContentEndIndex, string blockContent)
        {
            int parameterBeginIndex = blockContent.IndexOf('(');
            if (parameterBeginIndex == -1) return blockContent;
            
            if (firstContentEndIndex < parameterBeginIndex)
                return blockContent;

            return blockContent.Remove(parameterBeginIndex, firstContentEndIndex - parameterBeginIndex);
        }

        private bool TryCacheToPrepare(string controlIDWithIndex, string coreContent)
        {
            if (!ContentDescription._PartsCache.TryGetValue(controlIDWithIndex, out PartCache partCache))
                return false;

            if (string.Compare(partCache.Content, coreContent) != 0)
                return false;

            this.MessageTemplate = partCache.MessageTemplate;
            this.Parts = partCache.Parts;

            return true;
        }

        private void PrepareDesciption(string content, string controlIDWithIndex, bool isSpecialDirective)
        {
            string searchString =
                string.Format("}}:{0}:{{", controlIDWithIndex);
            int sIdx, cIdx = 0;

            do
            {
                sIdx = content.IndexOf(searchString, cIdx, System.StringComparison.InvariantCulture);
                if (sIdx == -1)
                    sIdx = content.Length;

                string contentPart =
                    content.Substring(cIdx, sIdx - cIdx);
                cIdx = sIdx + searchString.Length;

                if (isSpecialDirective)
                    contentPart = contentPart.Trim();

                if (contentPart.IndexOf(ContentDescription.MESSAGE_TEMPLATE_POINTER_TEXT, System.StringComparison.InvariantCulture) == 0)
                {
                    if (!string.IsNullOrEmpty(this.MessageTemplate))
                        throw new Exception.MultipleBlockException("Only One Message Template Block Allowed!");

                    this.MessageTemplate = contentPart.Substring(ContentDescription.MESSAGE_TEMPLATE_POINTER_TEXT.Length);

                    continue;
                }

                if (string.IsNullOrEmpty(contentPart)) continue;

                this.Parts.Add(contentPart);
            } while (sIdx != content.Length);

            if (!this.HasParts)
                throw new Exception.EmptyBlockException();

            ContentDescription._PartsCache.TryAdd(
                controlIDWithIndex,
                new PartCache
                {
                    Content = content,
                    Parts = this.Parts,
                    MessageTemplate = this.MessageTemplate
                }
            );
        }

        public List<string> Parts { get; private set; }
        public bool HasParts => this.Parts.Count > 0;

        public string MessageTemplate { get; private set; }
        public bool HasMessageTemplate => !string.IsNullOrEmpty(this.MessageTemplate);
    }
}