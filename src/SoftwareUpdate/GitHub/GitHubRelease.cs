// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SoftwareUpdate.GitHub
{
    /// <summary>
    /// Represents a GitHub release from the GitHub API.
    /// </summary>
    [DataContract]
    internal class GitHubRelease
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "tag_name")]
        public string TagName { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "draft")]
        public bool Draft { get; set; }

        [DataMember(Name = "prerelease")]
        public bool PreRelease { get; set; }

        [DataMember(Name = "published_at")]
        public string PublishedAt { get; set; }

        [DataMember(Name = "html_url")]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "assets")]
        public List<GitHubReleaseAsset> Assets { get; set; }

        /// <summary>
        /// Gets the published date as a DateTime.
        /// </summary>
        public DateTime GetPublishedDateTime()
        {
            if (DateTime.TryParse(PublishedAt, out var date))
                return date;
            return DateTime.MinValue;
        }
    }
}
