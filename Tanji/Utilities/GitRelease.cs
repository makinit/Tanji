﻿/*
    GitHub(Source): https://GitHub.com/ArachisH/Tanji

    This file is part of Tanji.
    Copyright (C) 2015 ArachisH
    
    This code is licensed under the GNU General Public License.
    See License.txt in the project root for license information.
*/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tanji.Utilities
{
    [DataContract(Name = "release")]
    public class GitRelease
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "assets_url")]
        public string AssetsUrl { get; set; }

        [DataMember(Name = "upload_url")]
        public string UploadUrl { get; set; }

        [DataMember(Name = "html_url")]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "tag_name")]
        public string TagName { get; set; }

        [DataMember(Name = "target_commitish")]
        public string TargetCommitish { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "draft")]
        public bool IsDraft { get; set; }

        [DataMember(Name = "prerelease")]
        public bool IsPrerelease { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedOn { get; set; }

        [DataMember(Name = "published_at")]
        public string PublishedOn { get; set; }

        [DataMember(Name = "tarball_url")]
        public string TarballUrl { get; set; }

        [DataMember(Name = "zipball_url")]
        public string ZipballUrl { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "assets")]
        public List<GitAsset> Assets { get; set; }

        public override string ToString()
        {
            return TagName;
        }
    }
}