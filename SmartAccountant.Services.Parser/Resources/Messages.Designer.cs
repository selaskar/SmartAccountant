﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartAccountant.Services.Parser.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SmartAccountant.Services.Parser.Resources.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unrecognized file format. Column count ({0}) was expected to be at least 5..
        /// </summary>
        internal static string InsufficientColumnCount {
            get {
                return ResourceManager.GetString("InsufficientColumnCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Total amount of transactions and the remaining balance do not match..
        /// </summary>
        internal static string TransactionAmountAndBalanceNotMatch {
            get {
                return ResourceManager.GetString("TransactionAmountAndBalanceNotMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Total amount of transactions and the due amount do not match..
        /// </summary>
        internal static string TransactionAmountAndDueAmountNotMatch {
            get {
                return ResourceManager.GetString("TransactionAmountAndDueAmountNotMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unrecognized file format. Expected to have transaction date at column A (item: {0})..
        /// </summary>
        internal static string TransactionDateColumnMissing {
            get {
                return ResourceManager.GetString("TransactionDateColumnMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transaction (item: {0}) doesn&apos;t have a valid amount value..
        /// </summary>
        internal static string UnexpectedAmountFormat {
            get {
                return ResourceManager.GetString("UnexpectedAmountFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unrecognized date format. Could not parse transaction date &apos;{0}&apos; (item: {1})..
        /// </summary>
        internal static string UnexpectedDateFormat {
            get {
                return ResourceManager.GetString("UnexpectedDateFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unexpected error occurred while parsing the spreadsheet..
        /// </summary>
        internal static string UnexpectedError {
            get {
                return ResourceManager.GetString("UnexpectedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transaction (item: {0}) doesn&apos;t have a valid remaining amount value..
        /// </summary>
        internal static string UnexpectedRemainingAmountFormat {
            get {
                return ResourceManager.GetString("UnexpectedRemainingAmountFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a sheet in the document..
        /// </summary>
        internal static string UploadedDocumentMissingSheet {
            get {
                return ResourceManager.GetString("UploadedDocumentMissingSheet", resourceCulture);
            }
        }
    }
}
