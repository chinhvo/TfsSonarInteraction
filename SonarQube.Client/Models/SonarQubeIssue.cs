/*
 * SonarQube Client
 * Copyright (C) 2016-2020 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

namespace SonarQube.Client.Models
{
    public class SonarQubeIssue
    {
        public SonarQubeIssue(string filePath, string hash, int? line, string message, string moduleKey, string ruleId,
            bool isResolved, TextRange textRange, string key, string assignee)
        {
            FilePath = "/" + filePath?.Trim('/', '\\');
            Hash = hash;
            Line = line;
            Message = message;
            ModuleKey = moduleKey;
            RuleId = ruleId;
            IsResolved = isResolved;
            TextRange = textRange;
            Key = key;
            Assignee = assignee;
            Issue = key;
        }
        public TextRange TextRange { get; }

        public string FilePath { get; }
        public string Hash { get; }
        public int? Line { get; }
        public string Message { get; }
        public string ModuleKey { get; }
        public string RuleId { get; }
        public bool IsResolved { get; }
        public string Key { get; set; }
        public string Assignee { get; set; }
        public string Issue { get; set; }

    }

    public class TextRange
    {
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }
        public int? StartOffset { get; set; }
        public int? EndOffset { get; set; }

    }
}
