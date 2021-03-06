//
// social.cs: API definition for Apple's Social Framework
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2013 Xamarin Inc
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.Accounts;

#if !MONOMAC
using XamCore.UIKit;
#endif
#if MONOMAC
using XamCore.AppKit;
#endif

namespace XamCore.Social {
	[Since (6,0)]
	[Mac (10,8, onlyOn64 : true)]
	[Static]
	interface SLServiceType {
		[Field ("SLServiceTypeFacebook")]
		[Mac (10,8)]
		NSString Facebook { get; }

		[Field ("SLServiceTypeTwitter")]
		[Mac (10,8)]
		NSString Twitter { get; }

		[Field ("SLServiceTypeSinaWeibo")]
		[Mac (10,8)]
		NSString SinaWeibo { get; }

		[Since (7,0)]
		[Field ("SLServiceTypeTencentWeibo")]
		[Mac (10,9)]
		NSString TencentWeibo { get; }

#if MONOMAC
		[Field ("SLServiceTypeLinkedIn")]
		[Mac (10,9)]
		NSString LinkedIn { get; }
#endif
	}
	
	[Since (6,0)]
	[BaseType (typeof (NSObject))]
	// init -> Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: SLRequestMultiPart must be obtained through!
	[DisableDefaultCtor]
	[Mac (10,8, onlyOn64 : true)]
	interface SLRequest {
		[Static]
		[Export ("requestForServiceType:requestMethod:URL:parameters:")]
		SLRequest Create (NSString serviceType, SLRequestMethod requestMethod, NSUrl url, [NullAllowed] NSDictionary parameters);

		[Export ("account", ArgumentSemantic.Retain), NullAllowed]
		ACAccount Account { get; set;  }

		[Export ("requestMethod")]
		SLRequestMethod RequestMethod { get;  }

		[Export ("URL")]
		NSUrl Url { get;  }

		[Export ("parameters")]
		NSDictionary Parameters { get;  }

		[Export ("addMultipartData:withName:type:filename:")]
		void AddMultipartData (NSData data, string partName, string partType, string filename);

		[Export ("preparedURLRequest")]
		NSUrlRequest GetPreparedUrlRequest ();

		// async 
		[Export ("performRequestWithHandler:")]
		[Async (ResultTypeName = "SLRequestResult")]
		void PerformRequest (Action<NSData,NSHttpUrlResponse,NSError> handler);
	}

#if !MONOMAC
	[Since (6,0)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor] // see note on 'composeViewControllerForServiceType:'
	interface SLComposeViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("serviceType")]
		NSString ServiceType { get;  }

		[Export ("completionHandler", ArgumentSemantic.Copy)]
		Action<SLComposeViewControllerResult> CompletionHandler { get; set;  }

		[Static]
		[Export ("composeViewControllerForServiceType:")]
		// note: Use this method to create a social compose view controller. Do not use any other methods.
		SLComposeViewController FromService (NSString serviceType);

		[Static]
		[Export ("isAvailableForServiceType:")]
		bool IsAvailable (NSString serviceType);

		[Export ("setInitialText:")]
		bool SetInitialText (string text);

		[Export ("addImage:")]
		bool AddImage (UIImage image);

		[Export ("removeAllImages")]
		bool RemoveAllImages ();

		[Export ("addURL:")]
		bool AddUrl (NSUrl url);

		[Export ("removeAllURLs")]
		bool RemoveAllUrls ();
	}

	[Mac (10,10, onlyOn64 : true)]
	[iOS (8,0)]
#if MONOMAC
	[BaseType (typeof (NSViewController))]
#else
	[BaseType (typeof (UIViewController))]
#endif
#if XAMCORE_2_0
	#if MONOMAC
	interface SLComposeServiceViewController : NSTextViewDelegate {
	#else
	interface SLComposeServiceViewController : UITextViewDelegate {
	#endif
#else
	interface SLComposeServiceViewController {
#endif
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("presentationAnimationDidFinish")]
		void PresentationAnimationDidFinish ();

#if !MONOMAC
		[Export ("textView")]
		UITextView TextView { get; }
#else
		[Export ("textView")]
		NSTextView TextView { get; }
#endif

		[Export ("contentText")]
		string ContentText { get; }

		[NullAllowed] // by default this property is null
		[Export ("placeholder")]
		string Placeholder { get; set; }

		[Export ("didSelectPost")]
		void DidSelectPost ();

		[Export ("didSelectCancel")]
		void DidSelectCancel ();

		[Export ("cancel")]
		void Cancel ();

		[Export ("isContentValid")]
		bool IsContentValid ();

		[Export ("validateContent")]
		void ValidateContent ();

		[NullAllowed] // by default this property is null
		[Export ("charactersRemaining", ArgumentSemantic.Strong)]
		NSNumber CharactersRemaining { get; set; }

#if !MONOMAC
		[Export ("configurationItems")]
		SLComposeSheetConfigurationItem [] GetConfigurationItems ();

		[Export ("reloadConfigurationItems")]
		void ReloadConfigurationItems ();

		[Export ("pushConfigurationViewController:")]
		void PushConfigurationViewController (UIViewController viewController);

		[Export ("popConfigurationViewController")]
		void PopConfigurationViewController ();

		[Export ("loadPreviewView")]
		UIView LoadPreviewView();

		[NullAllowed] // by default this property is null
		[Export ("autoCompletionViewController", ArgumentSemantic.Strong)]
		UIViewController AutoCompletionViewController { get; set; }
#endif
	}
#endif


#if !MONOMAC
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SLComposeSheetConfigurationItem {
		[NullAllowed] // by default this property is null
		[Export ("title")]
		string Title { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("value")]
		string Value { get; set; }

		[Export ("valuePending", ArgumentSemantic.Assign)]
		bool ValuePending { get; set; }

		[Export ("tapHandler", ArgumentSemantic.Copy)]
		Action TapHandler { get; set; }
	}
#endif
}
#endif
