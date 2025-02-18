﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.WindowsAzure.Commands.Common.CustomAttributes;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.ActiveDirectory
{
    /// <summary>
    /// Get AD users.
    /// </summary>
    [CmdletOutputBreakingChange(typeof(PSADUser), ReplacementCmdletOutputTypeName = "Microsoft.Azure.PowerShell.Cmdlets.Resources.MSGraph.Models.ApiV10.IMicrosoftGraphUser")]
    [Cmdlet("Get", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "ADUser", DefaultParameterSetName = ParameterSet.Empty, SupportsPaging = true), OutputType(typeof(PSADUser))]
    public class GetAzureADUserCommand : ActiveDirectoryBaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSet.SearchString,
            HelpMessage = "Used to find users that begin with the provided string.")]
        [Alias("SearchString")]
        [ValidateNotNullOrEmpty]
        public string StartsWith { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSet.DisplayName, HelpMessage = "The display name of the user.")]
        public string DisplayName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSet.ObjectId,
            HelpMessage = "The user object id.")]
        [ValidateNotNullOrEmpty]
        public string ObjectId { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSet.Empty,
            HelpMessage = "The user UPN.")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSet.UPN,
            HelpMessage = "The user UPN.")]
        [ValidateNotNullOrEmpty]
        [Alias("UPN")]
        public string UserPrincipalName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSet.Mail,
            HelpMessage = "The user mail.")]
        [ValidateNotNullOrEmpty]
        public string Mail { get; set; }

        public override void ExecuteCmdlet()
        {
            ExecutionBlock(() =>
            {
                ADObjectFilterOptions options = new ADObjectFilterOptions
                {
                    SearchString = this.IsParameterBound(c => c.StartsWith) ? StartsWith + "*" : DisplayName,
                    UPN = UserPrincipalName,
                    Id = ObjectId,
                    Paging = true,
                    Mail = Mail
                };
                if(!string.IsNullOrEmpty(options.SearchString))
                {
                    //query string is wrapped with single quote. Escape is needed if it contains additional quote.
                    options.SearchString = options.SearchString.Replace("'", "''");
                }
                 

                ulong first = MyInvocation.BoundParameters.ContainsKey("First") ? this.PagingParameters.First : ulong.MaxValue;
                ulong skip = MyInvocation.BoundParameters.ContainsKey("Skip") ? this.PagingParameters.Skip : 0;
                WriteObject(ActiveDirectoryClient.FilterUsers(options, first, skip), true);
            });
        }
    }
}
