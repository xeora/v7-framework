using System;
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

        private static readonly ConcurrentDictionary<string, PartCache> PartsCache = 
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
                rawValue.IndexOf($"{modifier}:{{", System.StringComparison.InvariantCulture);
            if (firstContentEndIndex == -1) return;

            string directiveIdentifier =
                rawValue.Substring(0, firstContentEndIndex);

            string blockContent = rawValue;
            bool isSpecialDirective = false;
            int colonIndex = 
                directiveIdentifier.IndexOf(':');

            if (colonIndex == -1) // Special Directive such as PC, MB, XF, AG
                isSpecialDirective = true;
            else // Common Directive such as DirectiveType:DirectiveId
            {
                blockContent = blockContent.Substring(colonIndex + 1);
                firstContentEndIndex -= colonIndex + 2; // 2 = 1: make it length + 1: skip colon
            }

            blockContent = this.CleanupParameters(firstContentEndIndex, blockContent);

            // ControlIdWithIndex is Like ControlId~INDEX
            string controlIdWithIndex = 
                blockContent.Substring(0, blockContent.IndexOf(":{", System.StringComparison.InvariantCulture));

            string openingTag = $"{controlIdWithIndex}:{{";
            string closingTag = $"}}:{controlIdWithIndex}";

            int idxCoreContStart =
                blockContent.IndexOf(openingTag, System.StringComparison.InvariantCulture) + openingTag.Length;
            int idxCoreContEnd =
                blockContent.LastIndexOf(closingTag, blockContent.Length, System.StringComparison.InvariantCulture);

            if (idxCoreContStart != openingTag.Length || idxCoreContEnd != blockContent.Length - openingTag.Length)
                throw new Exception.ParseException();

            string coreContent = 
                blockContent.Substring(idxCoreContStart, idxCoreContEnd - idxCoreContStart);
            if (isSpecialDirective)
                coreContent = coreContent.Trim();

            if (this.TryCacheToPrepare(controlIdWithIndex, coreContent)) return;

            this.PrepareDescription(coreContent, controlIdWithIndex, isSpecialDirective);
        }

        private string CleanupParameters(int firstContentEndIndex, string blockContent)
        {
            int parameterBeginIndex = blockContent.IndexOf('(');
            if (parameterBeginIndex == -1) return blockContent;
            
            return firstContentEndIndex < parameterBeginIndex ? blockContent : blockContent.Remove(parameterBeginIndex, firstContentEndIndex - parameterBeginIndex);
        }

        private bool TryCacheToPrepare(string controlIdWithIndex, string coreContent)
        {
            if (!ContentDescription.PartsCache.TryGetValue(controlIdWithIndex, out PartCache partCache))
                return false;

            if (String.CompareOrdinal(partCache.Content, coreContent) != 0)
                return false;

            this.MessageTemplate = partCache.MessageTemplate;
            this.Parts = partCache.Parts;

            return true;
        }

        private void PrepareDescription(string content, string controlIdWithIndex, bool isSpecialDirective)
        {
            string searchString =
                $"}}:{controlIdWithIndex}:{{";
            int sIdx, cIdx = 0;

            do
            {
                sIdx = content.IndexOf(searchString, cIdx, StringComparison.InvariantCulture);
                if (sIdx == -1)
                    sIdx = content.Length;

                string contentPart =
                    content.Substring(cIdx, sIdx - cIdx);
                cIdx = sIdx + searchString.Length;

                if (isSpecialDirective)
                    contentPart = contentPart.Trim();

                if (contentPart.IndexOf(ContentDescription.MESSAGE_TEMPLATE_POINTER_TEXT, StringComparison.InvariantCulture) == 0)
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
            {
                this.Parts.Add(string.Empty);

                Basics.Console.Push("WARNING (ContentDescriptor)", $"Empty Block is detected! [{controlIdWithIndex}]", string.Empty, true);
            }

            ContentDescription.PartsCache.TryAdd(
                controlIdWithIndex,
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